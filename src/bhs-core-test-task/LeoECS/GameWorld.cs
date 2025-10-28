using LeoECS.Components;
using LeoECS.Systems;
using Leopotam.EcsLite;
using System.Numerics;

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

        // Register systems
        systems
            .Add(new MovementSystem(world))
            .Add(new BounceSystem(world))
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
            await Task.Delay(1);
        }
    }

    private void CreateTestScene()
    {
        int centerX = 400;
        int centerY = 300;
        int areaSize = 400;

        // Create bouncing balls
        for(int i = 0; i < 5; i++)
        {
            var ball = new Circle(
                new Vector2(centerX + (i - 2) * 50, centerY - 100),
                10,
                16
            );

            Edges.AddRange(ball.Edges);

            var entity = world.NewEntity();
            ref var sceneObj = ref world.GetPool<SceneObjectComponent>().Add(entity);
            ref var movement = ref world.GetPool<MovementComponent>().Add(entity);
            ref var bounce = ref world.GetPool<BounceComponent>().Add(entity);

            sceneObj.SceneObject = ball;
            movement.Speed = 2f + i * 0.5f;
            movement.Direction = Vector2.Normalize(new Vector2(
                (float)Math.Sin(i) * 0.7f,
                (float)Math.Cos(i) * 0.7f
            ));
            bounce.Bounciness = 0.9f;
        }

        int halfSize = areaSize / 2;
        CreateWall(new Vector2(centerX - halfSize, centerY - halfSize), new Vector2(centerX + halfSize, centerY - halfSize)); // Top
        CreateWall(new Vector2(centerX - halfSize, centerY + halfSize), new Vector2(centerX + halfSize, centerY + halfSize)); // Bottom
        CreateWall(new Vector2(centerX - halfSize, centerY - halfSize), new Vector2(centerX - halfSize, centerY + halfSize)); // Left
        CreateWall(new Vector2(centerX + halfSize, centerY - halfSize), new Vector2(centerX + halfSize, centerY + halfSize)); // Right

        CreateWall(new Vector2(centerX - 100, centerY - 50), new Vector2(centerX - 50, centerY));
        CreateWall(new Vector2(centerX + 50, centerY - 30), new Vector2(centerX + 100, centerY + 20));
    }

    private void CreateWall(Vector2 start, Vector2 end)
    {
        var wall = new Line(start, end);

        Edges.AddRange(wall.Edges);

        var entity = world.NewEntity();
        ref var sceneObj = ref world.GetPool<SceneObjectComponent>().Add(entity);
        sceneObj.SceneObject = wall;
    }
}
