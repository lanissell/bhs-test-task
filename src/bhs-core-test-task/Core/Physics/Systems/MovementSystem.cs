using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;
using PhysicsEngine.Colliders;

namespace LeoECS.Systems
{
    public class MovementSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter movableEntities = world.Filter<PhysicsComponentWrapper>().Inc<MovementComponent>().End();
        private readonly EcsPool<PhysicsComponentWrapper> physicsObjects = world.GetPool<PhysicsComponentWrapper>();
        private readonly EcsPool<MovementComponent> movementComponents = world.GetPool<MovementComponent>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in movableEntities)
            {
                ref var physics = ref physicsObjects.Get(entity);
                ref var movement = ref movementComponents.Get(entity);

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