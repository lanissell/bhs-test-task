using System.Drawing;
using System.Numerics;

namespace App.Model;

/// <summary>
/// Represents a straight line segment scene object.
/// </summary>
public sealed class Line : SceneObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Line"/> class.
    /// </summary>
    /// <param name="start">Starting point of the line.</param>
    /// <param name="end">Ending point of the line.</param>
    public Line(Vector2 start, Vector2 end)
    {
        SetPosition((start + end) / 2);
        Edges.Add(new Edge(start, end, Position, Color.Azure));
    }
}