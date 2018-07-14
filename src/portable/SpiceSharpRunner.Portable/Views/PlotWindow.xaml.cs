using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using SpiceSharpRunner.Portable.ViewModels;

namespace SpiceSharpRunner.Portable.Views
{
    public class PlotWindow : Window
    {
        public PlotWindow(Plot plot)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.DataContext = new PlotWindowViewModel(plot);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
