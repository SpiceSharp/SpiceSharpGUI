using SpiceSharp.Runner.Windows.ViewModels;
using SpiceSharp.Runner.Windows.Windows;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SpiceSharpRunner.Windows.Windows
{
    /// <summary>
    /// MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel(Dispatcher);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Directory.SetCurrentDirectory(dialog.SelectedPath);

                    lblStatus.Text = "Working directory: " + dialog.SelectedPath;
                }
            }
        }
    }
}
