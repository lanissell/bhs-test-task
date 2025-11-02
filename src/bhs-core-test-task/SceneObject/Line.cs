using System.Numerics;

public sealed class Line : SceneObject
{
    public Line(Vector2 start, Vector2 end)
    {
        SetPosition((start + end) / 2);
        Edges.Add(new Edge(start, end, Position));
    }

    public override CrossingResult GetEdgeCrossing(Edge edge)
    {
        return Edges.Count > 0 ? EdgesIntersect(edge, Edges[0]) : new CrossingResult(false);
    }

    private static CrossingResult EdgesIntersect(Edge edge1, Edge edge2)
    {
        Vector2 p1 = edge1.VertexA;
        Vector2 p2 = edge1.VertexB;
        Vector2 p3 = edge2.VertexA;
        Vector2 p4 = edge2.VertexB;

        float d = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);

        if (MathF.Abs(d) < 1e-10f)
            return new CrossingResult(false);

        float t = ((p3.X - p1.X) * (p4.Y - p3.Y) - (p3.Y - p1.Y) * (p4.X - p3.X)) / d;
        float u = ((p3.X - p1.X) * (p2.Y - p1.Y) - (p3.Y - p1.Y) * (p2.X - p1.X)) / d;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            Vector2 intersectionPoint = p1 + t * (p2 - p1);

            Vector2 edge2Direction = Vector2.Normalize(p4 - p3);
            Vector2 normal = new Vector2(-edge2Direction.Y, edge2Direction.X);

            return new CrossingResult(true, intersectionPoint, normal);
        }

        return new CrossingResult(false);
    }
}