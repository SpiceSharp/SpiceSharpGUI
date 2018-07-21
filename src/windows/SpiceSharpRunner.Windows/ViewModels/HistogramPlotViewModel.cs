using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using System.Collections.Generic;

namespace SpiceSharp.Runner.Windows.ViewModels
{
    public class HistogramPlotViewModel
    {
        public HistogramPlotViewModel(HistogramPlot plot)
        {
            OxyPlotModel = CreateOxyPlotModel(plot);
        }

        public PlotModel OxyPlotModel { get; private set; }

        /// <summary>
        /// Creates Oxyplot library plot model
        /// </summary>
        /// <returns></returns>
        private PlotModel CreateOxyPlotModel(HistogramPlot plot)
        {
            var model = new PlotModel { Title = plot.Name };
            model.IsLegendVisible = true;

            var items = new List<Item>();

            for (var i = 0; i < plot.Bins.Count; i++)
            {
                if (plot.Bins.ContainsKey(i + 1))
                {
                    items.Add(new Item() { Label = plot.Bins[i + 1].Value.ToString("N1"), Value = plot.Bins[i + 1].Count });
                }
            }

            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, Unit = plot.XUnit, ItemsSource = items, LabelField = "Label" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, AbsoluteMinimum = 0 });
            model.Series.Add(new ColumnSeries { Title = "Count", ItemsSource = items, ValueField = "Value" });

            return model;
        }

        public class Item
        {
            public string Label { get; set; }
            public double Value { get; set; }
        }
    }
}
