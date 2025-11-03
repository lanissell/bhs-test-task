using System.Numerics;
using Core.Physics.Components;
using Core.Physics.Components.PhysicsComponent;
using LeoECS;
using Leopotam.EcsLite;

namespace Core.Physics.Systems;

/// <summary>
/// System that handles bouncing behavior when entities collide with other objects.
/// Reflects the movement direction based on collision normal.
/// </summary>
/// <param name="world">The ECS world instance.</param>
public class BounceSystem(EcsWorld world) : IEcsRunSystem
{
    private readonly EcsFilter bouncingEntities = world.Filter<BounceComponent>().Inc<CollisionComponent>().Inc<MovementComponent>().Inc<PhysicsComponentWrapper>().End();
    private readonly EcsFilter collisionEntities = world.Filter<PhysicsComponentWrapper>().Inc<CollisionComponent>().End();

    private readonly EcsPool<CollisionComponent> collisionsPool = world.GetPool<CollisionComponent>();
    private readonly EcsPool<MovementComponent> movementsPool = world.GetPool<MovementComponent>();
    private readonly EcsPool<PhysicsComponentWrapper> physicsObjectsPool = world.GetPool<PhysicsComponentWrapper>();

    /// <summary>
    /// Executes the bounce logic for all entities with bounce capability that have collided.
    /// </summary>
    /// <param name="systems">The ECS systems instance.</param>
    public void Run(IEcsSystems systems)
    {
        foreach (int entity in bouncingEntities)
        {
            ref var collision = ref collisionsPool.Get(entity);

            if (collision.OtherEntity == -1)
                continue;

            ref var movement = ref movementsPool.Get(entity);

            Vector2 normal = collision.CollisionNormal;
            if (normal == Vector2.Zero)
                continue;

            // Calculate reflection: R = D - 2(D·N)N
            // Where D is the direction vector and N is the surface normal
            // This formula reflects the direction vector across the collision normal
            float dotProduct = Vector2.Dot(movement.Direction, normal);

            movement.Direction -= 2f * dotProduct * normal;
            movement.Direction = Vector2.Normalize(movement.Direction);

            // After bouncing, continue moving with the remaining distance
            // This ensures the entity uses its full movement budget even after collision
            if (movement.RemainingDistance > 0.0001f)
            {
                ref var wrapper = ref physicsObjectsPool.Get(entity);
                var physicsObj = wrapper.Component;

                MoveRemainingDistance(entity, physicsObj, movement.Direction, movement.RemainingDistance);

                movement.RemainingDistance = 0f;
            }

            collision.OtherEntity = -1;
        }
    }

    /// <summary>
    /// Moves an entity along the reflected direction for the remaining distance,
    /// using continuous collision detection to prevent tunneling through obstacles.
    /// </summary>
    private void MoveRemainingDistance(
        int entityA,
        IPhysicsSceneObjectComponent physicsObjA,
        Vector2 direction,
        float distance)
    {
        float remaining = distance;

        // Break movement into small steps to detect collisions continuously
        while (remaining > 0.0001f)
        {
            float step = MathF.Min(Consts.Collision.MaxStepLength, remaining);
            var startPos = physicsObjA.SceneObject.Position;
            var targetPos = startPos + direction * step;

            physicsObjA.SceneObject.SetPosition(targetPos);

            bool collided = false;
            // Check collision with all other entities
            foreach (int entityB in collisionEntities)
            {
                if (entityB == entityA) continue;

                ref var wrapperB = ref physicsObjectsPool.Get(entityB);
                var physicsObjB = wrapperB.Component;

                if (CheckCollision(physicsObjA, physicsObjB, out Vector2 normal))
                {
                    // Binary search to find exact contact point between start and target positions
                    Vector2 contact = FindContactPosition(physicsObjA, physicsObjB, startPos, targetPos, out normal);

                    // Move back slightly from contact point to prevent overlap
                    physicsObjA.SceneObject.SetPosition(contact - normal * Consts.Collision.BackoffEpsilon);

                    collided = true;
                    break;
                }
            }

            if (collided)
                break;

            remaining -= step;
        }
    }

    private bool CheckCollision(IPhysicsSceneObjectComponent a, IPhysicsSceneObjectComponent b, out Vector2 normal)
    {
        normal = Vector2.Zero;

        foreach (var edgeA in a.SceneObject.Edges)
        {
            var crossingResult = b.CheckCollision(edgeA);

            if (crossingResult.Intersects)
            {
                normal = crossingResult.Normal;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Uses binary search to find the exact contact position between two colliding objects.
    /// This prevents objects from penetrating each other by finding the last non-colliding position.
    /// </summary>
    private Vector2 FindContactPosition(IPhysicsSceneObjectComponent a, IPhysicsSceneObjectComponent b, Vector2 from, Vector2 to, out Vector2 normal)
    {
        normal = Vector2.Zero;
        Vector2 lo = from;  // Known non-colliding position
        Vector2 hi = to;    // Known colliding position

        // Binary search: narrow down the interval to find exact contact point
        for (int i = 0; i < Consts.Collision.BinarySearchIterations; i++)
        {
            Vector2 mid = (lo + hi) * 0.5f;
            a.SceneObject.SetPosition(mid);

            if (CheckCollision(a, b, out Vector2 n))
            {
                normal = n;
                hi = mid;  // Collision detected, search lower half
            }
            else
            {
                lo = mid;  // No collision, search upper half
            }
        }

        // Final check at the last known non-colliding position
        a.SceneObject.SetPosition(lo);
        if (CheckCollision(a, b, out Vector2 n2))
            normal = n2;

        return lo;
    }
}