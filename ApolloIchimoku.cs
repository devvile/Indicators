#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class ApolloIchimoku : Indicator
	{
		//	for Ichimoku Signals by kkc2015, see document from IchimokuTrader.com
		private const int icTrendSpan	= 4;	//	4 previous trading days, used for avoiding whipsaw signals
		private const int icNoSignal	= 0;
		private const int icBelowCloud	= 1;	//	signal occurred below the cloud
		private const int icInCloud		= 2;
		private const int icAboveCloud	= 3;
		private const int icSignal_Base	= 11;	//	use for adjusting icSignal_ to start from 0
		private const int icSignal_TK	= icSignal_Base+0;	//	TK = signal for Tenkan / Kijun sen crossed up
		private const int icSignal_PK	= icSignal_Base+1;	//	do not change number sequence from _TK to _CP
		private const int icSignal_KB	= icSignal_Base+2;
		private const int icSignal_SS	= icSignal_Base+3;
		private const int icSignal_CP	= icSignal_Base+4;	//	do not change number sequence from _TK to _CP
		private const int icSignal_LS	= icSignal_Base+5;	//	trade record [Long/Short]
		private const int icSignal_DU	= icSignal_Base+6;	//	Alert [Down / Up trend]
		private const int icSignalType	= 5;	//	exclude icSignal_LS
		private const int icReadTradeData = -2;	//	flag to show that ReadTradeData is completed, do not repeat
		private const int icMACD_data = -3;		//	flag to show that MACD data is completed, do not repeat
		private const int icMACD_Peak = 1;		//	check for MACD peaks
		private const int icMACD_Valley = 2;	//	check for MACD valleys
		private const bool bMACD_Completed = true;	//	used by MACD function
		
		private static readonly int[] icSignals = {icSignal_TK,icSignal_PK,icSignal_KB,icSignal_SS,icSignal_CP};
		private static readonly string[] scSignals = {"TK", "PK", "KB", "SS", "CP", "LS", "DU"};
		
		private const int icSignal_start	= 100;	//	use for adjusting TK_DN to start from 1
		private const int icSignal_TK_DN	= 101;	//	_DN = odd number = signal for Tenkan / Kijun sen crossed down
		private const int icSignal_TK_UP	= 102;	//	_UP = even number = signal for Tenkan / Kijun sen crossed up
		private const int icSignal_PK_DN	= 103;
		private const int icSignal_PK_UP	= 104;
		private const int icSignal_KB_DN	= 105;
		private const int icSignal_KB_UP	= 106;
		private const int icSignal_SS_DN	= 107;
		private const int icSignal_SS_UP	= 108;
		private const int icSignal_CP_DN	= 109;
		private const int icSignal_CP_UP	= 110;
		
		private const int icSignal_Weak		= 1;
		private const int icSignal_Neutral	= 2;
		private const int icSignal_Strong	= 3;
		private const int icSignal_Trade	= 4;
		//	5 Ichimoku signal types
		private int iSignal			= icNoSignal;
		private string sSignalCode	= " ";
		private bool bArrowDn		= false;
		private int iSignalStrength	= icSignal_Neutral;
		private double dChart_Ymin	= 90.0; 	//	Y value (=Price) at the bottom of chart 
		private double dChart_Ymax	= 134.0; 	//	Y value (=Price) at the top of chart
		private double dChart_Y		= 0.0;		//	Y location for showing the arrow
		private	double dChart_Yspan = 100.0;
		private static int iNbrSameBar = 0;
		private string sLegend = "SignalColor: LightGray(weak), NoColor(Neutral), Green/Red(strong)\n" +
							"Cross:\tTK - Tenkan / Kijun,  PK - Price / Kijun,  KB - Price / Kumo,\n\tSS - Senkou SpanA / SpanB,  CP - Chikou / Price\n";
							
		NinjaTrader.Gui.Tools.SimpleFont LegendFont = new NinjaTrader.Gui.Tools.SimpleFont("Courier New", 12);
		private Brush ArrowBrush	= Brushes.DarkGray;		//	use for debugging, do not remove
		
		//	for SharpDX drawing of Ichimoku cloud and Indicator ColorBar
		private Brush upAreaBrush	= Brushes.LightGreen;
		private Brush dnAreaBrush	= Brushes.Crimson;
		private Brush upLineBrush	= Brushes.DarkGreen;
		private Brush dnLineBrush	= Brushes.Red;
		private Brush textBrush		= Brushes.White;
		int iareaOpacity = 55;		//	this provides reasonable cloud density, can be changed by user input
		const float fontHeight		= 12f;
		int iX_barWidth = 10;		//	space for each bar, initialize at OnRender()

		private SharpDX.Direct2D1.Brush	upAreaBrushDx, dnAreaBrushDx, upLineBrushDx, dnLineBrushDx, textBrushDx;
		
		private static int iSignalIdx = 0;
		private bool bRendering = false;
		private static bool bGetSignal = false;
		private const int icSignalMax = 2000;
		private const int icSignalSort = -999;	//	to indicate that stSignal_all[] requires sorting
		private struct stSignal
		{
			public int iBar;
			public int iSignal;
			public bool bTrendDown;
			public int iStrength;
			public int iNbrSignal;
		};
		//	[0].iBar = total number of signals. [0].iNbrSignal = icReadTradeData, [0].iStrength = icMACD_data
		private stSignal[] stSignal_all = new stSignal[icSignalMax+1];

		//	2017.02.06 - added for external access of CloudBreak signal
		private const int icCloudBreakUp	= 1;
		private const int icCloudBreakDn	= -1;
		private const int icCloudBreakNone	= 0;
		[XmlIgnore]
		public Series<int> iExt_Signal;				//	expose signals for external access, 2017.06.19
		[XmlIgnore]
		public Series<int> iExt_SignalStrength;		//	expose signal strength for external access, 2017.06.19

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= @"Display Ichimoku cloud / indicators.";
				Name						= "IchimokuSignal_Apollo";
				Calculate					= Calculate.OnBarClose;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				DrawOnPricePanel			= true;
				DrawHorizontalGridLines		= true;
				DrawVerticalGridLines		= true;
				PaintPriceMarkers			= true;
				ScaleJustification			= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive	= true;
				Conversion					= 9;
				BBase						= 26;
				SpanB						= 52;
				Lag							= 26;



				upAreaBrush 				= Brushes.LightGreen;
				dnAreaBrush 				= Brushes.Crimson;
				upLineBrush 				= Brushes.DarkGreen;
				dnLineBrush 				= Brushes.Red;
				iareaOpacity				= 55;
				
				AddPlot(Brushes.Purple, "ConversionLine");	//	Plots[0]
				AddPlot(Brushes.Teal, "BaseLine");			//	Plots[1]
				AddPlot(Brushes.Transparent, "SpanALine");	//	Plots[2]
				AddPlot(Brushes.Transparent, "SpanBLine");	//	Plots[3]
				AddPlot(Brushes.Transparent, "LagLine");	//	Plots[5]

			}
			else if (State == State.Configure)
			{

				ZOrder = -1;	//	2016.05.24, per ReusableBrushExample
				iExt_Signal	= new Series<int>(this);
				iExt_SignalStrength	= new Series<int>(this);
			}
			else if (State == State.Historical)
			{
				//	2017.06.19,	avoid crashes when creating DxBrush(), also not required to clone brushes when RenderTarget==null
				if(RenderTarget != null)
				{				
					//	2016.05.24	from Ninjascript ReuseDxBrushesExample.cs
					if (upAreaBrush.IsFrozen)
						upAreaBrush = upAreaBrush.Clone();		//	this will ensure that previous drawing using this brush is not affected
					upAreaBrush.Opacity = iareaOpacity / 100d;		//	.Opacity[0..1]
					upAreaBrush.Freeze();	//	freeze brush so that it can be changed later by other functions		
					upAreaBrushDx = upAreaBrush.ToDxBrush(RenderTarget);
					
					if (dnAreaBrush.IsFrozen)
						dnAreaBrush = dnAreaBrush.Clone();
					dnAreaBrush.Opacity = iareaOpacity / 100d;
					dnAreaBrush.Freeze();
					dnAreaBrushDx = dnAreaBrush.ToDxBrush(RenderTarget);

					//	the following brushes are not to be changed
					upLineBrushDx = upLineBrush.ToDxBrush(RenderTarget);		
					dnLineBrushDx = dnLineBrush.ToDxBrush(RenderTarget);
					textBrushDx = textBrush.ToDxBrush(RenderTarget);
				}
			}
		}

		//	2016.05.24
		public override void OnRenderTargetChanged()
		{
			if (upAreaBrushDx != null)
				upAreaBrushDx.Dispose();

			if (dnAreaBrushDx != null)
				dnAreaBrushDx.Dispose();

			if (upLineBrushDx != null)
				upLineBrushDx.Dispose();

			if (dnLineBrushDx != null)
				dnLineBrushDx.Dispose();

			if (textBrushDx != null)
				textBrushDx.Dispose();

			if (RenderTarget != null)	//	another rendering is starting
			{
				try
				{
					upAreaBrushDx	= upAreaBrush.ToDxBrush(RenderTarget);
					dnAreaBrushDx	= dnAreaBrush.ToDxBrush(RenderTarget);
					upLineBrushDx	= upLineBrush.ToDxBrush(RenderTarget);
					dnLineBrushDx	= dnLineBrush.ToDxBrush(RenderTarget);
					textBrushDx		= textBrush.ToDxBrush(RenderTarget);
				}
				catch (Exception e) { }
			}
		}

		//	kkc_2015, 2016.01.06	
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			if( Bars == null || ChartControl == null || Bars.Instrument == null || !IsVisible || IsInHitTest || bRendering) 
				return;

			bRendering = true;
			iX_barWidth = chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.FromIndex+1)
							- chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.FromIndex);
			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode = RenderTarget.AntialiasMode;	//	save for restore later			
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
			
			SharpDxDrawCLoud(chartControl, chartScale, SpanALine, SpanBLine, BBase);	
			base.OnRender(chartControl, chartScale);
			
			// get chart XYscale data, global variables with Indicator
			dChart_Ymin = chartScale.MinValue;
			dChart_Ymax = chartScale.MaxValue;
			dChart_Yspan = chartScale.MaxMinusMin;
			dChart_Y = dChart_Ymin + dChart_Yspan*0.01;

			int iX1 = ChartBars.FromIndex;		//	global parameter, absolute bar number

			if(ChartBars.Bars.Count-iX1 > 50)
			{
				bool bPrintText = true;
				for(int i=0; i<50; i++)		//	legend display width = 50 bars
					if(dChart_Ymax - High.GetValueAt(iX1+i) < 0.1*dChart_Yspan)
						bPrintText = false;
				if(bPrintText)
					Draw.TextFixed(this, "Legend", sLegend, TextPosition.TopLeft, Brushes.Black, LegendFont, Brushes.Blue, Brushes.LightGreen, 35);
				else
					Draw.TextFixed(this, "Legend", "", TextPosition.TopLeft, Brushes.Transparent, LegendFont, Brushes.Transparent, Brushes.Transparent, 0);
			}
			
			//	restore render properties and dispose of resources
			RenderTarget.AntialiasMode = oldAntialiasMode;
			bRendering = false;	//	rendering is completed
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			iExt_Signal[0]	= icNoSignal;	//	initialize to no signal, 2017.06.19
			iExt_SignalStrength[0] = icSignal_Neutral;		//	initialize, 2017.06.19
			
			if(CurrentBar < SpanB)
			{
				iSignalIdx = 0;
				stSignal_all[0].iBar = iSignalIdx;	//	initialize maximum number of bars
				stSignal_all[0].iSignal = icSignalSort;	//	initialize signal to sort the struct
				stSignal_all[0].iNbrSignal = -1;		//	initialize to indicate fresh set of data
				stSignal_all[0].iStrength = -1;
			}
			if(CurrentBar < SpanB || CurrentBar < Conversion || CurrentBar < Lag || CurrentBar < BBase){return;}

			//	Tenkan sen = ConversionLine, Kijun sen = BaseLine, Senkou Span A = SpanALine, Senkou Span B = SpanBLine
			ConversionLine[0] = ((MAX(High,Conversion)[0] + MIN(Low,Conversion)[0]) / 2);
			BaseLine[0] = ((MAX(High,BBase)[0] + MIN(Low,BBase)[0]) / 2);
			SpanALine[0] = ((ConversionLine[0] + BaseLine[0]) / 2);
			SpanBLine[0] = ((MAX(High,SpanB)[0] + MIN(Low,SpanB)[0]) / 2);
			LagLine[Lag] = Close[0];
			

			//	kkc_2015 2015.12.31  display Ichimoku signals as per IchimokuTrader.com
			dChart_Y = Low[0] - 20*TickSize;
		}

		#region Misc_functions

	
		//	sort the bar into sequential order
		protected bool bBarSort()
		{
			bool bSortReqd = false;
			if(stSignal_all[0].iSignal != icSignalSort)
				return(bSortReqd);

			int i = 1, j = 1, k = 1;
			stSignal stTemp;				
			while(++i <= stSignal_all[0].iBar)
			{
				if(stSignal_all[i].iBar < stSignal_all[i-1].iBar)
				{
					stTemp = stSignal_all[i];
					j = i-1;
					while((--j>0) && (stSignal_all[j].iBar > stSignal_all[i].iBar));
					for(k=i; k>j+1; k--)
						stSignal_all[k] = stSignal_all[k-1];
					stSignal_all[j+1] = stTemp;
				}
			}
			stSignal_all[0].iSignal = 0;		//	all sorted
			return(bSortReqd);
		}
		

		protected void SharpDxDrawArrow(Point pStart, SharpDX.Direct2D1.Brush lineBrushDx, SharpDX.Direct2D1.Brush fillBrushDx, bool iUpArrow=true)
		{
			const int icPoints = 9;
			float iX = (pStart.X > 6) ? (float)pStart.X : 6f;		//	make sure there is at least n pixels on chart left side
			float iY = (float)pStart.Y;		//	Y is always positive
			float [,] XY = new float[icPoints,2] {{0,0}, {2,0}, {2,-4}, {6,-4}, {0,-10}, {-6,-4}, {-2,-4}, {-2,0}, {0,0}};
			Point P = new Point();
			SharpDX.Vector2[] vector = new SharpDX.Vector2[icPoints];
			float sgn = iUpArrow ? 1f : -1f;

			for(int i=0; i<icPoints; i++)
			{
				P.X = iX + XY[i,0];
				P.Y = iY + sgn*XY[i,1];
				vector[i] = P.ToVector2();
			}

			DrawGeometry(vector, lineBrushDx, fillBrushDx, true);
		}



		//	kkc_2015, 2016.05.25, must be called by OnRender	
		protected void SharpDxDrawCLoud(ChartControl chartControl, ChartScale chartScale, ISeries<double> SpanA_s, ISeries<double> SpanB_s, int iOffset)
		{
			// RenderTarget is always full panel, so we need to be mindful which sub ChartPanel we're dealing with
			// always use ChartPanel X, Y, W, H - as ActualWidth, Width, ActualHeight, Height are in WPF units, so they can be drastically different depending on DPI set

			int iX_start = chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.FromIndex);		//	panel.X location
			int iX_End = chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.ToIndex);
			int iBarWidth = chartControl.GetXByBarIndex(chartControl.BarsArray[0], 1) - chartControl.GetXByBarIndex(chartControl.BarsArray[0], 0);
			int iBar_Start = (ChartBars.FromIndex > iOffset*3) ? ChartBars.FromIndex : iOffset*3;		//	global parameter, absolute bar number
			int iNbrBarSpaceAvailable = (ChartPanel.W - iX_End) / iBarWidth - 1;
			if(ChartBars.ToIndex - iBar_Start < 2)		//	
				return;
			
			int iBar_End = ChartBars.ToIndex + Math.Min(iOffset, iNbrBarSpaceAvailable);
			
			int iPointMax = iBar_End - iBar_Start +1;
			int[] iX = new int[iPointMax];		//	chartX coordinate, located at center of bar
			int[] iA = new int[iPointMax];		//	chartY coordinate for series A
			int[] iB = new int[iPointMax];

			int idx, idx_offset;		//	index for series A, B
			int i, j, k, m;
   			for(i = 0; i < iPointMax; i++)
			{
				idx = i + iBar_Start;
				idx_offset = idx - iOffset;
				iX[i] = chartControl.GetXByBarIndex(chartControl.BarsArray[0], idx);
				iA[i] = chartScale.GetYByValue(SpanA_s.GetValueAt(idx_offset));
				iB[i] = chartScale.GetYByValue(SpanB_s.GetValueAt(idx_offset));
			}

			double dX = iX[0];		//	initialize prior to entering while() loop
			double dY = iA[0];
			Point p = new Point();
			int iVectorMemberMax;
			bool bUpCloud = (iA[0] > iB[0]);		//	Uptrend
			bool bReqd = true;
			idx = iBar_Start;
			i = 0; j = 0;
			while(i < iPointMax-1)
			{
				k = i;	//	k has the starting point for each geometry
				while((i < iPointMax) && (iA[i] > iB[i]) == bUpCloud)		//	must check i<iPointMax first, prior to == bUpCloud
					i++;
				iVectorMemberMax = 2*(i-k+1);	//	Points for iA + iB + connecting point between iA and iB
				SharpDX.Vector2[] vectorSpan = new SharpDX.Vector2[iVectorMemberMax];
				m = 0;
				p.X = dX;	//	use last data point at the cross of SpanA_s and SpanB_s as starting point
				p.Y = dY;
				vectorSpan[m++] = p.ToVector2();
				
				for(j=k; j<i; j++)
				{
					p.X = iX[j];
					p.Y = iA[j]; 	//	data for SpanA_s series
					vectorSpan[m++] = p.ToVector2();
				}
				if(i < iPointMax-1)	//	Not the last point, need to create the intersection point
				{
					//	calculate iA, iB connecting point using equation
					dX = (iA[j-1]-iB[j-1]) * iX_barWidth / (iB[j]-iA[j]+iA[j-1]-iB[j-1]);	//	dX portion of iX_barWidth
					dY = (iB[j]-iB[j-1]) / iX_barWidth * dX + iB[j-1];
					dX += iX[j-1];	//	actual dX including iX_barWidth portion
					p.X = dX;
					p.Y = dY;
				}
				else
				{
					//	no iA / iB crossed, therefore draw straight line between iA & iB
					p.X = iX[j-1];
					p.Y = iB[j-1];
				}
				vectorSpan[m++] = p.ToVector2();
				for(j=i-1; j>=k; j--)
				{
					p.X = iX[j];
					p.Y = iB[j]; 	//	data for SpanB_s series
					vectorSpan[m++] = p.ToVector2();
				}
				//	no need to connect the line end to start
				if(!bUpCloud)	//	bUpCloud has the next cloud status, previous status = !bUpCloud
					DrawGeometry(vectorSpan, upLineBrushDx, upAreaBrushDx, false);
				else
					DrawGeometry(vectorSpan, dnLineBrushDx, dnAreaBrushDx, false);
				if(i < iPointMax)
					bUpCloud = (iA[i] > iB[i]);		//	placed in this location to ensure iA[i] is not out of range
			}
			//	draw the cloud outline
			SharpDX.Vector2[] vectorSpanA = new SharpDX.Vector2[iPointMax];
			SharpDX.Vector2[] vectorSpanB = new SharpDX.Vector2[iPointMax];
			Point p1 = new Point();
			Point p2 = new Point();

			for(i = 0; i < iPointMax-2; i++)
			{
				p1.X = iX[i];
				p2.X = iX[i+1];
				p1.Y = iA[i];
				p2.Y = iA[i+1];
				RenderTarget.DrawLine(p1.ToVector2(), p2.ToVector2(), upLineBrushDx, 1);
				p1.Y = iB[i];
				p2.Y = iB[i+1];
				RenderTarget.DrawLine(p1.ToVector2(), p2.ToVector2(), dnLineBrushDx, 1);
			}
		}

		protected void DrawGeometry(SharpDX.Vector2[] vectorSpan, SharpDX.Direct2D1.Brush LineBrushDx, SharpDX.Direct2D1.Brush AreaBrushDx, bool bDrawOutline)
		{
			//	the line in the vector must be continuous, unclosed gemoetry will be closed automatically
			//	all elements in the vector must be provided with data
			SharpDX.Direct2D1.PathGeometry geo1 = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
			SharpDX.Direct2D1.GeometrySink sink1 = geo1.Open();
			Point iP_start = new Point(vectorSpan[0].X, vectorSpan[0].Y);
			sink1.BeginFigure(iP_start.ToVector2(), SharpDX.Direct2D1.FigureBegin.Filled);
			sink1.AddLines(vectorSpan);
			sink1.EndFigure(SharpDX.Direct2D1.FigureEnd.Closed);
			sink1.Close();
			RenderTarget.FillGeometry(geo1, AreaBrushDx);
			if(bDrawOutline)
				RenderTarget.DrawGeometry(geo1, LineBrushDx);	//	draw shape outline
			geo1.Dispose();
			sink1.Dispose();
		}		
		
		
		
		protected int iGetCloudStatus(double dY, int iLag=0)
		{
			//	dY has data from 1 BarAgo 
			int iRetSignal = icInCloud;
			
			if(dY < Math.Min(SpanALine[0+BBase+iLag],SpanBLine[0+BBase+iLag]))
				iRetSignal = icBelowCloud;
			else
				if(dY > Math.Max(SpanALine[0+BBase+iLag],SpanBLine[0+BBase+iLag]))
					iRetSignal = icAboveCloud;
			return(iRetSignal);
		}
		


		#endregion
		
		#region Properties
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolShapesAreaBrush", Order=1, GroupName = "NinjaScriptGeneral")]
		public Brush UpAreaBrush
		{
			get { return upAreaBrush; }
			set
			{
				upAreaBrush = value;
				if (upAreaBrush != null)
				{
					if (upAreaBrush.IsFrozen)
						upAreaBrush = upAreaBrush.Clone();
					upAreaBrush.Opacity = iareaOpacity / 100d;
					upAreaBrush.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string UpAreaBrushSerialize
		{
			get { return Serialize.BrushToString(UpAreaBrush); }
			set { UpAreaBrush = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolShapesAreaBrush", Order=2, GroupName = "NinjaScriptGeneral")]
		public Brush DnAreaBrush
		{
			get { return dnAreaBrush; }
			set
			{
				dnAreaBrush = value;
				if (dnAreaBrush != null)
				{
					if (dnAreaBrush.IsFrozen)
						dnAreaBrush = dnAreaBrush.Clone();
					dnAreaBrush.Opacity = iareaOpacity / 100d;
					dnAreaBrush.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string DnAreaBrushSerialize
		{
			get { return Serialize.BrushToString(DnAreaBrush); }
			set { DnAreaBrush = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolAreaOpacity", Order=3, GroupName = "NinjaScriptGeneral")]
		public int iAreaOpacity
		{
			get { return iareaOpacity; }
			set
			{
				iareaOpacity = Math.Max(0, Math.Min(100, value));
				if (upAreaBrush != null)
				{
					upAreaBrush = upAreaBrush.Clone();
					upAreaBrush.Opacity = iareaOpacity / 100d;
					upAreaBrush.Freeze();
				}
				if (dnAreaBrush != null)
				{
					dnAreaBrush = dnAreaBrush.Clone();
					dnAreaBrush.Opacity = iareaOpacity / 100d;
					dnAreaBrush.Freeze();
				}
			}
		}

		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "UpCloudBrush line color", Order=4, GroupName = "NinjaScriptGeneral")]
		public Brush UpLineBrush
		{
			get { return upLineBrush; }
			set { upLineBrush = value; }
		}

		[Browsable(false)]
		public string UpLineBrushSerialize
		{
			get { return Serialize.BrushToString(UpLineBrush); }
			set { UpLineBrush = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "DnCloudBrush line color", Order=5, GroupName = "NinjaScriptGeneral")]
		public Brush DnLineBrush
		{
			get { return dnLineBrush; }
			set { dnLineBrush = value; }
		}

		[Browsable(false)]
		public string DnLineBrushSerialize
		{
			get { return Serialize.BrushToString(DnLineBrush); }
			set { DnLineBrush = Serialize.StringToBrush(value); }
		}
		
		[Range(1, 27)]
		[NinjaScriptProperty]
		[Display(Name="Conversion", Description = "Tenkan line - 9 typical", Order=1, GroupName="Parameters")]
		public int Conversion
		{ get; set; }

		[Range(1, 78)]
		[NinjaScriptProperty]
		[Display(Name="Base", Description = "Kijun line - 26 typical", Order=2, GroupName="Parameters")]
		public int BBase
		{ get; set; }

		[Range(1, 156)]
		[NinjaScriptProperty]
		[Display(Name="SpanB", Description = "Senkou Span B - 52 typical", Order=3, GroupName="Parameters")]
		public int SpanB
		{ get; set; }

		[Range(1, 78)]
		[NinjaScriptProperty]
		[Display(Name="Lag", Description = "Chikou Span - 26 typical", Order=4, GroupName="Parameters")]
		public int Lag
		{ get; set; }
	

		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> ConversionLine
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BaseLine
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SpanALine
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SpanBLine
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> LagLine
		{
			get { return Values[4]; }
		}

		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ApolloIchimoku[] cacheApolloIchimoku;
		public ApolloIchimoku ApolloIchimoku(int conversion, int bBase, int spanB, int lag)
		{
			return ApolloIchimoku(Input, conversion, bBase, spanB, lag);
		}

		public ApolloIchimoku ApolloIchimoku(ISeries<double> input, int conversion, int bBase, int spanB, int lag)
		{
			if (cacheApolloIchimoku != null)
				for (int idx = 0; idx < cacheApolloIchimoku.Length; idx++)
					if (cacheApolloIchimoku[idx] != null && cacheApolloIchimoku[idx].Conversion == conversion && cacheApolloIchimoku[idx].BBase == bBase && cacheApolloIchimoku[idx].SpanB == spanB && cacheApolloIchimoku[idx].Lag == lag && cacheApolloIchimoku[idx].EqualsInput(input))
						return cacheApolloIchimoku[idx];
			return CacheIndicator<ApolloIchimoku>(new ApolloIchimoku(){ Conversion = conversion, BBase = bBase, SpanB = spanB, Lag = lag }, input, ref cacheApolloIchimoku);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ApolloIchimoku ApolloIchimoku(int conversion, int bBase, int spanB, int lag)
		{
			return indicator.ApolloIchimoku(Input, conversion, bBase, spanB, lag);
		}

		public Indicators.ApolloIchimoku ApolloIchimoku(ISeries<double> input , int conversion, int bBase, int spanB, int lag)
		{
			return indicator.ApolloIchimoku(input, conversion, bBase, spanB, lag);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ApolloIchimoku ApolloIchimoku(int conversion, int bBase, int spanB, int lag)
		{
			return indicator.ApolloIchimoku(Input, conversion, bBase, spanB, lag);
		}

		public Indicators.ApolloIchimoku ApolloIchimoku(ISeries<double> input , int conversion, int bBase, int spanB, int lag)
		{
			return indicator.ApolloIchimoku(input, conversion, bBase, spanB, lag);
		}
	}
}

#endregion
