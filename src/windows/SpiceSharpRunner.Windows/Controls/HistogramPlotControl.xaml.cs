using SpiceSharp.Runner.Windows.ViewModels;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Simulations;
using System.Windows.Controls;

namespace SpiceSharp.Runner.Windows.Controls
{
    /// <summary>
    /// Interaction logic for HistogramPlotControl.xaml
    /// </summary>
    public partial class HistogramPlotControl : UserControl
    {
        public MonteCarloResult Data { get; set; }

        public HistogramPlotControl()
        {
            InitializeComponent();
        }

        public void DataBind()
        {
            if (Data != null)
            {
                HistogramPlotViewModel model = new HistogramPlotViewModel(Data);
                this.DataContext = model;
            }
        }
    }
}
