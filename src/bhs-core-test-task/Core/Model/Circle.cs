using System.Drawing;
using System.Numerics;

namespace App.Model;

/// <summary>
/// Represents a circular scene object composed of multiple edge segments.
/// </summary>
public sealed class Circle : SceneObject
{
    /// <summary>
    /// Gets the radius of the circle.
    /// </summary>
    public float Radius { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Circle"/> class.
    /// </summary>
    /// <param name="center">The center position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="segments">The number of segments to approximate the circle.</param>
    public Circle(Vector2 center, float radius, int segments = 32)
    {
        SetPosition(center);

        Radius = radius;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = 2 * MathF.PI * i / segments;
            float angle2 = 2 * MathF.PI * (i + 1) / segments;

            Vector2 point1 = new Vector2(
                center.X + radius * MathF.Cos(angle1),
                center.Y + radius * MathF.Sin(angle1)
            );
            Vector2 point2 = new Vector2(
                center.X + radius * MathF.Cos(angle2),
                center.Y + radius * MathF.Sin(angle2)
            );

            Edges.Add(new Edge(point1, point2, center, Color.DarkOrange));
        }
    }
}