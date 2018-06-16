using System.Windows;

namespace Hammer.MDIContainer.Control.Events
{
   public sealed class WindowStateChangedEventArgs : RoutedEventArgs
   {
      public WindowState OldValue { get; private set; }
      public WindowState NewValue { get; private set; }

      public WindowStateChangedEventArgs(RoutedEvent routedEvent, WindowState oldValue, WindowState newValue)
         : base(routedEvent)
      {
         NewValue = newValue;
         OldValue = oldValue;                  
      }
   }
}
