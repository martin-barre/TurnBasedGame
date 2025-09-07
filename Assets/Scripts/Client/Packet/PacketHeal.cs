using System;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class PacketHeal : IPacket
{
    [Key(0)] public int TargetId { get; set; }
    [Key(1)] public int Value { get; set; }
    
    public Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        if(entity == null) throw new Exception($"Entity with id {TargetId} not found.");
        
        EntityPrefabController entityPrefab = GameManagerClient.Instance.GetEntityPrefab(TargetId);
        if(entityPrefab == null) throw new Exception($"EntityPrefab with id {TargetId} not found.");
        
        entity.Hp += Value;
        ViewModelFactory.Entity.NotifyUpdate(entity);
        
        GameManagerClient.Instance.SendChatMessage($"{entity.Race.Name} gagne {Value} pv");
        InteractionManager.ShowInfo(Value.ToString(), entityPrefab.transform.position + Vector3.up * 1f, Color.green);
        
        return Task.CompletedTask;
    }
}