using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Graphics.ViewModels;
using System.ComponentModel;

namespace Graphics.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SceneViewModel viewModel)
        {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            RenderEdges(viewModel);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SceneViewModel.AllEdges) && sender is SceneViewModel viewModel)
        {
            Dispatcher.UIThread.InvokeAsync(() => RenderEdges(viewModel));
        }
    }

    private void RenderEdges(SceneViewModel viewModel)
    {
        RenderCanvas.Children.Clear();

        foreach (var edge in viewModel.AllEdges)
        {
            var line = new Avalonia.Controls.Shapes.Line
            {
                StartPoint = new Avalonia.Point(edge.VertexA.X, edge.VertexA.Y),
                EndPoint = new Avalonia.Point(edge.VertexB.X, edge.VertexB.Y),
                Stroke = new SolidColorBrush(
                    Avalonia.Media.Color.FromArgb(
                        edge.Color.A,
                        edge.Color.R,
                        edge.Color.G,
                        edge.Color.B)),
                StrokeThickness = 3
            };

            RenderCanvas.Children.Add(line);
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is SceneViewModel viewModel)
        {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        DataContextChanged -= OnDataContextChanged;

        base.OnClosing(e);
    }
}