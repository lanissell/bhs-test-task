using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems;

public class BounceSystem(EcsWorld world) : IEcsRunSystem
{
    private readonly EcsFilter bounceEntities = world.Filter<SceneObjectComponent>().Inc<MovementComponent>().Inc<BounceComponent>().End();
    private readonly EcsFilter allSceneObjects = world.Filter<SceneObjectComponent>().End();

    private readonly EcsPool<SceneObjectComponent> sceneObjectComponents = world.GetPool<SceneObjectComponent>();
    private readonly EcsPool<MovementComponent> movableComponents = world.GetPool<MovementComponent>();
    private readonly EcsPool<BounceComponent> bouncinessComponents = world.GetPool<BounceComponent>();

    public void Run(IEcsSystems systems)
    {
        var bounceEntitiesArray = bounceEntities.GetRawEntities();
        
        // Check each bouncy entity against all other scene objects
        for (int i = 0; i < bounceEntities.GetEntitiesCount(); i++)
        {
            int entityA = bounceEntitiesArray[i];
            ref var sceneObjectA = ref sceneObjectComponents.Get(entityA);
            ref var movementA = ref movableComponents.Get(entityA);
            ref var bounceA = ref bouncinessComponents.Get(entityA);

            foreach (int entityB in allSceneObjects)
            {
                // Don't check collision with self
                if (entityA == entityB) continue;

                ref var sceneObjectB = ref sceneObjectComponents.Get(entityB);

                // Check for collision between the two objects
                if (CheckCollision(sceneObjectA.SceneObject, sceneObjectB.SceneObject, out Vector2 collisionNormal))
                {
                    // Apply bounce to entityA
                    ApplyBounce(ref movementA, collisionNormal, bounceA.Bounciness);
                    
                    // If entityB also has movement and bounce components, make it bounce too
                    if (movableComponents.Has(entityB) && bouncinessComponents.Has(entityB))
                    {
                        ref var movementB = ref movableComponents.Get(entityB);
                        ref var bounceB = ref bouncinessComponents.Get(entityB);
                        ApplyBounce(ref movementB, -collisionNormal, bounceB.Bounciness);
                    }
                }
            }
        }
    }

    private bool CheckCollision(SceneObject a, SceneObject b, out Vector2 collisionNormal)
    {
        collisionNormal = Vector2.Zero;
        
        // Check each edge of object A against each edge of object B
        foreach (var edgeA in a.Edges)
        {
            foreach (var edgeB in b.Edges)
            {
                if (CheckEdgeCollision(edgeA, edgeB, out collisionNormal))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private bool CheckEdgeCollision(Edge a, Edge b, out Vector2 collisionNormal)
    {
        collisionNormal = Vector2.Zero;
        
        // Simple edge-edge collision detection
        Vector2 a1 = a.VertexA;
        Vector2 a2 = a.VertexB;
        Vector2 b1 = b.VertexA;
        Vector2 b2 = b.VertexB;

        // Calculate direction vectors
        Vector2 aDir = a2 - a1;
        Vector2 bDir = b2 - b1;

        // Check if lines are parallel
        float cross = aDir.X * bDir.Y - aDir.Y * bDir.X;
        if (Math.Abs(cross) < 0.0001f)
            return false;

        // Calculate intersection parameters
        Vector2 aToB = b1 - a1;
        float t = (aToB.X * bDir.Y - aToB.Y * bDir.X) / cross;
        float u = (aToB.X * aDir.Y - aToB.Y * aDir.X) / cross;

        // Check if intersection point is within both line segments
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            // Calculate collision normal (perpendicular to edge A)
            collisionNormal = new Vector2(-aDir.Y, aDir.X);
            collisionNormal = Vector2.Normalize(collisionNormal);
            return true;
        }

        return false;
    }

    private void ApplyBounce(ref MovementComponent movement, Vector2 normal, float bounciness)
    {
        // Reflect the movement direction across the collision normal
        Vector2 incident = movement.Direction;
        movement.Direction = Vector2.Reflect(incident, normal);
        
        // Apply bounciness factor to maintain or reduce energy
        movement.Speed *= bounciness;
        
        // Ensure minimum speed to prevent objects getting stuck
        if (movement.Speed < 0.1f)
            movement.Speed = 0.1f;
    }
}