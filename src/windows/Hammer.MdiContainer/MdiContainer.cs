using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Hammer.MDIContainer.Control.Events;

namespace Hammer.MDIContainer.Control
{
    public sealed class MdiContainer : Selector
    {
        public class NewWindowEventArgs : EventArgs
        {
            public MdiWindow Window { get; set; }
        }

        private IList InternalItemSource { get; set; }
        internal int MinimizedWindowsCount { get; private set; }


        public MdiContainer() : base()
        {
            this.SelectionChanged += MdiContainer_SelectionChanged;
        }

        private void MdiContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                 var windowNew = ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) as MdiWindow;
                 if (windowNew != null) windowNew.SetValue(MdiWindow.IsSelectedProperty, true);
            }
           
        }
        public static readonly DependencyProperty IsModalProperty =
     DependencyProperty.Register("IsModal", typeof(bool?), typeof(MdiContainer), new UIPropertyMetadata(IsModalChangedCallback));
        private static void IsModalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((MdiContainer)d).IsModal = (bool)e.NewValue;
        }
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set
            {
               
                SetValue(IsModalProperty, value);

            }
        }

        static MdiContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiContainer), new FrameworkPropertyMetadata(typeof(MdiContainer)));
        }

        public List<MdiWindow> Windows { get; set; } = new List<MdiWindow>();

        public event EventHandler NewWindow;

        protected override DependencyObject GetContainerForItemOverride()
        {
            var window = new MdiWindow();
            Windows.Add(window);

            if (NewWindow != null)
            {
                NewWindow.Invoke(this, new NewWindowEventArgs() { Window = window });
            }

            return window;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var window = element as MdiWindow;
            if (window != null)
            {
                window.IsCloseButtonEnabled = InternalItemSource != null;
                window.FocusChanged += OnWindowFocusChanged;
                window.Closing += OnWindowClosing;
                window.WindowStateChanged += OnWindowStateChanged;
                window.Initialize(this);

                window.Position();

                window.Focus();
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        private void OnWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            if (e.NewValue == WindowState.Minimized)
            {
                MinimizedWindowsCount++;
            }
            else if (e.OldValue == WindowState.Minimized)
            {
                MinimizedWindowsCount--;
            }
        }

        private void OnWindowClosing(object sender, RoutedEventArgs e)
        {
            var window = sender as MdiWindow;
            if (window?.DataContext != null)
            {
                InternalItemSource?.Remove(window.DataContext);
                if (Items.Count > 0)
                {
                    SelectedItem = Items[Items.Count - 1];
                    var windowNew = ItemContainerGenerator.ContainerFromItem(SelectedItem) as MdiWindow;
                    if (windowNew != null) windowNew.IsSelected = true;
                }

                // clear
                window.FocusChanged -= OnWindowFocusChanged;
                window.Closing -= OnWindowClosing;
                window.WindowStateChanged -= OnWindowStateChanged;
                window.DataContext = null;

                Windows.Remove(window);
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null && newValue is IList)
            {
                InternalItemSource = newValue as IList;
            }
        }

        private void OnWindowFocusChanged(object sender, RoutedEventArgs e)
        {
            if (((MdiWindow)sender).IsFocused)
            {
                SelectedItem = e.OriginalSource;

                ((MdiWindow) ItemContainerGenerator.ContainerFromItem(SelectedItem)).IsSelected = true;

                foreach (var item in Items)
                {
                    if (item != e.OriginalSource)
                    {
                        var window = ItemContainerGenerator.ContainerFromItem(item) as MdiWindow;
                        if (window != null)
                        {
                            window.IsSelected = false;
                            Panel.SetZIndex(window, 0);
                        }
                    }
                }
                SelectedItem = e.OriginalSource;

                ((MdiWindow)ItemContainerGenerator.ContainerFromItem(SelectedItem)).IsSelected = true;

            }
        }
    }
}
