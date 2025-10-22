using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public readonly struct PacketKillEntity : IPacket
{
    [Key(0)] public readonly int TargetId;
    
    [SerializationConstructor]
    public PacketKillEntity(int targetId)
    {
        TargetId = targetId;
    }
    
    public async Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        EntityPrefabController entityPrefabController = GameManagerClient.Instance.GetEntityPrefab(entity.Id);
        GameManagerClient.Instance.SendChatMessage($"<color=#FF0000>{entity.Race.Name}</color> est mort");
        await entityPrefabController.TriggerAnimAndWaitAsync("Dead");
        GameManagerClient.Instance.KillEntity(TargetId);
    }
}