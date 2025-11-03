using System.Numerics;

namespace Core.Physics.Components;

/// <summary>
/// Component that defines movement parameters for an entity.
/// </summary>
public struct MovementComponent
{
    /// <summary>
    /// The direction of movement as a normalized vector.
    /// </summary>
    public Vector2 Direction;

    /// <summary>
    /// The speed of movement in units per second.
    /// </summary>
    public float Speed;

    /// <summary>
    /// The remaining distance to travel before stopping.
    /// </summary>
    public float RemainingDistance;
}