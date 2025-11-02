using System.Numerics;

public struct Collision
{
    public bool Intersects { get; init; }

    public Vector2 Point { get; init; }

    public Vector2 Normal { get; init; }

    public Collision(bool intersects, Vector2 point = default, Vector2 normal = default)
    {
        Intersects = intersects;
        Point = point;
        Normal = normal;
    }
}