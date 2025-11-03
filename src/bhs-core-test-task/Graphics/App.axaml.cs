using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using App;
using App.GameLoop;
using App.ViewModels;
using Avalonia.Markup.Xaml;
using Graphics.Views;

namespace Graphics;

public class App : Application
{
    private Scene scene;
    private SceneViewModel sceneViewModel;
    private GameLoop gameLoop;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        scene = new Scene();
        sceneViewModel = new SceneViewModel(scene);

        gameLoop = new GameLoop();

        gameLoop.AddUpdateBehaviour(scene);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            desktop.MainWindow = new MainWindow
            {
                DataContext = sceneViewModel,
            };

            desktop.Exit += (_, _) =>
            {
                sceneViewModel.Dispose();
                gameLoop.Dispose();
            };

            gameLoop.RunLoop();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}