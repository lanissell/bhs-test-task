using App.Model;

namespace App.ViewModels;

/// <summary>
/// Graphics model for the scene that exposes all edges for rendering.
/// </summary>
public class SceneViewModel : ViewModelBase, IDisposable
{
    private readonly Scene scene;

    private readonly List<Edge> allEdges = new();

    /// <summary>
    /// Gets all edges from all scene objects.
    /// </summary>
    public List<Edge> AllEdges => allEdges;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneViewModel"/> class.
    /// </summary>
    /// <param name="scene">The scene to observe.</param>
    public SceneViewModel(Scene scene)
    {
        this.scene = scene;
        this.scene.SceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged()
    {
        allEdges.Clear();

        foreach (var sceneObject in scene.SceneObjects)
        {
            allEdges.AddRange(sceneObject.Edges);
        }

        OnPropertyChanged(nameof(AllEdges));
    }

    /// <summary>
    /// Disposes resources and unsubscribes from scene events.
    /// </summary>
    public void Dispose()
    {
        scene.SceneChanged -= OnSceneChanged;
    }
}