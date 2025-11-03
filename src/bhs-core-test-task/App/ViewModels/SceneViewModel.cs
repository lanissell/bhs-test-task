using System.Collections.Generic;

using App;

namespace Graphics.ViewModels;

public class SceneViewModel : ViewModelBase, IDisposable
{
    private readonly Scene scene;

    private readonly List<Edge> allEdges = new();

    public List<Edge> AllEdges => allEdges;

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

    public void Dispose()
    {
        scene.SceneChanged -= OnSceneChanged;
    }
}