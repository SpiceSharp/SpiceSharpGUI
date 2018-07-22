using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Simulations;
using SpiceSharpRunner.Windows.Common;
using System.Collections.Generic;

namespace SpiceSharp.Runner.Windows.ViewModels
{
    public class HistogramPlotViewModel : ViewModelBase
    {
        public HistogramPlotViewModel(MonteCarloResult data)
        {
            PlotModel = data.GetPlot(Bins);
            OxyPlotModel = CreateOxyPlotModel();
            Data = data; 
        }
        protected MonteCarloResult Data { get; set; }
        protected HistogramPlot PlotModel { get; set; }

        public PlotModel OxyPlotModel { get; private set; }

        private int _bins = 10;

        public int Bins
        {
            get
            {
                return _bins;
            }
            set
            {
                _bins = value;

                PlotModel = Data.GetPlot(Bins);
                OxyPlotModel = CreateOxyPlotModel();
                RaisePropertyChanged("OxyPlotModel");
            }
        }

        public double XMin
        {
            get
            {
                if (PlotModel == null) return 0;
                return PlotModel.XMin;
            }
        }

        public double XMax
        {
            get
            {
                if (PlotModel == null) return 0;
                return PlotModel.XMax;
            }
        }

        /// <summary>
        /// Creates Oxyplot library plot model
        /// </summary>
        /// <returns></returns>
        private PlotModel CreateOxyPlotModel()
        {
            var model = new PlotModel { Title = PlotModel.Name };
            model.IsLegendVisible = true;

            var items = new List<Item>();

            for (var i = 0; i < PlotModel.Bins.Count; i++)
            {
                if (PlotModel.Bins.ContainsKey(i + 1))
                {
                    items.Add(new Item() { Label = PlotModel.Bins[i + 1].Value.ToString("N5"), Value = PlotModel.Bins[i + 1].Count });
                }
            }

            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, Unit = PlotModel.XUnit, ItemsSource = items, LabelField = "Label" });
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
