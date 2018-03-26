using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace SpiceSharp.Runner.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            SpiceNetlistResult netlistWindow = new SpiceNetlistResult(this.txtEditor.Text);
            netlistWindow.Show();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Circuit files (*.cir)|*.cir|Netlist files (*.net)|*.net|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, txtEditor.Text);
                MessageBox.Show("Saved");
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Circuit files (*.cir)|*.cir|Netlist files (*.net)|*.net|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
    }
}
