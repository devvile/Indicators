using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System;
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

// Declare your indicator class
namespace NinjaTrader.NinjaScript.Indicators
{
    public class RVOL : Indicator
    {
        private Series<double> meanVolume;
        private int period = 22;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Relative Volume Indicator - compares current volume to the average volume of the same time period over 22 days.";
                Name = "RVOL";
                Calculate = Calculate.OnEachTick; // or Calculate.OnBarClose;
                IsOverlay = false;
                AddPlot(Brushes.Orange, "RVOL");
            }
            else if (State == State.DataLoaded)
            {
                // Initialize mean volume series
                meanVolume = new Series<double>(this);
            }
        }

        protected override void OnBarUpdate()
        {
            // Ensure we have enough data
            if (CurrentBar < period)
                return;

            // Calculate mean volume for the same bar over the past 22 days
            double totalVolume = 0;
            var today = Time[0];
            var dateTime = new DateTime(today.Year, today.Month, today.Day, 9, 30, 0);
            for (int i = 0; i < period; i++)
            {
                int barsAgo = Bars.GetBar(new DateTime(today.Year, today.Month, today.Day - i, 9, 30, 0));
                Print(i);
                Print(Close[barsAgo]);
				
            }
            meanVolume[0] = totalVolume / period;

            // Calculate RVOL
            double rvol = Volume[0] / meanVolume[0];
            Values[0][0] = rvol;
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private RVOL[] cacheRVOL;
		public RVOL RVOL()
		{
			return RVOL(Input);
		}

		public RVOL RVOL(ISeries<double> input)
		{
			if (cacheRVOL != null)
				for (int idx = 0; idx < cacheRVOL.Length; idx++)
					if (cacheRVOL[idx] != null &&  cacheRVOL[idx].EqualsInput(input))
						return cacheRVOL[idx];
			return CacheIndicator<RVOL>(new RVOL(), input, ref cacheRVOL);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.RVOL RVOL()
		{
			return indicator.RVOL(Input);
		}

		public Indicators.RVOL RVOL(ISeries<double> input )
		{
			return indicator.RVOL(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.RVOL RVOL()
		{
			return indicator.RVOL(Input);
		}

		public Indicators.RVOL RVOL(ISeries<double> input )
		{
			return indicator.RVOL(input);
		}
	}
}

#endregion
