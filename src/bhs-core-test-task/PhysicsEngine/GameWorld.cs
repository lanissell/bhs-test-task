using LeoECS.Components;
using LeoECS.Systems;
using Leopotam.EcsLite;
using System.Numerics;
using LeoECS;
using PhysicsEngine.Colliders;

public class GameWorld
{
    public static GameWorld Instance { get; private set; }

    public List<Edge> Edges = new List<Edge>();

    private EcsWorld world;
    private EcsSystems systems;

    private CancellationTokenSource cts;

    public GameWorld()
    {
        Instance = this;

        world = new EcsWorld();
        systems = new EcsSystems(world);

        // Register systems in correct order:
        // 1. CollisionSystem - detects collisions
        // 2. BounceSystem - processes bounce on collision
        // 3. MovementSystem - applies final movement
        systems
            .Add(new CollisionSystem(world))
            .Add(new BounceSystem(world))
            .Add(new MovementSystem(world))
            .Init();


        // Create test entities
        CreateTestScene();

        cts = new CancellationTokenSource();
        Task.Run(Update, cts.Token);
    }

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }

    private async Task Update()
    {
        while (!cts.IsCancellationRequested)
        {
            systems?.Run();
            await Task.Delay((int)(Consts.FrameDeltaTime * 1000f));
        }
    }

    private void CreateTestScene()
    {
        int centerX = 400;
        int centerY = 300;

        // Create bouncing balls
        for(int i = 0; i < 5; i++)
        {
            var ball = new Circle(
                new Vector2(centerX + (i - 2) * 50, centerY - 50),
                10,
                8
            );

            Edges.AddRange(ball.Edges);

            var entity = world.NewEntity();

            // Add SceneObjectComponent
            ref var sceneObj = ref world.GetPool<PhysicsComponentWrapper>().Add(entity);
            sceneObj.Component = new CirclePhysicsComponent(ball);

            // Add MovementComponent
            ref var movement = ref world.GetPool<MovementComponent>().Add(entity);
            movement.Speed = 150f;
            movement.Direction = new Vector2(0, 1);

            world.GetPool<CollisionComponent>().Add(entity);
            world.GetPool<BounceComponent>().Add(entity);
        }

        // Create triangle walls
        int triangleSize = 500;
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

        Edges.AddRange(wall.Edges);

        var entity = world.NewEntity();

        ref var sceneObj = ref world.GetPool<PhysicsComponentWrapper>().Add(entity);
        sceneObj.Component = new LinePhysicsComponent(wall);

        world.GetPool<CollisionComponent>().Add(entity);
    }
}