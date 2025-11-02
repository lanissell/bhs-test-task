using System.Numerics;

public abstract class SceneObject
{
    public Vector2 Position { get; private set; }

    public List<Edge> Edges { get; } = new List<Edge>();

    public void SetPosition(Vector2 newPosition)
    {
        var delta = newPosition - Position;
        Position = newPosition;

        for(int index = 0; index < Edges.Count; index++)
        {
            Edge edge = Edges[index];
            edge.VertexA += delta;
            edge.VertexB += delta;
            Edges[index] = edge;
        }
    }
}