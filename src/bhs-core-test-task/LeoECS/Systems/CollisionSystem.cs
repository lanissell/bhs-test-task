using System.Numerics;
using LeoECS.Components;
using Leopotam.EcsLite;

namespace LeoECS.Systems
{
    public class CollisionSystem(EcsWorld world) : IEcsRunSystem
    {
        private readonly EcsFilter _movableEntities = world.Filter<SceneObjectComponent>().Inc<MovementComponent>().End();
        private readonly EcsFilter _allEntities = world.Filter<SceneObjectComponent>().End();
        private readonly EcsPool<SceneObjectComponent> _sceneObjects = world.GetPool<SceneObjectComponent>();
        private readonly EcsPool<MovementComponent> _movements = world.GetPool<MovementComponent>();
        private readonly EcsPool<CollisionComponent> _collisions = world.GetPool<CollisionComponent>();

        public void Run(IEcsSystems systems)
        {
            foreach (int entityA in _movableEntities)
            {
                ref var sceneA = ref _sceneObjects.Get(entityA);
                ref var movement = ref _movements.Get(entityA);

                _collisions.Del(entityA); // clear previous collision

                Vector2 startPos = sceneA.SceneObject.Position;
                Vector2 endPos = startPos + movement.Direction * movement.Speed;

                foreach (int entityB in _allEntities)
                {
                    if (entityA == entityB) continue;

                    ref var sceneB = ref _sceneObjects.Get(entityB);

                    if (CheckInterpolatedCollision(sceneA.SceneObject, startPos, endPos, sceneB.SceneObject, out Vector2 normal))
                    {
                        ref var collision = ref _collisions.Add(entityA);
                        collision.OtherEntity = entityB;
                        collision.CollisionNormal = normal;
                        break; // first collision only
                    }
                }
            }
        }

        private bool CheckInterpolatedCollision(SceneObject a, Vector2 startPos, Vector2 endPos, SceneObject b, out Vector2 normal)
        {
            normal = Vector2.Zero;
            Vector2 movement = endPos - startPos;
            float distance = movement.Length();

            if (distance < 0.0001f) return false;

            // Use adaptive step based on distance and max step size
            float maxStep = 0.04f; // smaller = higher quality
            int steps = (int)Math.Ceiling(distance / maxStep);
            Vector2 stepDelta = movement / steps;

            Vector2 prevPos = startPos;

            for (int i = 1; i <= steps; i++)
            {
                Vector2 interpPos = startPos + stepDelta * i;
                Vector2 moveSegment = interpPos - prevPos;

                foreach (var edgeA in a.Edges)
                {
                    Vector2 edgeStartA = edgeA.VertexA + (interpPos - a.Position);
                    Vector2 edgeEndA = edgeA.VertexB + (interpPos - a.Position);

                    foreach (var edgeB in b.Edges)
                    {
                        if (CollisionUtils.DoSegmentsIntersect(edgeStartA, edgeEndA, edgeB.VertexA, edgeB.VertexB, out Vector2 contact))
                        {
                            normal = CollisionUtils.EdgeNormal(edgeStartA, edgeEndA);
                            if (Vector2.Dot(normal, b.Position - interpPos) < 0)
                                normal = -normal;

                            return true;
                        }
                    }
                }

                prevPos = interpPos;
            }

            return false;
        }
    }
}