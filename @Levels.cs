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
using NinjaTrader.NinjaScript.Indicators;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
    public class Levels : Indicator
    {
        double todayGlobexLow = 0;
        double todayGlobexHigh = 0;
        double yesterdayGlobexLow = 0;
        double yesterdayGlobexHigh = 0;
        int globexStartTime = 100;
        double todayRTHLow = 0;
        double todayRTHHigh = 0;
        double yesterdayRTHLow = 0;
        double yesterdayRTHHigh = 0;
        int rthStartTime = 153000;
        int rthEndTime = 220000;
        double lastWeekHigh = 0;
        double lastWeekLow = 0;

        double IBLow = 0;
        double IBHigh = 0;
        int IbEndTime = 163000;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Displays globex high, globex low, IB high, IB low, RTH high, RTH low";
                Name = "Levels";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                //Disable this property if your indicator requires custom values that cumulate with each new market data event. 
                //See Help Guide for additional information.
                IsSuspendedWhileInactive = true;


                AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "globexHigh");     
                AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "globexLow");     
                AddPlot(new Stroke(Brushes.Blue, 1), PlotStyle.Dot, "yglobexHigh");   
                AddPlot(new Stroke(Brushes.Blue, 1), PlotStyle.Dot, "yglobexLow");   
                AddPlot(new Stroke(Brushes.Yellow, 2), PlotStyle.Line, "rthHigh");   
                AddPlot(new Stroke(Brushes.Yellow, 2), PlotStyle.Line, "rthLow"); 
                AddPlot(new Stroke(Brushes.Yellow, 1), PlotStyle.Dot, "yRTHHigh");   
                AddPlot(new Stroke(Brushes.Yellow, 1), PlotStyle.Dot, "yRTHLow");   


                AddPlot(new Stroke(Brushes.White, 2), PlotStyle.Line, "ibHigh");  
                AddPlot(new Stroke(Brushes.White, 2), PlotStyle.Line, "ibLow");  

                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "lwHigh");  
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "lwLow");   

            }
            else if (State == State.Configure)
            {
                background = Brushes.DarkSlateGray;
                AddDataSeries(BarsPeriodType.Minute, 1);
                AddDataSeries(BarsPeriodType.Week, 1);
            }else if(State == State.DataLoaded)
            {
                ClearOutputWindow();
            }
        }
        protected override void OnBarUpdate()
        {

            if (CurrentBars[0] < BarsRequiredToPlot || CurrentBars[1] < BarsRequiredToPlot || CurrentBars[2] < 1)
                return;
            if (BarsInProgress == 1)
            {
                Print("Progresss 1");
                Print(Time[0]);
                if (ToTime(Time[0]) == globexStartTime)
            {
                yesterdayGlobexLow = todayGlobexLow;
                yesterdayGlobexHigh = todayGlobexHigh;
                yesterdayRTHLow = todayRTHLow;
                yesterdayRTHHigh = todayRTHHigh;
                todayGlobexLow = Close[0];
                todayGlobexHigh = Close[0];
            }

            else if (ToTime(Time[0]) == rthStartTime)
            {

                todayRTHLow = Close[0];
                todayRTHHigh = Close[0];
                IBLow = Close[0];
                IBHigh = Close[0];
            }

            if (isGlobex(ToTime(Time[0])))
            {
                if (High[0] > todayGlobexHigh)
                {
                    todayGlobexHigh = High[0];
                }
                else if (Low[0] < todayGlobexLow)
                {
                    todayGlobexLow = Low[0];
                }
            }

            if (isRTH(ToTime(Time[0])))
            {
                if (High[0] > todayRTHHigh)
                {
                    todayRTHHigh = High[0];
                }
                else if (Low[0] < todayRTHLow)
                {
                    todayRTHLow = Low[0];
                }
            }


            if (isIB(ToTime(Time[0])))
            {
                if (High[0] > IBHigh)
                {
                    IBHigh = High[0];
                }
                else if (Low[0] < IBLow)
                {
                    IBLow = Low[0];
                }
            }


            Draw.TextFixed(this, "NinjaScriptInfo",
            "Today Globex High: " + todayGlobexHigh.ToString() + "\nToday Globex Low: " + todayGlobexLow.ToString()
            + "\nIB High: " + IBHigh.ToString() + "\nIB Low: " + IBLow.ToString()
            + "\nToday RTH High: " + todayRTHHigh.ToString() + "\nToday RTH Low: " + todayRTHLow.ToString()
            + "\n ~~~~~~~~~~"
            + "\nYestertday Globex High: " + yesterdayGlobexHigh.ToString() + "\nTodayGlobex Low: " + yesterdayGlobexLow.ToString()
            + "\nYesterday RTH High: " + yesterdayRTHHigh.ToString() + "\nYesterday RTH Low: " + yesterdayRTHLow.ToString()

            ,
            TextPosition.TopLeft,
            ChartControl.Properties.ChartText,
            ChartControl.Properties.LabelFont,
            Brushes.Gray, background, 100);




            Values[0][0] = todayGlobexHigh;
            Values[1][0] = todayGlobexLow;
            Values[2][0] = yesterdayGlobexHigh;
            Values[3][0] = yesterdayGlobexLow;

            Values[6][0] = yesterdayRTHHigh;
            Values[7][0] = yesterdayRTHLow;
            Values[10][0] = lastWeekHigh;
            Values[11][0] = lastWeekLow;

            if (ToTime(Time[0]) >= rthStartTime && ToTime(Time[0]) <= 235900) //time >= rthStartTime && time <= rthEndTime
            {
                Values[4][0] = todayRTHHigh;
                Values[5][0] = todayRTHLow;
                Values[8][0] = IBHigh;
                Values[9][0] = IBLow;
            }
        }
            if (BarsInProgress == 2) //16
            {
                Print("Progresss 2");
                Print(Time[0]);
                lastWeekHigh = Highs[2][0];
                lastWeekLow  = Lows[2][0];
            }




                /*
                if (todayGlobexHigh > todayRTHHigh && isRTH((ToTime(Time[0]))))
                {
                    Values[0][0] = todayGlobexHigh;
                }else if (isGlobex((ToTime(Time[0]))))
                {
                    Values[0][0] = todayGlobexHigh;
                }

                if (todayGlobexLow < todayRTHLow && isRTH((ToTime(Time[0]))))
                {
                    Values[1][0] = todayGlobexLow;
                }
                else if (isGlobex((ToTime(Time[0]))))
                {
                    Values[1][0] = todayGlobexLow;
                }


                if (isRTH((ToTime(Time[0]))))
                {
                    Values[2][0] = todayRTHHigh;
                    Values[3][0] = todayRTHLow;
                    Values[4][0] = IBHigh;
                    Values[5][0] = IBLow;
                } */

            }

        private bool isGlobex(int time)
        {
            if (time >= globexStartTime && time <= rthStartTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isRTH(int time)
        {
            if (time >= rthStartTime && time <= rthEndTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool isIB(int time)
        {
            if (time >= rthStartTime && time <= IbEndTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Properties

        [XmlIgnore]
        [Display(Name = "table background", Order = 3, GroupName = "Parameters")]
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
		private Levels[] cacheLevels;
		public Levels Levels()
		{
			return Levels(Input);
		}

		public Levels Levels(ISeries<double> input)
		{
			if (cacheLevels != null)
				for (int idx = 0; idx < cacheLevels.Length; idx++)
					if (cacheLevels[idx] != null &&  cacheLevels[idx].EqualsInput(input))
						return cacheLevels[idx];
			return CacheIndicator<Levels>(new Levels(), input, ref cacheLevels);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Levels Levels()
		{
			return indicator.Levels(Input);
		}

		public Indicators.Levels Levels(ISeries<double> input )
		{
			return indicator.Levels(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Levels Levels()
		{
			return indicator.Levels(Input);
		}

		public Indicators.Levels Levels(ISeries<double> input )
		{
			return indicator.Levels(input);
		}
	}
}

#endregion
