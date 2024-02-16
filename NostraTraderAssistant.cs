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
	public class NostraTraderAssistant : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Nostra TradeMate";
				Name										= "NostraTraderAssistant";
				Calculate									= Calculate.OnEachTick;
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
        //        background = Brushes.DarkSlateGray;
            }
		}

		protected override void OnBarUpdate()
		{
         //Draw.TextFixed(this, "NinjaScriptInfo",
			 /*
        "Today Globex High: " + todayGlobexHigh.ToString() + "\nToday Globex Low: " + todayGlobexLow.ToString()
        + "\nIB High: " + IBHigh.ToString() + "\nIB Low: " + IBLow.ToString()
        + "\nToday RTH High: " + todayRTHHigh.ToString() + "\nToday RTH Low: " + todayRTHLow.ToString()
        + "\n ~~~~~~~~~~"
        + "\nYestertday Globex High: " + yesterdayGlobexHigh.ToString() + "\nTodayGlobex Low: " + yesterdayGlobexLow.ToString()
        + "\nYesterday RTH High: " + yesterdayRTHHigh.ToString() + "\nYesterday RTH Low: " + yesterdayRTHLow.ToString()
			 */
   //     ,
			 /*
        TextPosition.TopLeft,
        ChartControl.Properties.ChartText,
        ChartControl.Properties.LabelFont,
        Brushes.Gray, background, 100);*/
        }
	}


}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private NostraTraderAssistant[] cacheNostraTraderAssistant;
		public NostraTraderAssistant NostraTraderAssistant()
		{
			return NostraTraderAssistant(Input);
		}

		public NostraTraderAssistant NostraTraderAssistant(ISeries<double> input)
		{
			if (cacheNostraTraderAssistant != null)
				for (int idx = 0; idx < cacheNostraTraderAssistant.Length; idx++)
					if (cacheNostraTraderAssistant[idx] != null &&  cacheNostraTraderAssistant[idx].EqualsInput(input))
						return cacheNostraTraderAssistant[idx];
			return CacheIndicator<NostraTraderAssistant>(new NostraTraderAssistant(), input, ref cacheNostraTraderAssistant);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.NostraTraderAssistant NostraTraderAssistant()
		{
			return indicator.NostraTraderAssistant(Input);
		}

		public Indicators.NostraTraderAssistant NostraTraderAssistant(ISeries<double> input )
		{
			return indicator.NostraTraderAssistant(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.NostraTraderAssistant NostraTraderAssistant()
		{
			return indicator.NostraTraderAssistant(Input);
		}

		public Indicators.NostraTraderAssistant NostraTraderAssistant(ISeries<double> input )
		{
			return indicator.NostraTraderAssistant(input);
		}
	}
}

#endregion
