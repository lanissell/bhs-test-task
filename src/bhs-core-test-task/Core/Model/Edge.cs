using System.Drawing;
using System.Numerics;

namespace App.Model;

/// <summary>
/// Represents an edge defined by two vertices and a pivot point.
/// </summary>
/// <param name="a">First vertex of the edge.</param>
/// <param name="b">Second vertex of the edge.</param>
/// <param name="pivot">Pivot point of the edge.</param>
/// <param name="color">Color of the edge.</param>
public class Edge(Vector2 a, Vector2 b, Vector2 pivot, Color color)
{
    /// <summary>
    /// First vertex of the edge.
    /// </summary>
    public Vector2 VertexA = a;

    /// <summary>
    /// Second vertex of the edge.
    /// </summary>
    public Vector2 VertexB = b;

    /// <summary>
    /// Pivot point of the edge.
    /// </summary>
    public Vector2 Pivot = pivot;

    /// <summary>
    /// Color of the edge.
    /// </summary>
    public Color Color = color;
}