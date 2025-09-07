using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class PacketSetGameLogic : IPacket
{
    [Key(0)] public bool IsStarted;
    [Key(1)] public int CurrentPlayer;
    
    public Task ApplyAsync()
    {
        GameManagerClient.Instance.GameState.IsStarted = IsStarted;
        GameManagerClient.Instance.GameState.CurrentEntityIndex = CurrentPlayer;
        ViewModelFactory.Game.NotifyUpdate(GameManagerClient.Instance.GameState);
        MapManager.Instance.ActiveTilemapSpawns(!IsStarted);
        return Task.CompletedTask;
    }
}