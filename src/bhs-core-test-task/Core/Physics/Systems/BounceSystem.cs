using System.Numerics;
using Core.Physics.Components;
using Core.Physics.Components.PhysicsComponent;
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

                MoveRemainingDistance(physicsObj, movement.Direction, movement.RemainingDistance);

                movement.RemainingDistance = 0f;
            }

            collision.OtherEntity = -1;
        }
    }

    /// <summary>
    /// Moves an entity along the reflected direction for the remaining distance.
    /// </summary>
    private void MoveRemainingDistance(
        IPhysicsSceneObjectComponent physicsObj,
        Vector2 direction,
        float distance)
    {
        Vector2 movement = direction * distance;
        Vector2 newPosition = physicsObj.SceneObject.Position + movement;
        physicsObj.SceneObject.SetPosition(newPosition);
    }
}