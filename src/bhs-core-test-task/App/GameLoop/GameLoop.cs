using LeoECS;

namespace App.GameLoop;

/// <summary>
/// Manages the game loop and executes update behaviours at fixed intervals.
/// </summary>
public class GameLoop : IDisposable
{
    private List<IUpdateBehaviour> updateBehaviours = new List<IUpdateBehaviour>();

    private CancellationTokenSource cts;

    /// <summary>
    /// Starts the game loop execution.
    /// </summary>
    public void RunLoop()
    {
        if (cts != null)
        {
            cts.Cancel();
        }

        cts = new CancellationTokenSource();
        Task.Run(Update, cts.Token);
    }

    /// <summary>
    /// Adds an update behaviour to the game loop.
    /// </summary>
    /// <param name="updateBehaviour">The behaviour to add.</param>
    public void AddUpdateBehaviour(IUpdateBehaviour updateBehaviour)
    {
        updateBehaviours.Add(updateBehaviour);
    }

    /// <summary>
    /// Removes an update behaviour from the game loop.
    /// </summary>
    /// <param name="updateBehaviour">The behaviour to remove.</param>
    public void RemoveUpdateBehaviour(IUpdateBehaviour updateBehaviour)
    {
        updateBehaviours.Remove(updateBehaviour);
    }

    private async Task Update()
    {
        while (!cts.IsCancellationRequested)
        {
            foreach (var updateBehaviour in updateBehaviours)
            {
                updateBehaviour.Update();
            }

            await Task.Delay((int)(Consts.GameLoop.FrameDeltaTime * 1000f));
        }
    }

    /// <summary>
    /// Disposes resources and stops the game loop.
    /// </summary>
    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }
}