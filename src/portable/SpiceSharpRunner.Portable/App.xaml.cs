using Avalonia;
using Avalonia.Markup.Xaml;

namespace SpiceSharpRunner.Portable
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }
    }
}
