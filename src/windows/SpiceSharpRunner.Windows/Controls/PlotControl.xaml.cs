using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SpiceSharp.Runner.Windows.ViewModels;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using SpiceSharpRunner.Windows.Logic;
using System.Windows;
using System.Windows.Controls;

namespace SpiceSharpRunner.Windows.Controls
{
    /// <summary>
    /// Interaction logic for PlotControl.xaml
    /// </summary>
    public partial class PlotControl : UserControl
    {
        public PlotControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlotProperty = DependencyProperty.Register("Plot", typeof(SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot), typeof(PlotControl),
                        new PropertyMetadata(OnPlotPropertyChanged));

        public SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot Plot
        {
            get { return GetValue(PlotProperty) as SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot; }
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
                MessageBox.Show("Saved", "SpiceSharpRunner");
            }
        }

        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotControl)d).Plot = e.NewValue as SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots.Plot;
            ((PlotControl)d).DataBind();
        }

        private void DataBind()
        {
            if (Plot != null)
            {
                PlotViewModel model = new PlotViewModel(Plot);
                this.DataContext = model;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var series in ((PlotViewModel)this.DataContext).Series)
            {
                series.Selected = false;
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (var series in ((PlotViewModel)this.DataContext).Series)
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
                pngExporter.ExportToFile(((PlotViewModel)this.DataContext).OxyPlotModel, dialog.FileName);
                MessageBox.Show("Saved", "SpiceSharpRunner");
            }

        }
    }
}
