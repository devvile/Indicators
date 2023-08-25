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
using PlotByEnum;
#endregion

//This namespace contains information for PlotBy
namespace PlotByEnum
{
	public enum PlotBy 
	{ 
		SK,
		SD,
	}
	
}

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	public class StochRSIMod2NT8 : Indicator
    {
    	private bool _arrowUpSeries;
        private bool _arrowDownSeries;
                              
        private double minimum;
        private double maximum;
        private double rsi;
        private Series<double> storsi;
		private SMA storsiSMA;
		private SMA skSMA;


                              
        private double SK0 = 0;
        private double SD0 = 0;
                              
        private double _obLevel = 20;
        private double _osLevel = 80;

                              
        private Series<double> _plotBySeries;
		PlotBy _plotBy = PlotBy.SK;
        private int periodRsi;
        private int fastMAPeriod;
        private int slowMAPeriod;
        private int lookBack;
        private int _arrowsOffset;

        protected override void OnStateChange()
        {
        	if (State == State.SetDefaults)
            {
            	Description   			 	= @"The StochRSIMod is an oscillator similar in computation to the stochastic measure,er a specified number.";
                Name                        = "StochRSIMod2NT8";
                Calculate                   = Calculate.OnBarClose;
                IsOverlay                   = false;
                DisplayInDataBox            = true;
                DrawOnPricePanel            = true;
                DrawHorizontalGridLines     = true;
                DrawVerticalGridLines       = true;
                PaintPriceMarkers           = true;
                ScaleJustification          = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                //Disable this property if your indicator requires custom values that cumulate with each new market data event.
                //See Help Guide for additional information
                IsSuspendedWhileInactive    = true;
                                                            
                PeriodRsi                 	= 21;
                FastMAPeriod              	= 5;
                SlowMAPeriod              	= 8;
                LookBack                    = 13;
                                                 
                AddPlot(Brushes.CornflowerBlue, "SK");
                AddPlot(Brushes.Firebrick, "SD");
                AddLine(Brushes.Gray, 80, "Overbought");
                AddLine(Brushes.Gray, 20, "Oversold");
				
				PlotBy _plotBy = PlotBy.SK;
                                                            
              }
              else if (State == State.Configure)
              {
              }
              else if (State == State.DataLoaded)
              {  
				//Set up Series 
					storsi = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);
				  	_plotBySeries = new Series<double>(this);	
				  
				//Set up SMAs
				  	storsiSMA = SMA(storsi, SlowMAPeriod);
				  	skSMA = SMA(SK, FastMAPeriod);
			                                  
              }
           }
			
		protected override void OnBarUpdate()
        {
        	{
            	if ((CurrentBar < PeriodRsi) || (CurrentBar < SlowMAPeriod) || (CurrentBar < FastMAPeriod))
                	return;
				
            	minimum = MIN(RSI(PeriodRsi, 1), LookBack)[0];
            	maximum = MAX(RSI(PeriodRsi, 1), LookBack)[0];
            	rsi = RSI(PeriodRsi, 1)[0];
            	double denominator = (maximum - minimum);
           		if (denominator == 0) denominator = 1;                 // Prevent divide by zero error.
            	storsi[0] = (rsi - minimum) / denominator * 100;
               
                SK[0] = storsiSMA[0]; 
            	SD[0] = skSMA[0]; 
				
            	if (SK[0] <= 0)
                SK0 = 0;
            	else
                	if (SK[0] >= 99.9999999999996)
                    	SK0 = 100;
                	else
                    	SK0 = SK[0];

            	if (SD[0] <= 0)
                	SD0 = 0;
            	else
                	if (SD[0] >= 99.9999999999996)
                    	SD0 = 100;
                	else
                    	SD0 = SD[0];
            
			
			
			if (_plotBy == PlotBy.SK)
            	{
                	_plotBySeries = SK;
            	}
            	else
            	{
                	_plotBySeries = SD;
            	}
			
            //arrows calculation
            if (CrossAbove(_plotBySeries, _osLevel, 1))
            {
            	_arrowUpSeries = true;
         
				
            }
            else
            {
                _arrowUpSeries = false;
            }
            if (CrossBelow(_plotBySeries, _obLevel, 1))
            {
                _arrowDownSeries = true;
            }
            else
            {
                _arrowDownSeries = false;
            }
        	}
       	}

        #region Properties
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name="PeriodRsi", Description="Numbers of bars used for calculations (default 21)", Order=1, GroupName="Parameters")]
        public int PeriodRsi
        {
        	get { return periodRsi; }
            set { periodRsi = Math.Max(1, value); }
        }
        
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
        [Display(Name="FastMAPeriod", Description="Fast Moving Average (default 5)", Order=2, GroupName="Parameters")]
        public int FastMAPeriod
        {
        	get { return fastMAPeriod; }
            set { fastMAPeriod = Math.Max(1, value); }
        }
		
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name="SlowMAPeriod", Description="Slow Moving Average (default 8)", Order=3, GroupName="Parameters")]
        public int SlowMAPeriod
        {
            get { return slowMAPeriod; }
            set { slowMAPeriod = Math.Max(1, value); }
        }
		
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name="LookBack", Description="Look back Period (default 13)", Order=4, GroupName="Parameters")]
        public int LookBack
        {
        	get { return lookBack; }
            set { lookBack = Math.Max(1, value); }
        }
	    
	   
	   
                           
        
        public Series<double> SK
        {
        	get { return Values[0]; }
        }
        
        public Series<double> SD
        {
        	get { return Values[1]; }
        }
                              

     	#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private StochRSIMod2NT8[] cacheStochRSIMod2NT8;
		public StochRSIMod2NT8 StochRSIMod2NT8(int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			return StochRSIMod2NT8(Input, periodRsi, fastMAPeriod, slowMAPeriod, lookBack);
		}

		public StochRSIMod2NT8 StochRSIMod2NT8(ISeries<double> input, int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			if (cacheStochRSIMod2NT8 != null)
				for (int idx = 0; idx < cacheStochRSIMod2NT8.Length; idx++)
					if (cacheStochRSIMod2NT8[idx] != null && cacheStochRSIMod2NT8[idx].PeriodRsi == periodRsi && cacheStochRSIMod2NT8[idx].FastMAPeriod == fastMAPeriod && cacheStochRSIMod2NT8[idx].SlowMAPeriod == slowMAPeriod && cacheStochRSIMod2NT8[idx].LookBack == lookBack && cacheStochRSIMod2NT8[idx].EqualsInput(input))
						return cacheStochRSIMod2NT8[idx];
			return CacheIndicator<StochRSIMod2NT8>(new StochRSIMod2NT8(){ PeriodRsi = periodRsi, FastMAPeriod = fastMAPeriod, SlowMAPeriod = slowMAPeriod, LookBack = lookBack }, input, ref cacheStochRSIMod2NT8);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.StochRSIMod2NT8 StochRSIMod2NT8(int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			return indicator.StochRSIMod2NT8(Input, periodRsi, fastMAPeriod, slowMAPeriod, lookBack);
		}

		public Indicators.StochRSIMod2NT8 StochRSIMod2NT8(ISeries<double> input , int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			return indicator.StochRSIMod2NT8(input, periodRsi, fastMAPeriod, slowMAPeriod, lookBack);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.StochRSIMod2NT8 StochRSIMod2NT8(int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			return indicator.StochRSIMod2NT8(Input, periodRsi, fastMAPeriod, slowMAPeriod, lookBack);
		}

		public Indicators.StochRSIMod2NT8 StochRSIMod2NT8(ISeries<double> input , int periodRsi, int fastMAPeriod, int slowMAPeriod, int lookBack)
		{
			return indicator.StochRSIMod2NT8(input, periodRsi, fastMAPeriod, slowMAPeriod, lookBack);
		}
	}
}

#endregion
