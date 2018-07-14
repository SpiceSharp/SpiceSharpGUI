using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SpiceSharpRunner.Portable.ViewModels
{
    public class PlotWindowViewModel 
    {
        public PlotWindowViewModel(SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot plot)
        {
            PlotModel = CreateOxyPlotModel(plot);
        }

        public PlotModel PlotModel { get; private set; }

        /// <summary>
        /// Creates Oxyplot library plot model
        /// </summary>
        /// <returns></returns>
        private PlotModel CreateOxyPlotModel(SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot plot)
        {
            var tmp = new PlotModel { Title = plot.Name };
            tmp.IsLegendVisible = true;
          
            for (var i = 0; i < plot.Series.Count; i++)
            {
                if (plot.Series[i].Points.Count > 1)
                {
                    var series = new OxyPlot.Series.LineSeries { Title = plot.Series[i].Name, MarkerType = MarkerType.None };                  
                    tmp.Series.Add(series);

                    for (var j = 0; j < plot.Series[i].Points.Count; j++)
                    {
                        var y = plot.Series[i].Points[j].Y;
                        series.Points.Add(new DataPoint(plot.Series[i].Points[j].X, y));
                    }
                }
                else
                {
                    var scatterSeries = new OxyPlot.Series.ScatterSeries
                    {
                        Title = plot.Series[i].Name,
                        MarkerSize = 3,
                        SelectionMode = SelectionMode.Single,
                        MarkerType = MarkerType.Circle,                      
                    };
                 
                    scatterSeries.Points.Add(new ScatterPoint(plot.Series[i].Points[0].X, plot.Series[i].Points[0].Y));
                    tmp.Series.Add(scatterSeries);         
                }
            }

            CreateAxis(plot, tmp);

            return tmp;
        }

        private void CreateAxis(SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot plot, PlotModel tmp)
        {
            if (plot.Series.Count > 0)
            {
                string xUnit = plot.Series[0].XUnit;
                string yUnit = plot.Series[0].YUnit;

                tmp.Axes.Clear();

                tmp.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom, Unit = xUnit });
                tmp.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Unit = yUnit });
                
            }
        }
    }
}
