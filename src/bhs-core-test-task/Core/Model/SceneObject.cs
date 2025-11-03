using System.Numerics;

namespace App.Model;

/// <summary>
/// Base class for all objects that can be placed in the scene.
/// </summary>
public abstract class SceneObject
{
    /// <summary>
    /// Gets the current position of the scene object.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Gets the list of edges that compose this scene object.
    /// </summary>
    public List<Edge> Edges { get; } = new List<Edge>();

    /// <summary>
    /// Sets the position of the scene object and updates all its edges accordingly.
    /// </summary>
    /// <param name="newPosition">The new position to set.</param>
    public void SetPosition(Vector2 newPosition)
    {
        var delta = newPosition - Position;
        Position = newPosition;

        foreach (Edge edge in Edges)
        {
            edge.VertexA += delta;
            edge.VertexB += delta;
            edge.Pivot += delta;
        }
    }
}