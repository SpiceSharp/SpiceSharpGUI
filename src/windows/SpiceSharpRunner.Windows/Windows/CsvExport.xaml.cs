using Microsoft.Win32;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Prints;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Prints;
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
using System.Windows.Shapes;

namespace SpiceSharp.Runner.Windows.Windows
{
    /// <summary>
    /// Interaction logic for CsvExport.xaml
    /// </summary>
    public partial class CsvExport : Window
    {
        public Print Print { get; }

        public CsvExport(Print print)
        {
            Print = print;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                Print.ToCsv(dialog.FileName, this.columns_seprator.Text, this.decimal_seprator.Text, this.header.IsChecked);
                MessageBox.Show("Zapisano");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application app = Application.Current;
            Window mainWindow = app.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }
    }
}
