using System.Numerics;

namespace LeoECS;

/// <summary>
/// Contains constant values used throughout the application.
/// </summary>
public static class Consts
{
    /// <summary>
    /// Constants related to the game loop timing.
    /// </summary>
    public static class GameLoop
    {
        /// <summary>
        /// The fixed time step for each frame in seconds.
        /// </summary>
        public const float FrameDeltaTime = 1f / 144f;
    }

    /// <summary>
    /// Constants related to scene configuration and layout.
    /// </summary>
    public static class Scene
    {
        /// <summary>
        /// The X coordinate of the scene center.
        /// </summary>
        public const float CenterX = 400f;

        /// <summary>
        /// The Y coordinate of the scene center.
        /// </summary>
        public const float CenterY = 300f;

        /// <summary>
        /// The offset distance between circles.
        /// </summary>
        public const float CirclesOffset = 25f;

        /// <summary>
        /// The total number of circles to create.
        /// </summary>
        public const int CirclesCount = 8;

        /// <summary>
        /// The radius of each circle.
        /// </summary>
        public const int CircleRadius = 10;

        /// <summary>
        /// The number of segments used to approximate each circle.
        /// </summary>
        public const int CircleSegments = 8;

        /// <summary>
        /// The size of the triangle boundary.
        /// </summary>
        public const float TriangleSize = 550f;
    }

    /// <summary>
    /// Constants related to entity movement.
    /// </summary>
    public static class Movement
    {
        /// <summary>
        /// The initial speed of moving entities in units per frame.
        /// </summary>
        public const float InitialSpeed = 6f;

        /// <summary>
        /// The initial direction of movement.
        /// </summary>
        public readonly static Vector2 Direction = new Vector2(0f, 1f);
    }

    /// <summary>
    /// Constants related to collision detection and resolution.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Maximum step length for continuous collision detection to prevent tunneling.
        /// </summary>
        public const float MaxStepLength = 10f;

        /// <summary>
        /// Small epsilon value to back off from collision surface to prevent stuck objects.
        /// </summary>
        public const float BackoffEpsilon = 0.001f;

        /// <summary>
        /// Number of binary search iterations to find precise contact point.
        /// </summary>
        public const int BinarySearchIterations = 5;
    }
}