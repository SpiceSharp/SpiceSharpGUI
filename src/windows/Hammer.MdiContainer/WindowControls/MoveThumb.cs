using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Hammer.MDIContainer.Control.Extensions;

namespace Hammer.MDIContainer.Control.WindowControls
{
   public sealed class MoveThumb : Thumb
   {
      static MoveThumb()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveThumb), new FrameworkPropertyMetadata(typeof(MoveThumb)));
      }

      public MoveThumb()
      {

         DragDelta += OnMoveThumbDragDelta;
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         var window = VisualTreeExtension.FindMdiWindow(this);         

         if (window != null)
         {
            window.DoFocus(e);         
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
      {
         var window = VisualTreeExtension.FindMdiWindow(this);         

         if (window != null && window.Container != null)
         {
            switch (window.WindowState)
            { 
               case WindowState.Maximized:
                  window.Normalize();
                  break;
               case WindowState.Normal:
                  window.Maximize();
                  break;
               case WindowState.Minimized:
                  window.Normalize();
                  break;
               default:
                  throw new InvalidOperationException("Unsupported WindowsState mode");
            }
         }

         e.Handled = true;
      }

      private void OnMoveThumbDragDelta(object sender, DragDeltaEventArgs e)
      {
         var window = VisualTreeExtension.FindMdiWindow(this);

         if (window != null)                  
         {
             

                if (window.WindowState == WindowState.Maximized)
            {
               window.Normalize();
            }

            if (window.WindowState != WindowState.Minimized)
            {
                   
               window.LastLeft = Canvas.GetLeft(window);
               window.LastTop = Canvas.GetTop(window);


               var  candidateLeft =  window.LastLeft + e.HorizontalChange;
               var candidateTop = window.LastTop + e.VerticalChange;

               Canvas.SetLeft(window, Math.Min(Math.Max(0,candidateLeft), window.Container.ActualWidth -25));
               Canvas.SetTop(window, Math.Min(Math.Max(0, candidateTop), window.Container.ActualHeight  - 25));
            }
         }
      }
   }
}
