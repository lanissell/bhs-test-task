using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace Graphics.Views
{
    public partial class DrawingControl : Control
    {
        public static readonly StyledProperty<IEnumerable<Edge>> EdgesProperty = AvaloniaProperty.Register<DrawingControl, IEnumerable<Edge>>(nameof(Edges));

        public IEnumerable<Edge> Edges
        {
            get => GetValue(EdgesProperty);
            set => SetValue(EdgesProperty, value);
        }

        static DrawingControl()
        {
            AffectsRender<DrawingControl>(EdgesProperty);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var pen = new Pen(new SolidColorBrush(Colors.White), 2);

            foreach (var edge in Edges)
            {
                context.DrawLine(pen,
                    new Point(edge.VertexA.X, edge.VertexA.Y),
                    new Point(edge.VertexB.X, edge.VertexB.Y));
            }
        }
    }
}
