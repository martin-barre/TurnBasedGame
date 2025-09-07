using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class PacketKillEntity : IPacket
{
    [Key(0)] public int TargetId { get; set; }
    
    private bool _animationEnded;
    
    public async Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        EntityPrefabController entityPrefabController = GameManagerClient.Instance.GetEntityPrefab(entity.Id);
        GameManagerClient.Instance.SendChatMessage($"{entity.Race.Name} est mort");
        await entityPrefabController.TriggerAnimAndWaitAsync("Dead");
        GameManagerClient.Instance.KillEntity(TargetId);
    }
}