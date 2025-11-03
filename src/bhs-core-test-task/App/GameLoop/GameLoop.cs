using LeoECS;

namespace App.GameLoop;

public class GameLoop : IDisposable
{
    private List<IUpdateBehaviour> updateBehaviours = new List<IUpdateBehaviour>();

    private CancellationTokenSource cts;

    public void RunLoop()
    {
        if (cts != null)
        {
            cts.Cancel();
        }

        cts = new CancellationTokenSource();
        Task.Run(Update, cts.Token);
    }

    public void AddUpdateBehaviour(IUpdateBehaviour updateBehaviour)
    {
        updateBehaviours.Add(updateBehaviour);
    }

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

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }
}