using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Hammer.MDIContainer.Control.Events;
using Hammer.MDIContainer.Control.Extensions;
using Hammer.MDIContainer.Control.WindowControls;

namespace Hammer.MDIContainer.Control
{
    [TemplatePart(Name = "PART_Border", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BorderGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Header", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ButtonBar", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_ButtonBar_CloseButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MaximizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MinimizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_BorderContent", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_MoverThumb", Type = typeof(MoveThumb))]
    [TemplatePart(Name = "PART_ResizerThumb", Type = typeof(ResizeThumb))]
    [TemplatePart(Name = "PART_Thumblr", Type = typeof(Image))]
    [DebuggerDisplay("{Title}")]
    public sealed class MdiWindow : ContentControl
    {
        internal double LastTop { get; set; }
        internal double LastLeft { get; set; }
        internal double LastWidth { get; set; }
        internal double LastHeight { get; set; }

        internal MdiContainer Container { get; private set; }
        internal WindowState PreviousWindowState { get; set; }


        public Image Tumblr { get; private set; }

        private WindowButton _closeButton;
        private WindowButton _maximizeButton;
        private WindowButton _minimizeButton;

        public MdiWindow()
        {
            _myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            
        }

   

        static MdiWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MdiWindow), new FrameworkPropertyMetadata(typeof(MdiWindow)));
        
        }

        private AdornerLayer _myAdornerLayer;

        internal void Initialize(MdiContainer container)
        {
            Container = container;
            Container.SizeChanged += OnContainerSizeChanged;
           // SizeChanged +=
            LastHeight = ActualHeight;
            LastWidth = ActualWidth;

        }

        public void Position()
        {
            var actualContainerHeight = Container.ActualHeight;
            var actualContainerWidth = Container.ActualWidth;
            UpdateLayout();
            InvalidateMeasure();
            var actualWidth = ActualWidth;
            var actualHeight = ActualHeight;

            var left = Math.Max(0, (actualContainerWidth - actualWidth)/2);
            var top = Math.Max(0, (actualContainerHeight - actualHeight) / 2);


            SetValue(Canvas.LeftProperty, left);
            SetValue(Canvas.TopProperty, top);

        }


        public void Position(double left, double top)
        {
            UpdateLayout();
            InvalidateMeasure();

            SetValue(Canvas.LeftProperty, left);
            SetValue(Canvas.TopProperty, top);
        }

        private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Width += e.NewSize.Width - e.PreviousSize.Width;
                Height += e.NewSize.Height - e.PreviousSize.Height;

                this.RemoveWindowLock();
            }

            if (WindowState == WindowState.Minimized)
            {
                Canvas.SetTop(this, Container.ActualHeight - 32);
            }
        }

        public override void OnApplyTemplate()
        {
            _closeButton = GetTemplateChild("PART_ButtonBar_CloseButton") as WindowButton;
            if (_closeButton != null)
            {
                _closeButton.Click += CloseWindow;
            }

            _maximizeButton = GetTemplateChild("PART_ButtonBar_MaximizeButton") as WindowButton;
            if (_maximizeButton != null)
            {
                _maximizeButton.Click += ToggleMaximizeWindow;
            }

            _minimizeButton = GetTemplateChild("PART_ButtonBar_MinimizeButton") as WindowButton;
            if (_minimizeButton != null)
            {
                _minimizeButton.Click += ToggleMinimizeWindow;
            }

            Tumblr = GetTemplateChild("PART_Tumblr") as Image;
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
               
            }
        }

        private Adorner _myAdorner;
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set
            {
                if (value)
                {
                    if(_myAdornerLayer == null)
                        _myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (_myAdorner == null)
                    {
                        _myAdorner = new HollowRectangleAdorner(this);
                    }
                    _myAdornerLayer.Add(_myAdorner);

                }
                else
                {
                    _myAdornerLayer?.Remove(_myAdorner);
                }
                if(Container!=null)
                    Container.IsModal = value;

                SetValue(IsModalProperty, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(false));



        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register("IsModal", typeof(bool?), typeof(MdiWindow), new UIPropertyMetadata(IsModalChangedCallback));

        private static void IsModalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ((MdiWindow) d).IsModal = (bool) e.NewValue;
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MdiWindow), new PropertyMetadata(string.Empty));

        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MdiWindow), new PropertyMetadata(WindowState.Normal, OnWindowStateChanged));

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
            "Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MdiWindow));

        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MdiWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.NotDataBindable));

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        public static readonly RoutedEvent FocusChangedEvent = EventManager.RegisterRoutedEvent(
           "FocusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MdiWindow));

        public event RoutedEventHandler FocusChanged
        {
            add { AddHandler(FocusChangedEvent, value); }
            remove { RemoveHandler(FocusChangedEvent, value); }
        }

        public bool HasDropShadow
        {
            get { return (bool)GetValue(HasDropShadowProperty); }
            set { SetValue(HasDropShadowProperty, value); }
        }

        public static readonly DependencyProperty HasDropShadowProperty =
            DependencyProperty.Register("HasDropShadow", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(true));

        public delegate void WindowStateChangedRoutedEventHandler(object sender, WindowStateChangedEventArgs e);

        public static readonly RoutedEvent WindowStateChangedEvent = EventManager.RegisterRoutedEvent(
           "WindowStateChanged", RoutingStrategy.Bubble, typeof(WindowStateChangedRoutedEventHandler), typeof(MdiWindow));

        public event WindowStateChangedRoutedEventHandler WindowStateChanged
        {
            add { AddHandler(WindowStateChangedEvent, value); }
            remove { RemoveHandler(WindowStateChangedEvent, value); }
        }

        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(MdiWindow), new FrameworkPropertyMetadata(true));

        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set {
                if (!value)
                {
                    Height = double.NaN;
                    Width = double.NaN; 
                }
                SetValue(IsResizableProperty, value); }
        }

        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register("IsResizable", typeof(bool), typeof(MdiWindow), new UIPropertyMetadata(IsResizableChangedCallback));
        private static void IsResizableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool) e.NewValue ))
            {
                ((MdiWindow)d).Height = double.NaN;
                ((MdiWindow)d).Width = double.NaN;
            }

            if (e.NewValue == null) return;
            ((MdiWindow)d).IsResizable = (bool)e.NewValue;


        }


   
        private static void OnWindowStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as MdiWindow;
            if (window != null)
            {
                window.PreviousWindowState = (WindowState)e.OldValue;

                var args = new WindowStateChangedEventArgs(WindowStateChangedEvent, (WindowState)e.OldValue, (WindowState)e.NewValue);
                window.RaiseEvent(args);
            }
        }

        public void DoFocus(MouseButtonEventArgs mouseButtonEventArgs)
        {
           OnMouseLeftButtonDown(mouseButtonEventArgs);

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsSelected = true;
            Panel.SetZIndex(this, 2);
            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, DataContext));

            Focus();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {

            base.OnLostKeyboardFocus(e);
            FrameworkElement parent =  VisualTreeExtension.FindMdiWindow(e.NewFocus as FrameworkElement);
            if ((e.NewFocus is MdiWindow && !Equals(e.NewFocus, this) )|| (parent  != null && !Equals(parent, this)))
            {
                IsSelected = false;
                Panel.SetZIndex(this, 0);
                var newWindow = (e.NewFocus is MdiWindow) ? (e.NewFocus as MdiWindow) : (parent as MdiWindow);
                Container.SetValue(MdiContainer.SelectedItemProperty , newWindow.DataContext);
                newWindow.IsSelected = true;
                //RaiseEvent(new RoutedEventArgs(FocusChangedEvent, DataContext));
            }

        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            IsSelected = true;
            Panel.SetZIndex(this, 2);

            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, DataContext));
        }

        private void ToggleMaximizeWindow(object sender, RoutedEventArgs e)
        {
            Focus();
            this.ToggleMaximize();
        }

        private void ToggleMinimizeWindow(object sender, RoutedEventArgs e)
        {
            Focus();
            this.ToggleMinimize();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var canCloseBinding = BindingOperations.GetBindingExpression(this, CanCloseProperty);
            if (canCloseBinding != null)
            {
                canCloseBinding.UpdateTarget();
            }

            if (CanClose)
            {
                RaiseEvent(new RoutedEventArgs(ClosingEvent));
            }
        }
    }
}
