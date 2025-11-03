using System.Numerics;
using App.Model;

namespace Core.Physics.Components.PhysicsComponent;

/// <summary>
/// Physics component for line scene objects that handles collision detection with edges.
/// </summary>
/// <param name="line">The line scene object associated with this physics component.</param>
public class LinePhysicsComponent (Line line): IPhysicsSceneObjectComponent
{
    /// <summary>
    /// Gets the scene object associated with this physics component.
    /// </summary>
    public SceneObject SceneObject => line;

    /// <summary>
    /// Checks for collision between the given edge and this line's edges.
    /// </summary>
    /// <param name="edge">The edge to check for collision against.</param>
    /// <returns>A collision result containing intersection data if collision occurred.</returns>
    public Collision CheckCollision(Edge edge)
    {
        var edges = SceneObject.Edges;
        return edges.Count > 0 ? EdgesIntersect(edge, edges[0]) : new Collision(false);
    }

    private static Collision EdgesIntersect(Edge edge1, Edge edge2)
    {
        Vector2 p1 = edge1.VertexA;
        Vector2 p2 = edge1.VertexB;
        Vector2 p3 = edge2.VertexA;
        Vector2 p4 = edge2.VertexB;

        float d = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);

        if (MathF.Abs(d) < 1e-10f)
            return new Collision(false);

        float t = ((p3.X - p1.X) * (p4.Y - p3.Y) - (p3.Y - p1.Y) * (p4.X - p3.X)) / d;
        float u = ((p3.X - p1.X) * (p2.Y - p1.Y) - (p3.Y - p1.Y) * (p2.X - p1.X)) / d;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            Vector2 intersectionPoint = p1 + t * (p2 - p1);

            Vector2 edge2Direction = Vector2.Normalize(p4 - p3);
            Vector2 normal = new Vector2(-edge2Direction.Y, edge2Direction.X);

            return new Collision(true, intersectionPoint, normal);
        }

        return new Collision(false);
    }
}