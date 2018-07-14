using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using OxyPlot.Avalonia;
using SpiceSharpRunner.Portable.Views;

namespace SpiceSharpRunner.Portable
{
    class Program
    {
        static void Main(string[] args)
        {
            OxyPlotModule.EnsureLoaded();
            BuildAvaloniaApp().Start<MainWindow>();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .BeforeStarting(_ => OxyPlotModule.Initialize())
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
