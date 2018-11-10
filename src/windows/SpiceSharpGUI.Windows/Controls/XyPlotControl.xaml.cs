using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SpiceSharpGUI.Windows.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SpiceSharpGUI.Windows.Controls
{
    /// <summary>
    /// Interaction logic for PlotControl.xaml
    /// </summary>
    public partial class XyPlotControl : UserControl
    {
        public XyPlotControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlotProperty = DependencyProperty.Register("Plot", typeof(SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Plots.XyPlot), typeof(XyPlotControl),
                        new PropertyMetadata(OnPlotPropertyChanged));

        public SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Plots.XyPlot Plot
        {
            get { return GetValue(PlotProperty) as SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Plots.XyPlot; }
            set
            {
                SetValue(PlotProperty, value);
                //DataBind();
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "BMP files (*.bmp)|*.bmp|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                this.plot.SaveBitmap(dialog.FileName);
                MessageBox.Show("Saved", "SpiceSharpGUI");
            }
        }

        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((XyPlotControl)d).Plot = e.NewValue as SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Plots.XyPlot;
            ((XyPlotControl)d).DataBind();
        }

        private void DataBind()
        {
            if (Plot != null)
            {
                XyPlotViewModel model = new XyPlotViewModel(Plot);
                this.DataContext = model;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var series in ((XyPlotViewModel)this.DataContext).Series)
            {
                series.Selected = false;
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (var series in ((XyPlotViewModel)this.DataContext).Series)
            {
                series.Selected = true;
            }
        }

        private void save_png_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
                pngExporter.ExportToFile(((XyPlotViewModel)this.DataContext).OxyPlotModel, dialog.FileName);
                MessageBox.Show("Saved", "SpiceSharpGUI");
            }

        }
    }
}
