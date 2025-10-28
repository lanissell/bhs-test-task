using LeoECS.Components;
using LeoECS.Systems;
using Leopotam.EcsLite;
using System.Numerics;

public class GameWorld
{
    public static GameWorld Instance { get; private set; }

    private EcsWorld world;
    private EcsSystems systems;

    public GameWorld()
    {
        Instance = this;
    }

    public void Initialize()
    {
        world = new EcsWorld();
        systems = new EcsSystems(world);

        // Register systems
        systems
            .Add(new MovementSystem(world))
            .Add(new BounceSystem(world))
            .Init();

        // Create test entities
        CreateTestScene();
    }

    public void Update()
    {
        systems?.Run();
    }

    private void CreateTestScene()
    {
        // Create bouncing balls
        for( int i = 0; i < 5; i++ )
        {
            var ball = new Circle(
                new Vector2((i - 2) * 50, 0),
                20 + i * 5,
                16
            );

            var entity = world.NewEntity();
            ref var sceneObj = ref world.GetPool<SceneObjectComponent>().Add(entity);
            ref var movement = ref world.GetPool<MovementComponent>().Add(entity);
            ref var bounce = ref world.GetPool<BounceComponent>().Add(entity);

            sceneObj.SceneObject = ball;
            movement.Speed = 2f + i * 0.5f;
            movement.Direction = Vector2.Normalize(new Vector2(
                (float) Math.Sin(i) * 0.7f,
                (float) Math.Cos(i) * 0.7f
            ));
            bounce.Bounciness = 0.9f;
        }

        CreateWall(new Vector2(-200, 0), new Vector2(200, 0)); // Top wall
        CreateWall(new Vector2(-200, 150), new Vector2(200, 150)); // Bottom wall
        CreateWall(new Vector2(-200, 0), new Vector2(-200, 150)); // Left wall
        CreateWall(new Vector2(200, 0), new Vector2(200, 150)); // Right wall

        CreateWall(new Vector2(-100, 50), new Vector2(-50, 100)); // Diagonal wall
        CreateWall(new Vector2(50, 30), new Vector2(100, 80)); // Another diagonal
    }

    private void CreateWall(Vector2 start, Vector2 end)
    {
        var wall = new Line(start, end);

        var entity = world.NewEntity();
        ref var sceneObj = ref world.GetPool<SceneObjectComponent>().Add(entity);
        sceneObj.SceneObject = wall;
    }
}
