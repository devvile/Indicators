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
using NinjaTrader.NinjaScript.Indicators;

// Declare your indicator class
namespace NinjaTrader.NinjaScript.Indicators
{
    public class ReVOL : Indicator
    {
        private Series<double> meanVolume;
        private int period = 22;
        private int minutesInDay = 1440;
        private int interval = 1;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Relative Volume Indicator - compares current volume to the average volume of the same time period over 22 days.";
                Name = "ReVOL";
                Calculate =  Calculate.OnBarClose;
                IsOverlay = false;
                AddPlot(Brushes.Orange, "RVOL");
            }
            else if (State == State.Configure)
            {
                // Initialize mean volume series
                ClearOutputWindow();
         //       AddDataSeries(BarsPeriodType.Day, 1);
                //  meanVolume = new Series<double>(this);
            }
            else if (State == State.DataLoaded)
            {
                // Initialize mean volume series
                ClearOutputWindow();
                meanVolume = new Series<double>(this);
            }
        }

        protected override void OnBarUpdate()
        {
            // Ensure we have enough data
            if (CurrentBars[0] < minutesInDay || CurrentBars[1] <= period+1)
                return;

            // Calculate mean volume for the same bar over the past 22 days
            if (BarsInProgress == 0)
            {
                Print("``````````````````");
                double totalVolume = 0;
                var today = Time[0];
                TimeSpan specificTime = new TimeSpan(9, 0, 0); // 9:00 AM

                for (int i = 1; i < 20; i++)
                {

               //   CurrentBars[0] -  Bars.GetBar(Time[0]));
                      Print("xxxxx");
                      Print(Time[0]);
                      DateTime dateToCheck = Time[0].Date.AddDays(-i);
                      Print(dateToCheck.ToString());
                    //        int barsAgo =  CurrentBars[0] - Bars.GetBar(Time[minutesInDay * i]);
                    //     Print(CurrentBars[0]);
                    //     Print(i);
                    //     Print(new DateTime(today.Year, month, today.Day - i, 9, 30, 0));
                    //        Print(barsAgo);
                    //      Print(Closes[0][barsAgo]);

                }
        //        meanVolume[0] = totalVolume / period;

                // Calculate RVOL
      //          double rvol = Volume[0] / meanVolume[0];
         //       Values[0][0] = rvol;
            }
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ReVOL[] cacheReVOL;
		public ReVOL ReVOL()
		{
			return ReVOL(Input);
		}

		public ReVOL ReVOL(ISeries<double> input)
		{
			if (cacheReVOL != null)
				for (int idx = 0; idx < cacheReVOL.Length; idx++)
					if (cacheReVOL[idx] != null &&  cacheReVOL[idx].EqualsInput(input))
						return cacheReVOL[idx];
			return CacheIndicator<ReVOL>(new ReVOL(), input, ref cacheReVOL);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ReVOL ReVOL()
		{
			return indicator.ReVOL(Input);
		}

		public Indicators.ReVOL ReVOL(ISeries<double> input )
		{
			return indicator.ReVOL(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ReVOL ReVOL()
		{
			return indicator.ReVOL(Input);
		}

		public Indicators.ReVOL ReVOL(ISeries<double> input )
		{
			return indicator.ReVOL(input);
		}
	}
}

#endregion
