using Hammer.MDIContainer.Control;
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

            lblStatus.Text = "Working directory: " + Directory.GetCurrentDirectory();

            this.MdiContainer.NewWindow += Container_NewWindow;
        }

        private void Container_NewWindow(object sender, System.EventArgs e)
        {
            var args = (MdiContainer.NewWindowEventArgs)e;

            args.Window.Height = this.MdiContainer.ActualHeight / 2;
            args.Window.Width = this.MdiContainer.ActualWidth / 2;
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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var vm = (this.DataContext as MainWindowViewModel);

            var container = this.MdiContainer;

            double h = 0;
            double totalHeight = container.ActualHeight;

            foreach (var window in container.Windows)
            {
                window.Height = totalHeight / container.Windows.Count;
                window.Width = container.ActualWidth;
                window.Position(0, h);
                h += window.Height;
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var vm = (this.DataContext as MainWindowViewModel);

            var container = this.MdiContainer;

            double w = 0;
            double totalWidth = container.ActualWidth;

            foreach (var window in container.Windows)
            {
                window.Width = totalWidth / container.Windows.Count;
                window.Height = container.ActualHeight;
                window.Position(w, 0);
                w += window.Width;
            }
        }
    }
}
