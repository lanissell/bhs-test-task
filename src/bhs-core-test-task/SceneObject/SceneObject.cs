using System.Numerics;

public abstract class SceneObject
{
    public Vector2 Position { get; private set; }

    public Edge[] Edges { get; init; }

    public void SetPosition(Vector2 newPosition)
    {
        var delta = newPosition - Position;
        Position = newPosition;

        for(int index = 0; index < Edges.Length; index++)
        {
            Edge edge = Edges[index];
            edge.VertexA += delta;
            edge.VertexB += delta;
            Edges[index] = edge;
        }
    }
}
