using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class PacketSummonEntity : IPacket
{
    [Key(0)] public int EntityId { get; set; }
    [Key(1)] public Team Team { get; set; }
    [Key(2)] public int RaceId { get; set; }
    [Key(3)] public Vector2Int GridPosition { get; set; }
    [Key(4)] public bool IsPlayer { get; set; }
    [Key(5)] public int SummonerId { get; set; }
    
    public Task ApplyAsync()
    {
        Entity summoner = GameManagerClient.Instance.GameState.GetEntityById(SummonerId);
        Entity invocation = GameManagerClient.Instance.GameState.GetEntityById(EntityId);

        if (summoner != null)
        {
            GameManagerClient.Instance.SendChatMessage($"{summoner.Race.Name} invoque {invocation.Race.Name}");
        }
        GameManagerClient.Instance.SpawnEntity(EntityId, Team, RaceId, GridPosition, IsPlayer, SummonerId);
        return Task.CompletedTask;
    }
}