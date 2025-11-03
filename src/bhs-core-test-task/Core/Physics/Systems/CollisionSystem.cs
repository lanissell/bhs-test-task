using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;
using PhysicsEngine.Colliders;

namespace LeoECS.Systems
{
    public class CollisionSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter movableEntities = world.Filter<PhysicsComponentWrapper>().Inc<MovementComponent>().Inc<CollisionComponent>().End();
        private readonly EcsFilter collisionEntities = world.Filter<PhysicsComponentWrapper>().Inc<CollisionComponent>().End();

        private readonly EcsPool<PhysicsComponentWrapper> physicsObjects = world.GetPool<PhysicsComponentWrapper>();
        private readonly EcsPool<CollisionComponent> collisions = world.GetPool<CollisionComponent>();

        private int frame;

        public void Run(IEcsSystems systems)
        {
            unchecked { frame++; }

            foreach (int entityA in movableEntities)
            {
                ref var wrapperA = ref physicsObjects.Get(entityA);
                var physicsObjA = wrapperA.Component;

                ref var collisionA = ref collisions.Get(entityA);

                if (collisionA.CollisionFrame != frame)
                {
                    // Reset collision info for this frame
                    collisionA.OtherEntity = -1;
                    collisionA.CollisionNormal = Vector2.Zero;
                    collisionA.CollisionFrame = frame;
                }

                foreach (int entityB in collisionEntities)
                {
                    if (entityA == entityB) continue;

                    ref var wrapperB = ref physicsObjects.Get(entityB);
                    var physicsObjB = wrapperB.Component;

                    if (CheckCollision(physicsObjA, physicsObjB, out Vector2 normal))
                    {
                        collisionA.OtherEntity = entityB;
                        collisionA.CollisionNormal = normal;
                        collisionA.CollisionFrame = frame;
                        break;
                    }
                }
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
    }
}