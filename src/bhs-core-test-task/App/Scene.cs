using System.Numerics;
using App.GameLoop;
using App.Model;
using Core.Physics.Components;
using Core.Physics.Components.PhysicsComponent;
using Core.Physics.Systems;
using LeoECS;
using LeoECS.Systems;
using Leopotam.EcsLite;

namespace App;

/// <summary>
/// Manages the game scene containing all scene objects and ECS systems.
/// </summary>
public class Scene : IUpdateBehaviour
{
    /// <summary>
    /// List of all objects in the scene.
    /// </summary>
    public readonly List<SceneObject> SceneObjects = new List<SceneObject>();

    private EcsWorld world;
    private EcsSystems systems;

    /// <summary>
    /// Event raised when the scene state changes.
    /// </summary>
    public event Action? SceneChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scene"/> class.
    /// </summary>
    public Scene()
    {
        world = new EcsWorld();
        systems = new EcsSystems(world);

        systems
            .Add(new MovementSystem(world))
            .Add(new CollisionSystem(world))
            .Add(new BounceSystem(world))
            .Init();

            CreateScene();
    }

    /// <summary>
    /// Updates the scene by running all ECS systems.
    /// </summary>
    public void Update()
    {
        systems.Run();
        SceneChanged?.Invoke();
    }

    private void CreateScene()
    {
        float centerX = Consts.Scene.CenterX;
        float centerY = Consts.Scene.CenterY;
        float offset = Consts.Scene.CirclesOffset;

        // Create bouncing balls
        for(int i = 0; i < Consts.Scene.CirclesCount; i++)
        {
            var ball = new Circle(
                new Vector2(centerX + (i - 2) * offset, centerY -  offset),
                Consts.Scene.CircleRadius,
                Consts.Scene.CircleSegments
            );

            SceneObjects.Add(ball);

            var entity = world.NewEntity();

            // Add SceneObjectComponent
            ref var sceneObj = ref world.GetPool<PhysicsComponentWrapper>().Add(entity);
            sceneObj.Component = new CirclePhysicsComponent(ball);

            // Add MovementComponent
            ref var movement = ref world.GetPool<MovementComponent>().Add(entity);
            movement.Speed = Consts.Movement.InitialSpeed;
            movement.Direction = Consts.Movement.Direction;

            world.GetPool<CollisionComponent>().Add(entity);
            world.GetPool<BounceComponent>().Add(entity);
        }

        // Create triangle walls
        float triangleSize = Consts.Scene.TriangleSize;
        float height = triangleSize * MathF.Sqrt(3) / 2;

        Vector2 top = new Vector2(centerX, centerY - height * 0.6f);
        Vector2 bottomLeft = new Vector2(centerX - triangleSize / 2, centerY + height * 0.4f);
        Vector2 bottomRight = new Vector2(centerX + triangleSize / 2, centerY + height * 0.4f);

        CreateWall(top, bottomLeft);           // Left side
        CreateWall(bottomLeft, bottomRight);   // Bottom
        CreateWall(bottomRight, top);          // Right side
    }

    private void CreateWall(Vector2 start, Vector2 end)
    {
        var wall = new Line(start, end);

        SceneObjects.Add(wall);

        var entity = world.NewEntity();

        ref var sceneObj = ref world.GetPool<PhysicsComponentWrapper>().Add(entity);
        sceneObj.Component = new LinePhysicsComponent(wall);

        world.GetPool<CollisionComponent>().Add(entity);
    }
}