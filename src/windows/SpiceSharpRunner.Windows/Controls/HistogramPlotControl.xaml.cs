using SpiceSharp.Runner.Windows.ViewModels;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Simulations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
