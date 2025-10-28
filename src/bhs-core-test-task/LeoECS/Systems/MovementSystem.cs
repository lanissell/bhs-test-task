using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems;

public class MovementSystem(EcsWorld world) : IEcsRunSystem
{
    private readonly EcsFilter movableEntities = world.Filter<SceneObjectComponent>().Inc<MovementComponent>().End();

    private readonly EcsPool<SceneObjectComponent> sceneObjectComponents = world.GetPool<SceneObjectComponent>();
    private readonly EcsPool<MovementComponent> movableComponents = world.GetPool<MovementComponent>();

    public void Run(IEcsSystems systems)
    {
        foreach (int entity in movableEntities)
        {
            ref var movementComponent = ref movableComponents.Get(entity);
            ref var sceneObjectComponent = ref sceneObjectComponents.Get(entity);

            var sceneObject = sceneObjectComponent.SceneObject;

            var newPosition = sceneObject.Position + movementComponent.Direction * movementComponent.Speed;
            sceneObject.SetPosition(newPosition);
        }
    }
}
