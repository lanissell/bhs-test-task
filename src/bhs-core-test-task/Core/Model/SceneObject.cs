using System.Numerics;

public abstract class SceneObject
{
    public Vector2 Position { get; private set; }

    public List<Edge> Edges { get; } = new List<Edge>();

    public void SetPosition(Vector2 newPosition)
    {
        var delta = newPosition - Position;
        Position = newPosition;

        foreach (Edge edge in Edges)
        {
            edge.VertexA += delta;
            edge.VertexB += delta;
            edge.Pivot += delta;
        }
    }
}