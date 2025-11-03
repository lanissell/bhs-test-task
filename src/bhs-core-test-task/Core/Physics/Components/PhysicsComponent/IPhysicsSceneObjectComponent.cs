using App.Model;

namespace Core.Physics.Components.PhysicsComponent;

/// <summary>
/// Interface for physics components that can detect collisions with scene object edges.
/// </summary>
public interface IPhysicsSceneObjectComponent
{
    /// <summary>
    /// Gets the scene object associated with this physics component.
    /// </summary>
    SceneObject SceneObject {  get;  }

    /// <summary>
    /// Checks for collision between this component and the given edge.
    /// </summary>
    /// <param name="edge">The edge to check for collision against.</param>
    /// <returns>A collision result containing intersection data if collision occurred.</returns>
    Collision CheckCollision(Edge edge);
}