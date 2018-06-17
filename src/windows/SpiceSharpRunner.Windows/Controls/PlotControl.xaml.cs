using Microsoft.Win32;
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

        public static readonly DependencyProperty PlotProperty = DependencyProperty.Register("Plot", typeof(Plot), typeof(PlotControl),
                        new PropertyMetadata(OnPlotPropertyChanged));

        public Plot Plot
        {
            get { return GetValue(PlotProperty) as Plot; }
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
                MessageBox.Show("Saved");
            }
        }

        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotControl)d).Plot = e.NewValue as Plot;
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
        
    }
}
