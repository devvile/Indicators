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
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class WVF : Indicator
	{
		
		#region Variables
        // Wizard generated variables
            private int wVF_Period = 22; // Default setting for WVF_Period
            private int stochD_Period = 14; // Default setting for StochD_Period
			private Series<double>		CurVar_DS;
		    private Series<double>		CurVar_StochK;
        // User defined variables (add any user defined variables below)
        #endregion
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Willium Vix Fix -- Of tuple";
				Name										= "WVF";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
				
				AddPlot(Brushes.Orange, "WVFplot0");
				AddPlot(new Stroke(Brushes.Green), PlotStyle.Bar, "StockDplot");
				
				CurVar_DS = new Series<double>(this, MaximumBarsLookBack.Infinite);
				CurVar_StochK = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			
			// Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			//CurRSI_DS.Set(RSI(periodRSI,0)[0]);
			
			double LLV = MIN(Close, wVF_Period)[0];
			double HHV = MAX(Close, wVF_Period)[0];
			//WVF = (HHV (Close,22) - Low)/(HHV(Close,22))*100;
			CurVar_DS[0] = (100*((HHV-Low[0])/HHV));
			
            WVFplot0[0] = (CurVar_DS[0]);
			//StoK.Set(SMA(StoRSI_DS, periodK)[0]);
			//CurVar_StochK.Set(Stochastics(CurVar_DS,stochD_Period,2).K[0]);
			//CurVar_StochK.Set(Stochastics(CurVar_DS,stochD_Period,2).D[0]);
            //StockDplot.Set(CurVar_StochK[0]);
			
		}
		
		#region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> WVFplot0
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> StockDplot
        {
            get { return Values[1]; }
        }

        [Description("WVF works with two value 22 and 26")]
        [Category("Parameters")]
        public int WVF_Period
        {
            get { return wVF_Period; }
            set { wVF_Period = Math.Max(22, value); }
        }

        [Description("")]
        [Category("Parameters")]
        public int StochD_Period
        {
            get { return stochD_Period; }
            set { stochD_Period = Math.Max(1, value); }
        }
        #endregion
		
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private WVF[] cacheWVF;
		public WVF WVF()
		{
			return WVF(Input);
		}

		public WVF WVF(ISeries<double> input)
		{
			if (cacheWVF != null)
				for (int idx = 0; idx < cacheWVF.Length; idx++)
					if (cacheWVF[idx] != null &&  cacheWVF[idx].EqualsInput(input))
						return cacheWVF[idx];
			return CacheIndicator<WVF>(new WVF(), input, ref cacheWVF);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.WVF WVF()
		{
			return indicator.WVF(Input);
		}

		public Indicators.WVF WVF(ISeries<double> input )
		{
			return indicator.WVF(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.WVF WVF()
		{
			return indicator.WVF(Input);
		}

		public Indicators.WVF WVF(ISeries<double> input )
		{
			return indicator.WVF(input);
		}
	}
}

#endregion
