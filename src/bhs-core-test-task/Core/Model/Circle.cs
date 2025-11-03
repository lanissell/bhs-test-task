using System.Drawing;
using System.Numerics;

public sealed class Circle : SceneObject
{
    public float Radius { get; }

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