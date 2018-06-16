using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using SpiceSharpRunner.Windows.Common;
using SpiceSharpRunner.Windows.ViewModels;
using SpiceSharpRunner.Windows.Windows;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace SpiceSharpRunner.Windows.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NetlistView
    {
        public bool CanClose => true;

        public NetlistView()
        {
            InitializeComponent();
            SearchPanel.Install(txtNetlist);

            txtNetlist.TextChanged += (e, a) =>
            {
                (DataContext as NetlistWindowViewModel).Netlist = txtNetlist.Text;
            };

            Dispatcher.InvokeAsync(() =>
            {
                txtNetlist.Text = (DataContext as NetlistWindowViewModel).Netlist;
            });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void SetWorkingDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Directory.SetCurrentDirectory(dialog.SelectedPath);
                }
            }
        }
    }
}
