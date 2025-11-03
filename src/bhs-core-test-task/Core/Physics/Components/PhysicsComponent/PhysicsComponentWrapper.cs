namespace Core.Physics.Components.PhysicsComponent;

/// <summary>
/// Wrapper struct that holds a reference to a physics component for ECS integration.
/// </summary>
public struct PhysicsComponentWrapper
{
    /// <summary>
    /// Gets or sets the physics component instance.
    /// </summary>
    public IPhysicsSceneObjectComponent Component { get; set; }
}