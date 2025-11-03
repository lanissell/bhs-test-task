using System.Numerics;
using App.Model;

namespace Core.Physics.Components.PhysicsComponent;

/// <summary>
/// Physics component for circle scene objects that handles collision detection with edges.
/// </summary>
/// <param name="circle">The circle scene object associated with this physics component.</param>
public class CirclePhysicsComponent(Circle circle) : IPhysicsSceneObjectComponent
{
    /// <summary>
    /// Gets the scene object associated with this physics component.
    /// </summary>
    public SceneObject SceneObject => circle;

    /// <summary>
    /// Checks for collision between the given edge and this circle.
    /// </summary>
    /// <param name="edge">The edge to check for collision against.</param>
    /// <returns>A collision result containing intersection data if collision occurred.</returns>
    public  Collision CheckCollision(Edge edge)
    {
        var edgeRadius = Vector2.Distance(edge.Pivot, edge.VertexA );

        Vector2 diff = edge.Pivot - circle.Position;
        float distance = diff.Length();
        float minDistance = circle.Radius + edgeRadius;

        if (distance >= minDistance)
            return new Collision(false);

        // Normal points from this circle towards the other circle
        Vector2 cNormal = Vector2.Normalize(diff) ;

        // Intersection point on the surface of this circle
        Vector2 cIntersectionPoint = circle.Position + cNormal * circle.Radius;

        return new Collision(true, cIntersectionPoint, cNormal);
    }
}