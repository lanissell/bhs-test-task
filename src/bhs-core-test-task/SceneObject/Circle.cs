using System.Numerics;

public sealed class Circle : SceneObject
{
    private readonly float radius;

    public Circle(Vector2 center, float radius, int segments = 32)
    {
        SetPosition(center);

        this.radius = radius;

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

            Edges.Add(new Edge(point1, point2, Position));
        }
    }

    public override CrossingResult GetEdgeCrossing(Edge edge)
    {
        var edgeRadius = Vector2.Distance(edge.Pivot, edge.VertexA );

        Vector2 diff = edge.Pivot - Position;
        float distance = diff.Length();
        float minDistance = radius + edgeRadius;

        if (distance >= minDistance)
            return new CrossingResult(false);

        // Normal points from this circle towards the other circle
        Vector2 cNormal = Vector2.Normalize(diff) ;

        // Intersection point on the surface of this circle
        Vector2 cIntersectionPoint = Position + cNormal * radius;

        return new CrossingResult(true, cIntersectionPoint, cNormal);
    }
}