using System.Numerics;
using Core.Physics.Components;
using Core.Physics.Components.PhysicsComponent;
using Leopotam.EcsLite;

namespace LeoECS.Systems
{
    /// <summary>
    /// System that moves entities based on their movement components.
    /// Updates entity positions each frame according to direction and speed.
    /// </summary>
    /// <param name="world">The ECS world instance.</param>
    public class MovementSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter movableEntities = world.Filter<PhysicsComponentWrapper>().Inc<MovementComponent>().End();

        private readonly EcsPool<PhysicsComponentWrapper> physicsObjectsPool = world.GetPool<PhysicsComponentWrapper>();
        private readonly EcsPool<MovementComponent> movementComponentsPool = world.GetPool<MovementComponent>();

        /// <summary>
        /// Executes movement logic for all movable entities.
        /// </summary>
        /// <param name="systems">The ECS systems instance.</param>
        public void Run(IEcsSystems systems)
        {
            foreach (int entity in movableEntities)
            {
                ref var physics = ref physicsObjectsPool.Get(entity);
                ref var movement = ref movementComponentsPool.Get(entity);

                Vector2 dir = movement.Direction;
                if (dir != Vector2.Zero)
                    dir = Vector2.Normalize(dir);

                Vector2 delta = dir * movement.Speed;
                var sceneObject = physics.Component.SceneObject;
                sceneObject.SetPosition(sceneObject.Position + delta);
            }
        }
    }
}