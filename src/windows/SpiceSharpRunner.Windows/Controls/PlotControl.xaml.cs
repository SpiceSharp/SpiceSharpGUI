using Microsoft.Win32;
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

        public bool YEnabled { get; set; }

        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotControl)d).Plot = e.NewValue as Plot;
            ((PlotControl)d).DataBind();
        }

        public Plot Plot
        {
            get { return GetValue(PlotProperty) as Plot; }
            set
            {
                SetValue(PlotProperty, value);
                DataBind();
            }
        }

        private void PlotControl_Initialized(object sender, System.EventArgs e)
        {
            DataBind();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            DataBind();
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            DataBind();
        }

        private void CheckBox_Click_3(object sender, RoutedEventArgs e)
        {
            DataBind();
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

        private void DataBind()
        {
            if (Plot != null)
            {
                PlotViewModel model = new PlotViewModel(Plot, this.x.IsChecked.Value, this.y.IsChecked.Value, this.legend.IsChecked.Value);
                this.y.IsEnabled = YEnabled;
                this.DataContext = model;
            }
        }
    }
}
