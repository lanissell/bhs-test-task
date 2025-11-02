using System.Numerics;

/// <summary>
/// Edge defined by two vertices and a pivot point.
/// </summary>
public class Edge(Vector2 a, Vector2 b,  Vector2 pivot)
{
    public Vector2 VertexA = a;
    public Vector2 VertexB = b;
    public Vector2 Pivot = pivot;
}