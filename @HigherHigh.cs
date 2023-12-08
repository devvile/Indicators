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
    public class HigherHigh : Indicator
    {
        List<Price> PricesList = new List<Price>();
        double lastLowPrice = 0;
        double lastHighPrice = 0;
        double lowerPrice = 0;
        double higherPrice = 0;
        double pullbackThreshold= 20;
        double minMovement = 40;

        Random rnd = new Random();
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Determines Higher highs and Higher Lows and the opposite";
                Name = "Higher Highs";
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
                background = Brushes.DarkSlateGray;

                AddPlot(Brushes.Yellow, "higherHigh");     // Defines the plot for Values[0]
                AddPlot(Brushes.Red, "lowerLow");     // Defines the plot for Values[1]
            }
            else if (State == State.Configure)
            {
            }
            else if (State == State.DataLoaded)
            {
                ClearOutputWindow();
            }

        }
        protected override void OnBarUpdate()
        {

            if (CurrentBar < 0)
                return;

            if (Bars.IsFirstBarOfSession)
            {
                lastLowPrice = Close[0];
                lastHighPrice = Close[0];
                lowerPrice = Close[0];
                higherPrice = Close[0];
            }

            if (Close[0] > higherPrice)
            {
                higherPrice = Close[0];
            } else if (Close[0] < lowerPrice)
            {
                lowerPrice = Close[0];
            }


            if (isNewLow(Close[0]))
            {
                Values[1][0] = lastLowPrice;
                //        Mark("low");
            }
                 else  if (isNewHigh(Close[0]))
            {
                Values[0][0] = lastHighPrice;
                //          Mark("high");
            }


            // cena 

            // Values[0][0] = wo;
            // Values[1][0] = lo;

            //pierwszy jest punkt pomiaru lastLow - potem najwyzsza cena od tego
            //potem sledzimy czy cena zamkniecia jest  

            // bierzemy na przestrzeni dwoch godzin wszystkie ceny


        }

        
        private bool isNewHigh(double price)
        {
           // Print(price);
            double pullbackInHandles = higherPrice - price;
            double forwardMovement = price - lastLowPrice;
            if (forwardMovement >= minMovement * TickSize && (pullbackInHandles) / (forwardMovement) * 100 >= pullbackThreshold) // dlugosc pullbacka stanowi x procent calego wzrostu
            {
                Print(forwardMovement);
                Print("Setting new High at " + higherPrice);
                Print(Time[0]);
                lastHighPrice = price;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isNewLow(double price)
        {
            // Print(price);
            double pullbackInHandles = price - lowerPrice;
            double forwardMovement = lastHighPrice - price;
            if (forwardMovement >= minMovement * TickSize && (pullbackInHandles) / (forwardMovement) * 100 >= pullbackThreshold) // dlugosc pullbacka stanowi x procent calego wzrostu
            {
                Print(forwardMovement);
                Print("Setting new Low at " + lowerPrice);
                Print(Time[0]);
                lastHighPrice = price;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Mark(string type)
        {
            int _nr = rnd.Next();
            string rando = Convert.ToString(_nr);
            string name = "tag " + rando;
            if (type == "low")
            {
                Draw.ArrowUp(this, name, true, 0, Low[0] - TickSize, Brushes.Red);
            } else if (type == "high")
            {
                Draw.ArrowDown(this, name, true, 0, High[0] - TickSize, Brushes.Blue);
            }
        }

        public class Price
        {
            private double price;
            private int index;
            private int time;

            public Price(double price, int index, int time)
            {
                this.price = price;
                this.index = index;
                this.time = time;
            }
            public double BarPrice
            {
                get { return price; }
                set { price = value; }
            }

            public int Index
            {
                get { return index; }
                set { index = value; }
            }

            public int Time
            {
                get { return time; }
                set { time = value; }
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
		private HigherHigh[] cacheHigherHigh;
		public HigherHigh HigherHigh()
		{
			return HigherHigh(Input);
		}

		public HigherHigh HigherHigh(ISeries<double> input)
		{
			if (cacheHigherHigh != null)
				for (int idx = 0; idx < cacheHigherHigh.Length; idx++)
					if (cacheHigherHigh[idx] != null &&  cacheHigherHigh[idx].EqualsInput(input))
						return cacheHigherHigh[idx];
			return CacheIndicator<HigherHigh>(new HigherHigh(), input, ref cacheHigherHigh);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.HigherHigh HigherHigh()
		{
			return indicator.HigherHigh(Input);
		}

		public Indicators.HigherHigh HigherHigh(ISeries<double> input )
		{
			return indicator.HigherHigh(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.HigherHigh HigherHigh()
		{
			return indicator.HigherHigh(Input);
		}

		public Indicators.HigherHigh HigherHigh(ISeries<double> input )
		{
			return indicator.HigherHigh(input);
		}
	}
}

#endregion
