using ICSharpCode.AvalonEdit.Search;
using SpiceSharpGUI.Windows.ViewModels;

namespace SpiceSharpGUI.Windows.Views
{
    /// <summary>
    /// Spice netlist run result window
    /// </summary>
    public partial class NetlistResultView
    {
        public NetlistResultView() 
        {
            InitializeComponent();
            SearchPanel.Install(txtNetlist);
            Dispatcher.InvokeAsync(() =>
            {
                txtNetlist.Text = (DataContext as NetlistResultWindowViewModel).Netlist;
            });
        }
    }
}
 