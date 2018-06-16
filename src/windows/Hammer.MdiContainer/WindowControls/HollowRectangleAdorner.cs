// Adorners must subclass the abstract base class Adorner.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Hammer.MDIContainer.Control.WindowControls
{
    public class HollowRectangleAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public HollowRectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            
        }
        protected override Size MeasureOverride(Size constraint)
        {
            var result = base.MeasureOverride(constraint);
            // ... add custom measure code here if desired ...
            InvalidateVisual();
            return result;
        }
        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            if ((AdornedElement as MdiWindow)?.Container==null) return;


            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Transparent);
            renderBrush.Opacity = 0.1;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Transparent), 0);


            var x = Math.Max(0, Canvas.GetLeft(AdornedElement));
            var y = Math.Max(0, Canvas.GetTop(AdornedElement));
            var w = AdornedElement.DesiredSize.Width;
            var h = AdornedElement.DesiredSize.Height;
            var ww = (AdornedElement as MdiWindow).Container.ActualWidth;
            var hh = (AdornedElement as MdiWindow).Container.ActualHeight;
            var xPlusw = Math.Min(ww, x + w);
            var yPlush = Math.Min(hh, y + h);


            var pointA = new Point(0,0);
            var pointB = new Point(xPlusw, 0);
            var pointD = new Point(0, y);
            var pointF = new Point(xPlusw, y);
            var pointG = new Point(0, yPlush);
            var pointH = new Point(x, yPlush);
            var pointK = new Point(xPlusw, hh);
            var pointL = new Point(ww, hh);


            var rectangle1 = new Rect(pointA, pointF);
            var rectangle2 = new Rect(pointB, pointL);
            var rectangle3 = new Rect(pointG, pointK);
            var rectangle4 = new Rect(pointD, pointH);


            drawingContext.PushTransform(new TranslateTransform(-Canvas.GetLeft(AdornedElement), -Canvas.GetTop(AdornedElement)));
            drawingContext.DrawRectangle(renderBrush, renderPen, rectangle1);
            drawingContext.DrawRectangle(renderBrush, renderPen, rectangle2);     
            drawingContext.DrawRectangle(renderBrush, renderPen, rectangle3);
            drawingContext.DrawRectangle(renderBrush, renderPen, rectangle4);


        }
    }
}