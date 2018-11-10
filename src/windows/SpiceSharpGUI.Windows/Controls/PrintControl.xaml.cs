using SpiceSharpGUI.Windows.Windows;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Prints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace SpiceSharpGUI.Windows.Controls
{
    /// <summary>
    /// Interaction logic for PrintControl.xaml
    /// </summary>
    public partial class PrintControl : UserControl
    {
        public Print Print { get; }

        public PrintControl(Print print)
        {
            Print = print;
            InitializeComponent();
        }

        public void DataBind()
        {
            this.Title.Content = $"{ Print.Name } ";
            Bind(Print.ColumnNames.ToArray(), Print.Rows);
        }

        void Bind(string[] newColumnNames, List<Row> rows)
        {
            var data = new DataTable();

            foreach (string name in newColumnNames)
            {
                data.Columns.Add(name);
            }

            foreach (var row in rows)
            {
                data.Rows.Add(row.Columns.Select(c => (object)c).ToArray());
            }

            this.dataGrid.ItemsSource = data.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new CsvExport(this.Print);
            window.ShowDialog();
        }
    }
}
