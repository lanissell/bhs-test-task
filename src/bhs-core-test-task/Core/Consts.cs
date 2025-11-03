using System.Numerics;

namespace LeoECS;

public static class Consts
{
    public static class GameLoop
    {
        public const float FrameDeltaTime = 1f / 144f;
    }

    public static class Scene
    {
        public const float CenterX = 400f;
        public const float CenterY = 300f;

        public const float CirclesOffset = 25f;
        public const int CirclesCount = 8;
        public const int CircleRadius = 10;
        public const int CircleSegments = 8;

        public const float TriangleSize = 550f;
    }

    public static class Movement
    {
        public const float InitialSpeed = 5f;
        public readonly static Vector2 Direction = new Vector2(0f, 1f);
    }
}