using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems
{
    public class BounceSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter _bouncingEntities = world.Filter<BounceComponent>().Inc<CollisionComponent>().Inc<MovementComponent>().End();
        private readonly EcsPool<CollisionComponent> _collisions = world.GetPool<CollisionComponent>();
        private readonly EcsPool<MovementComponent> _movements = world.GetPool<MovementComponent>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _bouncingEntities)
            {
                ref var collision = ref _collisions.Get(entity);

                if (collision.OtherEntity == -1)
                    continue; // No collision to process

                ref var movement = ref _movements.Get(entity);

                Vector2 normal = collision.CollisionNormal;
                if (normal == Vector2.Zero)
                    continue;

                float dotProduct = Vector2.Dot(movement.Direction, normal);

                // Reflect direction: d_new = d - 2 * (d · n) * n
                movement.Direction -= 2f * dotProduct * normal;
                movement.Direction = Vector2.Normalize(movement.Direction);
            }
        }
    }
}