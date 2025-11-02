using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems
{
    public class CollisionSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter _movableEntities = world.Filter<SceneObjectComponent>().Inc<MovementComponent>().Inc<CollisionComponent>().End();
        private readonly EcsFilter _allEntities = world.Filter<SceneObjectComponent>().Inc<CollisionComponent>().End();

        private readonly EcsPool<SceneObjectComponent> _sceneObjects = world.GetPool<SceneObjectComponent>();
        private readonly EcsPool<CollisionComponent> _collisions = world.GetPool<CollisionComponent>();

        private int frame;

        public void Run(IEcsSystems systems)
        {
            frame++;

            foreach (int entityA in _movableEntities)
            {
                ref var sceneA = ref _sceneObjects.Get(entityA);

                ref var collisionA = ref _collisions.Get(entityA);

                if (collisionA.CollisionFrame != frame)
                {
                    // Reset collision info for this frame
                    collisionA.OtherEntity = -1;
                    collisionA.CollisionNormal = Vector2.Zero;
                    collisionA.CollisionFrame = frame;
                }

                foreach (int entityB in _allEntities)
                {
                    if (entityA == entityB) continue;

                    ref var sceneB = ref _sceneObjects.Get(entityB);

                    if (CheckCollision(sceneA.SceneObject, sceneB.SceneObject, out Vector2 normal))
                    {
                        collisionA.OtherEntity = entityB;
                        collisionA.CollisionNormal = normal;
                        collisionA.CollisionFrame = frame;
                        break;
                    }
                }
            }
        }

        private bool CheckCollision(SceneObject a, SceneObject b, out Vector2 normal)
        {
            normal = Vector2.Zero;

            foreach (var edgeA in a.Edges)
            {
                var crossingResult = b.GetEdgeCrossing(edgeA);

                if (crossingResult.Intersects)
                {
                    Console.WriteLine(b.GetType().Name + " collided with " + a.GetType().Name);
                    normal = crossingResult.Normal;
                    return true;
                }
            }
            return false;
        }
    }
}