using System.Numerics;

/// <summary>
/// Represents the result of a collision detection check.
/// </summary>
public struct Collision
{
    /// <summary>
    /// Gets a value indicating whether an intersection occurred.
    /// </summary>
    public bool Intersects { get; init; }

    /// <summary>
    /// Gets the point of intersection in world coordinates.
    /// </summary>
    public Vector2 Point { get; init; }

    /// <summary>
    /// Gets the normal vector at the collision point.
    /// </summary>
    public Vector2 Normal { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Collision"/> struct.
    /// </summary>
    /// <param name="intersects">Indicates whether an intersection occurred.</param>
    /// <param name="point">The point of intersection.</param>
    /// <param name="normal">The normal vector at the collision point.</param>
    public Collision(bool intersects, Vector2 point = default, Vector2 normal = default)
    {
        Intersects = intersects;
        Point = point;
        Normal = normal;
    }
}