using System.Collections.Generic;

namespace Graphics.ViewModels;

public partial class EdgesViewModel : ViewModelBase
{
    public List<Edge> Edges { get; } = GameWorld.Instance.Edges;
}
