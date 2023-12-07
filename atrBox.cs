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
	public class ATRBox : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Displays the current ATR in the top right section of chart.";
				Name										= "ATRBox";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Period										= 14;
				atrThreshold								= 3.5;
				adxThreshold								= 25;
				background									= Brushes.DarkSlateGray;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{

            if (CurrentBar < 0)
                return;


			double todayGlobexLow = 0;
			//Add your custom indicator logic here.
			double todayGlobexHigh = 0;


			Print(Close[0]);
			Print(Time[0]);

			
			if (0 >= atrThreshold && 0 >= adxThreshold)
				background = Brushes.Green;
			else
				background = Brushes.DarkSlateGray;
			
			Draw.TextFixed(this, "NinjaScriptInfo",
            "Today Globex High: " + todayGlobexHigh.ToString() + "\nToday Globex Low: " + todayGlobexLow.ToString(),
			TextPosition.TopRight, 
			ChartControl.Properties.ChartText, 
			ChartControl.Properties.LabelFont, 
			Brushes.Gray, background, 100);
			
		}
		
		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }
		
		[Range(1, double.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "atrThreshold", GroupName = "NinjaScriptParameters", Order = 1)]
		public double atrThreshold
		{ get; set; }
		
		[Range(1, double.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "adxThreshold", GroupName = "NinjaScriptParameters", Order = 2)]
		public double adxThreshold
		{ get; set; }
		
		[XmlIgnore]
		[Display(Name="background", Order=3, GroupName="Parameters")]
		public Brush background
		{ get; set; }

		[Browsable(false)]
		public string backgroundSerializable
		{
			get { return Serialize.BrushToString(background); }
			set { background = Serialize.StringToBrush(value); }
		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ATRBox[] cacheATRBox;
		public ATRBox ATRBox(int period, double atrThreshold, double adxThreshold)
		{
			return ATRBox(Input, period, atrThreshold, adxThreshold);
		}

		public ATRBox ATRBox(ISeries<double> input, int period, double atrThreshold, double adxThreshold)
		{
			if (cacheATRBox != null)
				for (int idx = 0; idx < cacheATRBox.Length; idx++)
					if (cacheATRBox[idx] != null && cacheATRBox[idx].Period == period && cacheATRBox[idx].atrThreshold == atrThreshold && cacheATRBox[idx].adxThreshold == adxThreshold && cacheATRBox[idx].EqualsInput(input))
						return cacheATRBox[idx];
			return CacheIndicator<ATRBox>(new ATRBox(){ Period = period, atrThreshold = atrThreshold, adxThreshold = adxThreshold }, input, ref cacheATRBox);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ATRBox ATRBox(int period, double atrThreshold, double adxThreshold)
		{
			return indicator.ATRBox(Input, period, atrThreshold, adxThreshold);
		}

		public Indicators.ATRBox ATRBox(ISeries<double> input , int period, double atrThreshold, double adxThreshold)
		{
			return indicator.ATRBox(input, period, atrThreshold, adxThreshold);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ATRBox ATRBox(int period, double atrThreshold, double adxThreshold)
		{
			return indicator.ATRBox(Input, period, atrThreshold, adxThreshold);
		}

		public Indicators.ATRBox ATRBox(ISeries<double> input , int period, double atrThreshold, double adxThreshold)
		{
			return indicator.ATRBox(input, period, atrThreshold, adxThreshold);
		}
	}
}

#endregion
