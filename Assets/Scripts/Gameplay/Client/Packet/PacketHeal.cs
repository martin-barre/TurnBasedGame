using System;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public readonly struct PacketHeal : IPacket
{
    [Key(0)] public readonly int TargetId;
    [Key(1)] public readonly int Value;
    
    [SerializationConstructor]
    public PacketHeal(int targetId, int value)
    {
        TargetId = targetId;
        Value = value;
    }
    
    public Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        if(entity == null) throw new Exception($"Entity with id {TargetId} not found.");
        
        EntityPrefabController entityPrefab = GameManagerClient.Instance.GetEntityPrefab(TargetId);
        if(entityPrefab == null) throw new Exception($"EntityPrefab with id {TargetId} not found.");
        
        entity.Hp += Value;
        ViewModelFactory.Entity.NotifyUpdate(entity);
        
        GameManagerClient.Instance.SendChatMessage($"<color=#FF0000>{entity.Race.Name}</color> gagne <color=#00FF00>{Value}</color> pv");
        InteractionManager.ShowInfo(Value.ToString(), entityPrefab.transform.position + Vector3.up * 1f, Color.green);
        
        return Task.CompletedTask;
    }
}