using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public readonly struct PacketSummonEntity : IPacket
{
    [Key(0)] public readonly int EntityId;
    [Key(1)] public readonly Team Team;
    [Key(2)] public readonly int RaceId;
    [Key(3)] public readonly Vector2Int GridPosition;
    [Key(4)] public readonly bool IsPlayer;
    [Key(5)] public readonly int SummonerId;
    
    [SerializationConstructor]
    public PacketSummonEntity(int entityId, Team team, int raceId, Vector2Int gridPosition, bool isPlayer, int summonerId)
    {
        EntityId = entityId;
        Team = team;
        RaceId = raceId;
        GridPosition = gridPosition;
        IsPlayer = isPlayer;
        SummonerId = summonerId;
    }
    
    public Task ApplyAsync()
    {
        Entity summoner = GameManagerClient.Instance.GameState.GetEntityById(SummonerId);
        Entity invocation = GameManagerClient.Instance.GameState.GetEntityById(EntityId);

        if (summoner != null)
        {
            GameManagerClient.Instance.SendChatMessage($"<color=#FF0000>{summoner.Race.Name}</color> invoque <color=#00FF00>{invocation.Race.Name}</color>");
        }
        GameManagerClient.Instance.SpawnEntity(EntityId, Team, RaceId, GridPosition, IsPlayer, SummonerId);
        return Task.CompletedTask;
    }
}