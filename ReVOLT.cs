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
	public class ReVOLT : Indicator
	{

        #region declarations


        private int _period = 10;
        private Series<double> meanVolume;

        #endregion

        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"RVol indicator";
				Name										= "ReVOLT";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				_period = 10;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Period					= 10;
				AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Bar, "RevolNormal");
                AddPlot(new Stroke(Brushes.Orange, 2), PlotStyle.Bar, "RevolHigh");
                AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Bar, "RevolLow");
            }
			else if (State == State.Configure)
			{
            }
            else if (State == State.DataLoaded)
            {
                meanVolume = new Series<double>(this);
                ClearOutputWindow();
                Calculate = Calculate.OnBarClose;
            }
        }

		protected override void OnBarUpdate()
        {
            if (CurrentBar < BarsRequiredToPlot)
                return;

            double totalVolume = 0;
            Print("xxxxx");
            Print("Current Bar:");

            Print(Time[0]);
            Print("Vol:");
            Print(Volumes[0][0]);
            Print("``````");
            int daysToCheck = _period;
            int i = 1;
            while (i <= daysToCheck)
            {
                i++;
                //   CurrentBars[0] -  Bars.GetBar(Time[0]));

                TimeSpan ts = new TimeSpan(Time[0].Hour, Time[0].Minute, Time[0].Second);
                DateTime dateToCheck = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day);
                dateToCheck = dateToCheck.Date.AddDays(-i);
                if (dateToCheck.DayOfWeek != DayOfWeek.Sunday && dateToCheck.DayOfWeek != DayOfWeek.Saturday)
                {
                    dateToCheck = dateToCheck.Date + ts;
                    int barsAgo = CurrentBars[0] - Bars.GetBar(dateToCheck);
                    /*
                    Print("xxxxxxxxxxx");
                    Print(dateToCheck);
                    Print(dateToCheck.DayOfWeek);
                    Print("Bars Ago:");
                    Print(barsAgo);
                    Print("Price:");
                    Print(Closes[0][barsAgo]);
                    Print("Volume:");
                    Print(Volumes[0][barsAgo]);
                    */
                    totalVolume += Volumes[0][barsAgo];
                }
                else
                {
                    daysToCheck++;
                }

            }
            Print("vvvvvvvvvvvvvvvvvvvvvvv");
            Print("Total Volume");
            Print(totalVolume);
            Print("Mean Volume");
            Print(totalVolume / _period);
            meanVolume[0] = totalVolume / _period;
            double rvol = Volume[0] / meanVolume[0];
            Print("Revol");
            Print(rvol);
			if (rvol > AvrMax){
                Values[1][0] = rvol;
			}else if(rvol < AvrMax && rvol > AvrMin)
			{
                Values[0][0] = rvol;
            }
            else // rvol < AvrMin
            {
                Values[2][0] = rvol;
            }
        }

        #region Properties
        [NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Description="Days period for RVol", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }


        [NinjaScriptProperty]
        [Range(1, double.MaxValue)]
        [Display(Name = "High Value", Description = "Value above which rvol is colored as high", Order = 1, GroupName = "Parameters")]
        public double  AvrMax
        { get; set; }

        [NinjaScriptProperty]
        [Range(0.1, double.MaxValue)]
        [Display(Name = "Low Value", Description = "Value above which rvol is colored as Low", Order = 1, GroupName = "Parameters")]
        public double AvrMin
        { get; set; }

        [Browsable(false)]
		[XmlIgnore]
		public Series<double> Revol
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ReVOLT[] cacheReVOLT;
		public ReVOLT ReVOLT(int period, double avrMax, double avrMin)
		{
			return ReVOLT(Input, period, avrMax, avrMin);
		}

		public ReVOLT ReVOLT(ISeries<double> input, int period, double avrMax, double avrMin)
		{
			if (cacheReVOLT != null)
				for (int idx = 0; idx < cacheReVOLT.Length; idx++)
					if (cacheReVOLT[idx] != null && cacheReVOLT[idx].Period == period && cacheReVOLT[idx].AvrMax == avrMax && cacheReVOLT[idx].AvrMin == avrMin && cacheReVOLT[idx].EqualsInput(input))
						return cacheReVOLT[idx];
			return CacheIndicator<ReVOLT>(new ReVOLT(){ Period = period, AvrMax = avrMax, AvrMin = avrMin }, input, ref cacheReVOLT);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ReVOLT ReVOLT(int period, double avrMax, double avrMin)
		{
			return indicator.ReVOLT(Input, period, avrMax, avrMin);
		}

		public Indicators.ReVOLT ReVOLT(ISeries<double> input , int period, double avrMax, double avrMin)
		{
			return indicator.ReVOLT(input, period, avrMax, avrMin);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ReVOLT ReVOLT(int period, double avrMax, double avrMin)
		{
			return indicator.ReVOLT(Input, period, avrMax, avrMin);
		}

		public Indicators.ReVOLT ReVOLT(ISeries<double> input , int period, double avrMax, double avrMin)
		{
			return indicator.ReVOLT(input, period, avrMax, avrMin);
		}
	}
}

#endregion
