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

namespace NinjaTrader.NinjaScript.Indicators
{

	public class EfficiencyRatioIndicator : Indicator
	{
		private Series<double> result;
		private bool _overlay;
		private bool _directional;
		private int _period;

protected override void OnStateChange()
{
    if (State == State.SetDefaults)
    {
        _overlay = false;
        Calculate = Calculate.OnEachTick;
        _period = 5;
        _directional = false;

        AddPlot(Brushes.DodgerBlue, "ER");
    }
    else if (State == State.DataLoaded)
    {
        result = new Series<double>(this);
    }
}

protected override void OnBarUpdate()
{
    double a = 0.0;
    double b = 0.0;
    double er = 0.0;

    if (CurrentBar < Period)
        return;

    for (int i = 0; i < Period; i++)
    {
        double closeA = Directional ? Close[i] - Close[i + 1] : Math.Abs(Close[i] - Close[i + 1]);
        a += closeA;
        b += Math.Abs(Close[i] - Close[i + 1]);
    }

    er = a / b;
    result[0] = er;

    // Update Values series
    if (CurrentBar >= 1)
    {
        Values[0][0] = er;
    }
}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Description="KER Period", Order=1, GroupName="Parameters")]
		public int Period {   
			get { return _period; }
            set { _period = Math.Max(1, value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "Directional", Description = "is Directional", Order = 2, GroupName = "Parameters")]
		public bool Directional
		{
			get { return _directional; }
			set { _directional = value; }
		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private EfficiencyRatioIndicator[] cacheEfficiencyRatioIndicator;
		public EfficiencyRatioIndicator EfficiencyRatioIndicator(int period, bool directional)
		{
			return EfficiencyRatioIndicator(Input, period, directional);
		}

		public EfficiencyRatioIndicator EfficiencyRatioIndicator(ISeries<double> input, int period, bool directional)
		{
			if (cacheEfficiencyRatioIndicator != null)
				for (int idx = 0; idx < cacheEfficiencyRatioIndicator.Length; idx++)
					if (cacheEfficiencyRatioIndicator[idx] != null && cacheEfficiencyRatioIndicator[idx].Period == period && cacheEfficiencyRatioIndicator[idx].Directional == directional && cacheEfficiencyRatioIndicator[idx].EqualsInput(input))
						return cacheEfficiencyRatioIndicator[idx];
			return CacheIndicator<EfficiencyRatioIndicator>(new EfficiencyRatioIndicator(){ Period = period, Directional = directional }, input, ref cacheEfficiencyRatioIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.EfficiencyRatioIndicator EfficiencyRatioIndicator(int period, bool directional)
		{
			return indicator.EfficiencyRatioIndicator(Input, period, directional);
		}

		public Indicators.EfficiencyRatioIndicator EfficiencyRatioIndicator(ISeries<double> input , int period, bool directional)
		{
			return indicator.EfficiencyRatioIndicator(input, period, directional);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.EfficiencyRatioIndicator EfficiencyRatioIndicator(int period, bool directional)
		{
			return indicator.EfficiencyRatioIndicator(Input, period, directional);
		}

		public Indicators.EfficiencyRatioIndicator EfficiencyRatioIndicator(ISeries<double> input , int period, bool directional)
		{
			return indicator.EfficiencyRatioIndicator(input, period, directional);
		}
	}
}

#endregion
