using System.Drawing;
using System.Numerics;

public sealed class Line : SceneObject
{
    public Line(Vector2 start, Vector2 end)
    {
        SetPosition((start + end) / 2);
        Edges.Add(new Edge(start, end, Position, Color.Azure));
    }
}