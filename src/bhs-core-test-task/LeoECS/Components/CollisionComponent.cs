using System.Numerics;

namespace LeoECS.Components
{
    /// <summary>
    /// Stores information about a collision detected in this frame.
    /// </summary>
    public struct CollisionComponent
    {
        /// <summary>Entity collided with.</summary>
        public int OtherEntity;

        /// <summary>Normal at the contact point, pointing away from this entity.</summary>
        public Vector2 CollisionNormal;

        public int CollisionFrame;
    }
}