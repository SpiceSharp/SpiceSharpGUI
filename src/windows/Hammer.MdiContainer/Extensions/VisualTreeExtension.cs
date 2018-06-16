using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Hammer.MDIContainer.Control.Extensions
{
   internal static class VisualTreeExtension
   {
      public static TParent FindSpecificParent<TParent>(FrameworkElement sender)
         where TParent : FrameworkElement
      {
         var current = sender;
          if (current == null) return null;
         var p = VisualTreeHelper.GetParent(current) as FrameworkElement;

         if (p != null && p.GetType() != typeof(TParent))
         {
            p = FindSpecificParent<TParent>(p);
         }

          if (p == null && current.Parent is Popup)
          {
              var grandpa = ((Popup) current.Parent).Parent as FrameworkElement;
              if (grandpa != null)
              {
                  p = FindSpecificParent<TParent>(grandpa);
              }
              
          }
          
         return p as TParent;
      }


      public static MdiWindow FindMdiWindow(FrameworkElement sender)
      {
         return FindSpecificParent<MdiWindow>(sender);
      }
   }
}
