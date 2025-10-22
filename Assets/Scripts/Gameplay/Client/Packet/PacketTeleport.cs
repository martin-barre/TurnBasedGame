using System;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public readonly struct PacketTeleport : IPacket
{
    [Key(0)] public readonly int TargetId;
    [Key(1)] public readonly Vector2Int GridPosition;
    
    [SerializationConstructor]
    public PacketTeleport(int targetId, Vector2Int gridPosition)
    {
        TargetId = targetId;
        GridPosition = gridPosition;
    }
    
    public Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        if(entity == null) throw new Exception($"Entity with id {TargetId} not found.");

        EntityPrefabController entityPrefab = GameManagerClient.Instance.GetEntityPrefab(TargetId);
        if(entityPrefab == null) throw new Exception($"EntityPrefab with id {TargetId} not found.");
        
        Entity entity2 = GameManagerClient.Instance.GameState.GetEntityByGridPosition(GridPosition);
        EntityPrefabController entityPrefab2 = entity2 != null ? GameManagerClient.Instance.GetEntityPrefab(entity2.Id) : null;
        Vector2Int oldPosition = entity.GridPosition;
        
        GameManagerClient.Instance.GameState.MoveOrSwapEntity(entity, GridPosition);
        
        ViewModelFactory.Entity.NotifyUpdate(entity);
        ViewModelFactory.Entity.NotifyUpdate(entity2);
        
        entityPrefab.transform.position = GameManagerClient.Instance.Map.GetNode(GridPosition).WorldPosition;
        if (entityPrefab2 != null)
        {
            entityPrefab2.transform.position = GameManagerClient.Instance.Map.GetNode(oldPosition).WorldPosition;
        }
        
        return Task.CompletedTask;
    }
}