namespace App.GameLoop;

/// <summary>
/// Represents an object that can be updated in the game loop.
/// </summary>
public interface IUpdateBehaviour
{
    /// <summary>
    /// Updates the behaviour logic.
    /// </summary>
    void Update();
}