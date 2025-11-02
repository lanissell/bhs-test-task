using Avalonia.Controls;
using Avalonia.Threading;

namespace Graphics.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var timer = new System.Timers.Timer(1);
        timer.Elapsed += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Content is DrawingControl drawingControl)
                {
                    drawingControl.InvalidateVisual();
                }
            });
        };
        timer.Start();
    }
}