using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public readonly struct PacketSetGameLogic : IPacket
{
    [Key(0)] public readonly bool IsStarted;
    [Key(1)] public readonly int CurrentPlayer;
    
    [SerializationConstructor]
    public PacketSetGameLogic(bool isStarted, int currentPlayer)
    {
        IsStarted = isStarted;
        CurrentPlayer = currentPlayer;
    }
    
    public Task ApplyAsync()
    {
        GameManagerClient.Instance.GameState.IsStarted = IsStarted;
        GameManagerClient.Instance.GameState.CurrentEntityIndex = CurrentPlayer;
        ViewModelFactory.Game.NotifyUpdate(GameManagerClient.Instance.GameState);
        MapManager.Instance.ActiveTilemapSpawns(!IsStarted);
        return Task.CompletedTask;
    }
}