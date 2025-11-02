using System.Numerics;

public struct CrossingResult
{
    public bool Intersects { get; set; }
    public Vector2 Point { get; set; }
    public Vector2 Normal { get; set; }

    public CrossingResult(bool intersects, Vector2 point = default, Vector2 normal = default)
    {
        Intersects = intersects;
        Point = point;
        Normal = normal;
    }
}