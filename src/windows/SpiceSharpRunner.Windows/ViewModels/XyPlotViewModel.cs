using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using SpiceSharpRunner.Windows.Logic;
using System;
using System.Windows.Media;

namespace SpiceSharp.Runner.Windows.ViewModels
{
    public class XyPlotViewModel
    {
        private Random rand;

        public XyPlotViewModel(XyPlot plot)
        {
            rand = new Random(Environment.TickCount);

            Series = new Series[plot.Series.Count];
            for (var i = 0; i < Series.Length; i++)
            {
                Series[i] = new Series() { Name = plot.Series[i].Name, Selected = true };
            }

            YScaleLogEnabled = SpiceHelper.IsPlotPositive(plot);
            OxyPlotModel = CreateOxyPlotModel(plot);
        }

        public PlotModel OxyPlotModel { get; private set; }

        public bool YScaleLogEnabled { get; set; }
        public event EventHandler YScaleLogChanged;
        public event EventHandler XScaleLogChanged;
        public event EventHandler ShowLegendChanged;

        private bool _yScaleLog;
        public bool YScaleLog
        {
            get
            {
                return _yScaleLog;
            }

            set
            {
                _yScaleLog = value;
                YScaleLogChanged?.Invoke(this, null);
            }
        }

        private bool _xScaleLog;
        public bool XScaleLog
        {
            get
            {
                return _xScaleLog;
            }

            set
            {
                _xScaleLog = value;
                XScaleLogChanged?.Invoke(this, null);
            }
        }

        private bool _showLegend;
        public bool ShowLegend
        {
            get
            {
                return _showLegend;
            }

            set
            {
                _showLegend = value;
                ShowLegendChanged?.Invoke(this, null);
            }
        }

        public Series[] Series { get; set; }

        public int Count
        {
            get
            {
                return Series.Length;
            }
        }

        /// <summary>
        /// Creates Oxyplot library plot model
        /// </summary>
        /// <returns></returns>
        private PlotModel CreateOxyPlotModel(XyPlot plot)
        {
            var tmp = new PlotModel { Title = plot.Name };
            tmp.IsLegendVisible = ShowLegend;

            this.ShowLegendChanged += (o, e) => { tmp.IsLegendVisible = this.ShowLegend; OxyPlotModel.InvalidatePlot(false); };

            for (var i = 0; i < plot.Series.Count; i++)
            {
                if (plot.Series[i].Points.Count > 1)
                {
                    var series = new LineSeries { Title = plot.Series[i].Name, MarkerType = MarkerType.None, Color = OxyColor.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255))};

                    Series[i].Brush = new SolidColorBrush(Color.FromRgb(series.Color.R, series.Color.G, series.Color.B));
                    
                    tmp.Series.Add(series);

                    for (var j = 0; j < plot.Series[i].Points.Count; j++)
                    {
                        var y = plot.Series[i].Points[j].Y;
                        series.Points.Add(new DataPoint(plot.Series[i].Points[j].X, y));
                    }

                    series.IsVisible = Series[i].Selected;

                    Series[i].SelectedChanged += (o, e) => { series.IsVisible = ((SpiceSharp.Runner.Windows.ViewModels.Series)o).Selected; OxyPlotModel.InvalidatePlot(false); };
                }
                else
                {
                    var scatterSeries = new ScatterSeries {
                        Title = plot.Series[i].Name,
                        MarkerSize = 3,
                        SelectionMode = SelectionMode.Single,
                        MarkerType = MarkerType.Circle,
                        MarkerFill = OxyColor.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255))
                    };
                    Series[i].Brush = new SolidColorBrush(Color.FromRgb(scatterSeries.MarkerFill.R, scatterSeries.MarkerFill.G, scatterSeries.MarkerFill.B));

                    scatterSeries.Points.Add(new ScatterPoint(plot.Series[i].Points[0].X, plot.Series[i].Points[0].Y));
                    tmp.Series.Add(scatterSeries);

                    scatterSeries.IsVisible = Series[i].Selected;
                    Series[i].SelectedChanged += (o, e) => { scatterSeries.IsVisible = ((SpiceSharp.Runner.Windows.ViewModels.Series)o).Selected; OxyPlotModel.InvalidatePlot(false); };

                }
            }

            CreateAxis(plot, tmp);

            YScaleLogChanged += (o, e) => { CreateAxis(plot, tmp); OxyPlotModel.InvalidatePlot(false); };
            XScaleLogChanged += (o, e) => { CreateAxis(plot, tmp); OxyPlotModel.InvalidatePlot(false); };

            return tmp;
        }

        private void CreateAxis(XyPlot plot, PlotModel tmp)
        {
            if (plot.Series.Count > 0)
            {
                string xUnit = plot.Series[0].XUnit;
                string yUnit = plot.Series[0].YUnit;

                tmp.Axes.Clear();

                if (XScaleLog)
                {
                    tmp.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Unit = xUnit });
                }
                else
                {
                    tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Unit = xUnit });
                }

                if (YScaleLog)
                {
                    tmp.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left, Unit = yUnit });
                }
                else
                {
                    tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Unit = yUnit });
                }
            }
        }
    }

    public class Series
    {
        public string Name { get; set; }

        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
                SelectedChanged?.Invoke(this, null);
            }
        }

        public SolidColorBrush Brush { get; internal set; }

        public event EventHandler SelectedChanged;
    }
}
