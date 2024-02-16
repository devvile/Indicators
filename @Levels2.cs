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
    public class Levels2 : Indicator
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
        double thisWeekHigh = 0;
        double thisWeekLow = 0;

        double IBLow = 0;
        double IBHigh = 0;
        int IbEndTime = 163000;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Displays globex high, globex low, IB high, IB low, RTH high, RTH low";
                Name = "Levels2";
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
                BarsRequiredToPlot = 20;
                showRTHYesterday = true;
                showGlobexToday = true;
                showGlobexYesterday = true;
                showIb = true;
                showRTHToday = true;
                showThisWeek = true;
                showLastWeek = true;


                AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "globexHigh");     
                AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "globexLow");     
                AddPlot(new Stroke(Brushes.Blue, 1), PlotStyle.Dot, "yglobexHigh");   
                AddPlot(new Stroke(Brushes.Blue, 1), PlotStyle.Dot, "yglobexLow");   
                AddPlot(new Stroke(Brushes.Yellow, 2), PlotStyle.Line, "rthHigh");   //4
                AddPlot(new Stroke(Brushes.Yellow, 2), PlotStyle.Line, "rthLow");  //5
                AddPlot(new Stroke(Brushes.Yellow, 1), PlotStyle.Dot, "yRTHHigh");   //6
                AddPlot(new Stroke(Brushes.Yellow, 1), PlotStyle.Dot, "yRTHLow");   //7


                AddPlot(new Stroke(Brushes.White, 2), PlotStyle.Line, "ibHigh");  
                AddPlot(new Stroke(Brushes.White, 2), PlotStyle.Line, "ibLow");  

                AddPlot(new Stroke(Brushes.Red, 1), PlotStyle.Dot, "lwHigh");  //10
                AddPlot(new Stroke(Brushes.Red, 1), PlotStyle.Dot, "lwLow"); //11
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "twHigh"); //12
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "twLow"); //values[13]

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

            if (CurrentBars[0] < BarsRequiredToPlot || CurrentBars[1] < BarsRequiredToPlot)
                return;



            if (BarsInProgress == 1)
            {
                if (thisWeekHigh == 0)
                {
                    thisWeekHigh = High[0];
                }
                else if (High[0]> thisWeekHigh)
                {
                    thisWeekHigh = High[0];
                };

                if (Low[0] < thisWeekLow)
                {
                    thisWeekLow = Low[0];
                }
                else if (thisWeekLow == 0)
                {
                    thisWeekLow = Low[0];
                };

                if (ToTime(Time[0]) == globexStartTime)
            {
                yesterdayGlobexLow = todayGlobexLow;
                yesterdayGlobexHigh = todayGlobexHigh;
                yesterdayRTHLow = todayRTHLow;
                yesterdayRTHHigh = todayRTHHigh;
                todayGlobexLow = Low[0];
                todayGlobexHigh = High[0];
            }

            else if (ToTime(Time[0]) == rthStartTime)
            {

                todayRTHLow = Low[0];
                todayRTHHigh = High[0];
                IBLow = Low[0];
                IBHigh = High[0];
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




            if (showGlobexToday)
            {
                Values[0][0] = todayGlobexHigh;
                Values[1][0] = todayGlobexLow;
            }


            if (yesterdayGlobexHigh != 0 && yesterdayGlobexLow !=0 && showGlobexYesterday)
             {
                    Values[2][0] = yesterdayGlobexHigh;
                    Values[3][0] = yesterdayGlobexLow;
             }


            if (yesterdayRTHHigh != 0 && yesterdayRTHLow != 0 && showRTHYesterday)
             {
                    Values[6][0] = yesterdayRTHHigh;
                    Values[7][0] = yesterdayRTHLow;
             }
                if (thisWeekHigh!= 0 && thisWeekLow !=0 && showThisWeek)
                {
                    Values[12][0] = thisWeekHigh;
                    Values[13][0] = thisWeekLow;
                }


            if (CurrentBars[2] > 1 && lastWeekHigh!= 0 && lastWeekLow != 0 && showLastWeek)
                {
                  Values[10][0] = lastWeekHigh;
                  Values[11][0] = lastWeekLow;
            }

            if (ToTime(Time[0]) >= rthStartTime && ToTime(Time[0]) <= 235900) //time >= rthStartTime && time <= rthEndTime
            {
                    if (showRTHToday)
                    {
                        Values[4][0] = todayRTHHigh;
                        Values[5][0] = todayRTHLow;
                    }
                    if (showIb)
                    {
                        Values[8][0] = IBHigh;
                        Values[9][0] = IBLow;
                    }
            }
        }
            if (BarsInProgress == 2 && CurrentBars[2]>=1) //16
            {
                lastWeekHigh = Highs[2][0];
                lastWeekLow  = Lows[2][0];
                thisWeekHigh = Highs[2][0];
                thisWeekLow = Lows[2][0];
            }


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
        [Display(Name = "Show Last Week High/Low", Order = 0, GroupName = "Parameters")]
        public bool showLastWeek
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show This Week High/Low", Order = 1, GroupName = "Parameters")]
        public bool showThisWeek
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show Globex Current Day", Order = 2, GroupName = "Parameters")]
        public bool showGlobexToday
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show Globex Day Before", Order = 3, GroupName = "Parameters")]
        public bool showGlobexYesterday
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show RTH Current Day", Order = 4, GroupName = "Parameters")]
        public bool showRTHToday
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show RTH Day Before", Order = 5, GroupName = "Parameters")]
        public bool showRTHYesterday
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Show IB", Order = 6, GroupName = "Parameters")]
        public bool showIb
        { get; set; }

        [XmlIgnore]
        [Display(Name = "table background", Order = 7, GroupName = "Parameters")]
        public Brush background
        { get; set; }


        [Browsable(false)]
        public string backgroundSerializable
        {
            get { return Serialize.BrushToString(background); }
            set { background = Serialize.StringToBrush(value); }
        }

        /*
         * 
            Values[0][0] = todayGlobexHigh;
            Values[1][0] = todayGlobexLow;
            Values[2][0] = yesterdayGlobexHigh;
            Values[3][0] = yesterdayGlobexLow;
            Values[4][0] = todayRTHHigh;
            Values[5][0] = todayRTHLow;
            Values[6][0] = yesterdayRTHHigh;
            Values[7][0] = yesterdayRTHLow;
            Values[8][0] = IBHigh;
            Values[9][0] = IBLow;

        */

        [Browsable(false)]
        public Series<double> todayGlobexHighs
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        public Series<double> todayGlobexLows
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        public Series<double> yesterdayGlobexHighs
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        public Series<double> yesterdayGlobexLows
        {
            get { return Values[3]; }
        }

        [Browsable(false)]
        public Series<double> todayRTHHighs
        {
            get { return Values[4]; }
        }

        [Browsable(false)]
        public Series<double> todayRTHLows
        {
            get { return Values[5]; }
        }

        [Browsable(false)]
        public Series<double> yesterdayRTHHighs
        {
            get { return Values[6]; }
        }

        [Browsable(false)]
        public Series<double> yesterdayRTHLows
        {
            get { return Values[7]; }
        }

        [Browsable(false)]
        public Series<double> IBHighs
        {
            get { return Values[8]; }
        }

        [Browsable(false)]
        public Series<double> IBLows
        {
            get { return Values[9]; }
        }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Levels2[] cacheLevels2;
		public Levels2 Levels2()
		{
			return Levels2(Input);
		}

		public Levels2 Levels2(ISeries<double> input)
		{
			if (cacheLevels2 != null)
				for (int idx = 0; idx < cacheLevels2.Length; idx++)
					if (cacheLevels2[idx] != null &&  cacheLevels2[idx].EqualsInput(input))
						return cacheLevels2[idx];
			return CacheIndicator<Levels2>(new Levels2(), input, ref cacheLevels2);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Levels2 Levels2()
		{
			return indicator.Levels2(Input);
		}

		public Indicators.Levels2 Levels2(ISeries<double> input )
		{
			return indicator.Levels2(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Levels2 Levels2()
		{
			return indicator.Levels2(Input);
		}

		public Indicators.Levels2 Levels2(ISeries<double> input )
		{
			return indicator.Levels2(input);
		}
	}
}

#endregion
