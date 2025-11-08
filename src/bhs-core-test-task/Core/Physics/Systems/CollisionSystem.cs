using System.Numerics;
using Core.Physics.Components;
using Core.Physics.Components.PhysicsComponent;
using LeoECS;
using Leopotam.EcsLite;

namespace Core.Physics.Systems;

/// <summary>
/// System that detects and processes collisions between entities using continuous collision detection.
/// </summary>
/// <param name="world">The ECS world instance.</param>
public class CollisionSystem(EcsWorld world) : IEcsRunSystem
{
    private readonly EcsFilter movableEntities = world.Filter<PhysicsComponentWrapper>().Inc<MovementComponent>().Inc<CollisionComponent>().End();
    private readonly EcsFilter collisionEntities = world.Filter<PhysicsComponentWrapper>().Inc<CollisionComponent>().End();

    private readonly EcsPool<PhysicsComponentWrapper> physicsObjectsPool = world.GetPool<PhysicsComponentWrapper>();
    private readonly EcsPool<CollisionComponent> collisionsPool = world.GetPool<CollisionComponent>();
    private readonly EcsPool<MovementComponent> movementsPool = world.GetPool<MovementComponent>();

    private struct CollisionInfo
    {
        public int HitEntity;
        public Vector2 Normal;
        public Vector2 ContactPoint;
    }

    private struct CcdResult
    {
        public bool Collided;
        public int HitEntity;
        public Vector2 HitNormal;
        public float RemainingDistance;
    }

    /// <summary>
    /// Executes collision detection logic for all movable entities.
    /// </summary>
    /// <param name="systems">The ECS systems instance.</param>
    public void Run(IEcsSystems systems)
    {
        foreach (int entityA in movableEntities)
        {
            ref var wrapperA = ref physicsObjectsPool.Get(entityA);
            var physicsObjA = wrapperA.Component;

            ref var collisionA = ref collisionsPool.Get(entityA);
            ref var movementA = ref movementsPool.Get(entityA);

            collisionA.OtherEntity = -1;
            collisionA.CollisionNormal = Vector2.Zero;
            movementA.RemainingDistance = 0f;

            Vector2 delta = CalculateMovementDelta(ref movementA);
            if (delta == Vector2.Zero)
                return;

            var result = PerformContinuousCollisionDetection(entityA, physicsObjA, delta);

            if (result.Collided)
            {
                collisionA.OtherEntity = result.HitEntity;
                collisionA.CollisionNormal = result.HitNormal;

                movementA.RemainingDistance = result.RemainingDistance;
            }
        }
    }

    private static Vector2 CalculateMovementDelta(ref MovementComponent movement)
    {
        Vector2 dir = movement.Direction;
        if (dir != Vector2.Zero)
            dir = Vector2.Normalize(dir);
        return dir * movement.Speed;
    }

    private CcdResult PerformContinuousCollisionDetection(
        int entityA,
        IPhysicsSceneObjectComponent physicsObjA,
        Vector2 delta)
    {
        var result = new CcdResult();

        var endPos = physicsObjA.SceneObject.Position;
        var startPos = endPos - delta;

        float totalDistance = delta.Length();
        if (totalDistance <= 0f)
            return result;

        // Divide movement into discrete steps to prevent tunneling through thin objects
        // More steps = more accurate collision detection but higher computational cost
        int steps = Math.Max(1, (int)MathF.Ceiling(totalDistance / Consts.Collision.MaxStepLength));
        Vector2 dirNorm = delta / totalDistance;
        Vector2 stepDelta = dirNorm * (totalDistance / steps);

        // Start from the beginning of the movement path
        physicsObjA.SceneObject.SetPosition(startPos);

        float travelled = 0f;
        // Sweep through the movement path step by step
        for (int s = 0; s < steps; s++)
        {
            var prevPos = physicsObjA.SceneObject.Position;
            var newPos = prevPos + stepDelta;
            physicsObjA.SceneObject.SetPosition(newPos);

            // Check if this step caused a collision
            if (TryFindCollision(entityA, physicsObjA, prevPos, newPos, out var collisionInfo))
            {
                // Calculate how far we traveled before hitting
                travelled += (collisionInfo.ContactPoint - prevPos).Length();

                result.Collided = true;
                result.HitEntity = collisionInfo.HitEntity;
                result.HitNormal = collisionInfo.Normal;
                // Store remaining distance for bounce system to use
                result.RemainingDistance = totalDistance - travelled;

                return result;
            }

            travelled += stepDelta.Length();
        }

        // No collision detected, set final position
        physicsObjA.SceneObject.SetPosition(endPos);
        return result;
    }

    private bool TryFindCollision(
        int entityA,
        IPhysicsSceneObjectComponent physicsObjA,
        Vector2 prevPos,
        Vector2 newPos,
        out CollisionInfo collisionInfo)
    {
        collisionInfo = default;

        // Check against all collidable entities
        foreach (int entityB in collisionEntities)
        {
            if (entityA == entityB) continue;

            ref var wrapperB = ref physicsObjectsPool.Get(entityB);
            var physicsObjB = wrapperB.Component;

            if (CheckCollision(physicsObjA, physicsObjB, out Vector2 normal))
            {
                // Use binary search to find precise contact point
                // This prevents objects from overlapping and improves accuracy
                Vector2 contact = FindContactPosition(physicsObjA, physicsObjB, prevPos, newPos, out normal);

                // Push back slightly to ensure no overlap
                physicsObjA.SceneObject.SetPosition(contact - normal * Consts.Collision.BackoffEpsilon);

                collisionInfo.HitEntity = entityB;
                collisionInfo.Normal = normal;
                collisionInfo.ContactPoint = contact;
                return true;
            }
        }

        return false;
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
    /// This algorithm efficiently narrows down the collision point by repeatedly checking
    /// the midpoint between a known non-colliding and colliding position.
    /// </summary>
    private Vector2 FindContactPosition(IPhysicsSceneObjectComponent a, IPhysicsSceneObjectComponent b, Vector2 from, Vector2 to, out Vector2 normal)
    {
        normal = Vector2.Zero;
        Vector2 lo = from;  // Lower bound: last known non-colliding position
        Vector2 hi = to;    // Upper bound: first known colliding position

        // Iteratively narrow the search space by half each iteration
        for (int i = 0; i < Consts.Collision.BinarySearchIterations; i++)
        {
            Vector2 mid = (lo + hi) * 0.5f;
            a.SceneObject.SetPosition(mid);

            if (CheckCollision(a, b, out Vector2 n))
            {
                normal = n;
                hi = mid;  // Midpoint collides, search in first half
            }
            else
            {
                lo = mid;  // Midpoint is safe, search in second half
            }
        }

        // Set to the last non-colliding position found
        a.SceneObject.SetPosition(lo);
        if (CheckCollision(a, b, out Vector2 n2))
            normal = n2;

        return lo;
    }
}