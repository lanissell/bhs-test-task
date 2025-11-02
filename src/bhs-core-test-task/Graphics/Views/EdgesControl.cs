using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Graphics.ViewModels;

namespace Graphics.Views;

public class EdgesControl : Control
{
    public static readonly StyledProperty<EdgesViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<EdgesControl, EdgesViewModel?>(nameof(ViewModel));

    public EdgesViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);

        set => SetValue(ViewModelProperty, value);
    }

    static EdgesControl()
    {
        AffectsRender<EdgesControl>(ViewModelProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (ViewModel == null) return;

        var pen = new Pen(Brushes.White, 2);

        foreach (var edge in ViewModel.Edges)
        {
            context.DrawLine(
                pen,
                new Point(
                    edge.VertexA.X,
                    edge.VertexA.Y
                ),
                new Point(
                    edge.VertexB.X,
                    edge.VertexB.Y
                ));
        }
    }
}