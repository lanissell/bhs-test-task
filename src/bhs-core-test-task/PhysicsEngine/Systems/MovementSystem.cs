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
                ref var physicsObj = ref physicsObjects.Get(entity);
                ref var movement = ref movementComponents.Get(entity);

                var velocity = movement.Direction * movement.Speed * Consts.FrameDeltaTime;

                var sceneObject = physicsObj.Component.SceneObject;
                Vector2 newPosition = sceneObject.Position + velocity;

                sceneObject.SetPosition(newPosition);
            }
        }
    }
}