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
    public class LevelsSingleFrame: Indicator
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
        int _barsRequiredToPlot;
        int i = 1;
        private Series<double> diffSeries;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Displays globex high, globex low, IB high, IB low, RTH high, RTH low";
                Name = "Levels Single Timeframe";
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
                _barsRequiredToPlot = 20;

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

                AddPlot(new Stroke(Brushes.Red, 1), PlotStyle.Dot, "lwHigh");  
                AddPlot(new Stroke(Brushes.Red, 1), PlotStyle.Dot, "lwLow");
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "twHigh");
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "twLow");

            }
            else if (State == State.Configure)
            {
                background = Brushes.DarkSlateGray;


            }else if(State == State.DataLoaded)
            {
                diffSeries = new Series<double>(this);
                ClearOutputWindow();
            }
        }
        protected override void OnBarUpdate()
        {

            if (CurrentBar <= BarsRequiredToPlotVal + 1200)
                return;

            if (CurrentBar == 0)
            {
                Value[0] = Input[0];
            }


            if (Bars.IsFirstBarOfSession)
            {
                Print("1 miute bars");
                if (thisWeekHigh == 0)
                {
                    thisWeekHigh = High[0];
                }
                if (thisWeekLow == 0)
                {
                    thisWeekLow = Low[0];
                }

            }


                /*
                if (High[0]> thisWeekHigh)
                {
                    thisWeekHigh = High[0];
                };

                if (Low[0] < thisWeekLow)
                {
                    thisWeekLow = Low[0];
                };
                */
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

            /*

            if (yesterdayGlobexHigh != 0 && yesterdayGlobexLow !=0)
             {
                    Values[2][0] = yesterdayGlobexHigh;
                    Values[3][0] = yesterdayGlobexLow;
             }


            if (yesterdayRTHHigh != 0 && yesterdayRTHLow != 0)
             {
                    Values[6][0] = yesterdayRTHHigh;
                    Values[7][0] = yesterdayRTHLow;
             }
                if (thisWeekHigh!= 0 && thisWeekLow !=0)
                {
                    Values[12][0] = thisWeekHigh;
                    Values[13][0] = thisWeekLow;
                }

                /*
            if (CurrentBars[2] > 1 && lastWeekHigh!= 0 && lastWeekLow != 0 )
                {
                  Values[10][0] = lastWeekHigh;
                  Values[11][0] = lastWeekLow;
            }

            if (ToTime(Time[0]) >= rthStartTime && ToTime(Time[0]) <= 235900) //time >= rthStartTime && time <= rthEndTime
            {
                Values[4][0] = todayRTHHigh;
                Values[5][0] = todayRTHLow;
                Values[8][0] = IBHigh;
                Values[9][0] = IBLow;
            }
        
            /*
            if (BarsInProgress == 2 && CurrentBars[2]>=1) //16
            {
                lastWeekHigh = Highs[2][0];
                lastWeekLow  = Lows[2][0];
                thisWeekHigh = Closes[2][0];
                thisWeekLow = Closes[2][0];
            }*/


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

        /*
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
        */
        [Browsable(false)]
        [Range(1, 125), NinjaScriptProperty]
        [Display(Name = "BarsToPlotNinjascript", GroupName = "Position Management", Order = 0)]
        public int BarsRequiredToPlotVal
        {
            get { return _barsRequiredToPlot; }
            set { _barsRequiredToPlot = value; }
        }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private LevelsSingleFrame[] cacheLevelsSingleFrame;
		public LevelsSingleFrame LevelsSingleFrame(int barsRequiredToPlotVal)
		{
			return LevelsSingleFrame(Input, barsRequiredToPlotVal);
		}

		public LevelsSingleFrame LevelsSingleFrame(ISeries<double> input, int barsRequiredToPlotVal)
		{
			if (cacheLevelsSingleFrame != null)
				for (int idx = 0; idx < cacheLevelsSingleFrame.Length; idx++)
					if (cacheLevelsSingleFrame[idx] != null && cacheLevelsSingleFrame[idx].BarsRequiredToPlotVal == barsRequiredToPlotVal && cacheLevelsSingleFrame[idx].EqualsInput(input))
						return cacheLevelsSingleFrame[idx];
			return CacheIndicator<LevelsSingleFrame>(new LevelsSingleFrame(){ BarsRequiredToPlotVal = barsRequiredToPlotVal }, input, ref cacheLevelsSingleFrame);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.LevelsSingleFrame LevelsSingleFrame(int barsRequiredToPlotVal)
		{
			return indicator.LevelsSingleFrame(Input, barsRequiredToPlotVal);
		}

		public Indicators.LevelsSingleFrame LevelsSingleFrame(ISeries<double> input , int barsRequiredToPlotVal)
		{
			return indicator.LevelsSingleFrame(input, barsRequiredToPlotVal);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.LevelsSingleFrame LevelsSingleFrame(int barsRequiredToPlotVal)
		{
			return indicator.LevelsSingleFrame(Input, barsRequiredToPlotVal);
		}

		public Indicators.LevelsSingleFrame LevelsSingleFrame(ISeries<double> input , int barsRequiredToPlotVal)
		{
			return indicator.LevelsSingleFrame(input, barsRequiredToPlotVal);
		}
	}
}

#endregion
