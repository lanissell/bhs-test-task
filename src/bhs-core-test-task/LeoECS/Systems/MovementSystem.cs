using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems
{
    public class MovementSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter _movableEntities = world.Filter<SceneObjectComponent>().Inc<MovementComponent>().End();
        private readonly EcsPool<SceneObjectComponent> _sceneObjects = world.GetPool<SceneObjectComponent>();
        private readonly EcsPool<MovementComponent> _movements = world.GetPool<MovementComponent>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entity in _movableEntities)
            {
                ref var sceneObject = ref _sceneObjects.Get(entity);
                ref var movement = ref _movements.Get(entity);

                var velocity = movement.Direction * movement.Speed * Consts.FrameDeltaTime;

                Vector2 newPosition = sceneObject.SceneObject.Position + velocity;
                sceneObject.SceneObject.SetPosition(newPosition);
            }
        }
    }
}