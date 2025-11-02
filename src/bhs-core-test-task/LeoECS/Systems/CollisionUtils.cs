using System.Numerics;

public static class CollisionUtils
{
    // 2D cross product
    private static float Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;

    // Segment-segment intersection
    public static bool DoSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 contactPoint)
    {
        contactPoint = Vector2.Zero;
        Vector2 r = p2 - p1;
        Vector2 s = q2 - q1;
        float rxs = Cross(r, s);
        Vector2 qp = q1 - p1;
        float qpxr = Cross(qp, r);

        if (Math.Abs(rxs) < 1e-6f && Math.Abs(qpxr) < 1e-6f)
            return false; // Collinear
        if (Math.Abs(rxs) < 1e-6f && Math.Abs(qpxr) >= 1e-6f)
            return false; // Parallel

        float t = Cross(qp, s) / rxs;
        float u = Cross(qp, r) / rxs;

        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            contactPoint = p1 + t * r;
            return true;
        }

        return false;
    }

    // Edge normal pointing outward
    public static Vector2 EdgeNormal(Vector2 a, Vector2 b)
    {
        Vector2 edge = b - a;
        Vector2 normal = new Vector2(-edge.Y, edge.X);
        return Vector2.Normalize(normal);
    }
}