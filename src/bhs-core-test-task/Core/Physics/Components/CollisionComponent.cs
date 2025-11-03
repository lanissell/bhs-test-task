using System.Numerics;

namespace Core.Physics.Components;

/// <summary>
/// Component that stores collision information for an entity.
/// </summary>
public struct CollisionComponent
{
    /// <summary>
    /// The entity ID of the other colliding entity.
    /// </summary>
    public int OtherEntity;

    /// <summary>
    /// The normal vector at the collision point.
    /// </summary>
    public Vector2 CollisionNormal;

    /// <summary>
    /// The frame number when the collision occurred.
    /// </summary>
    public int CollisionFrame;
}