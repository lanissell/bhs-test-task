using System.Numerics;

namespace PhysicsEngine.Colliders;

public class CirclePhysicsComponent(Circle circle) : IPhysicsSceneObjectComponent
{
    public SceneObject SceneObject => circle;

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