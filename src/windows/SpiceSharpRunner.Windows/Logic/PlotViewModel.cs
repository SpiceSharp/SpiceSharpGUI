using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;

namespace SpiceSharpRunner.Windows.Logic
{
    /// <summary>
    /// Plot view model
    /// </summary>
    public class PlotViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlotViewModel"/> class.
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="xLog"></param>
        /// <param name="yLog"></param>
        public PlotViewModel(Plot plot, bool xLog = false, bool yLog = false, bool legendVisible = false)
        {
            OxyPlotModel = CreateOxyPlotModel(plot, xLog, yLog);
            OxyPlotModel.IsLegendVisible = legendVisible;
            OxyPlotModel.LegendPosition = LegendPosition.BottomRight;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel OxyPlotModel { get; private set; }

        /// <summary>
        /// Creates Oxyplot library plot model
        /// </summary>
        /// <param name="plot">Plot data</param>
        /// <param name="xLog">Specifies whether x-axis is logaritmic</param>
        /// <param name="yLog">Specifies whether y-axis is logaritmic</param>
        /// <returns></returns>
        private static PlotModel CreateOxyPlotModel(Plot plot, bool xLog, bool yLog)
        {
            var tmp = new PlotModel { Title = plot.Name };

            for (var i = 0; i < plot.Series.Count; i++)
            {
                if (plot.Series[i].Points.Count > 1)
                {
                    var series = new LineSeries { Title = plot.Series[i].Name, MarkerType = MarkerType.None };
                    tmp.Series.Add(series);

                    for (var j = 0; j < plot.Series[i].Points.Count; j++)
                    {
                        var y = plot.Series[i].Points[j].Y;
                        series.Points.Add(new DataPoint(plot.Series[i].Points[j].X, y));
                    }
                }
                else
                {
                    var scatterSeries = new ScatterSeries { Title = plot.Series[i].Name, MarkerSize = 3, SelectionMode = SelectionMode.Single, MarkerType = MarkerType.Circle };
                    scatterSeries.Points.Add(new ScatterPoint(plot.Series[i].Points[0].X, plot.Series[i].Points[0].Y));
                    tmp.Series.Add(scatterSeries);
                }
            }

            if (plot.Series.Count > 0)
            {
                string xUnit = plot.Series[0].XUnit;
                string yUnit = plot.Series[0].YUnit;

                if (xLog)
                {
                    tmp.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Unit = xUnit });
                }
                else
                {
                    tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Unit = xUnit });
                }

                if (yLog)
                {
                    tmp.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left, Unit = yUnit });
                }
                else
                {
                    tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Unit = yUnit });
                }
            }

            return tmp;
        }
    }
}
