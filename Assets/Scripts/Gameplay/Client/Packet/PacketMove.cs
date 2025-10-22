using System;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public readonly struct PacketMove : IPacket
{
    [Key(0)] public readonly int TargetId;
    [Key(1)] public readonly int PmCost;
    [Key(2)] public readonly Vector2Int[] Path;
    
    [SerializationConstructor]
    public PacketMove(int targetId, int pmCost, Vector2Int[] path)
    {
        TargetId = targetId;
        PmCost = pmCost;
        Path = path;
    }
    
    public async Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        if(entity == null) throw new Exception($"Entity with id {TargetId} not found.");

        EntityPrefabController entityPrefab = GameManagerClient.Instance.GetEntityPrefab(TargetId);
        if(entityPrefab == null) throw new Exception($"EntityPrefab with id {TargetId} not found.");

        entity.Pm -= PmCost;
        GameManagerClient.Instance.GameState.MoveOrSwapEntity(entity, Path.Last());
        ViewModelFactory.Entity.NotifyUpdate(entity);
        
        entityPrefab.GetComponentInChildren<Animator>()?.SetBool("Move", true);
        PathMover pathMover = new(Path.Select(pos => GameManagerClient.Instance.Map.GetNode(pos).WorldPosition).ToList(), 2.5f);
        await pathMover.Move(entityPrefab);
        InteractionManager.ShowInfo(PmCost.ToString(), entityPrefab.transform.position + Vector3.up * 1f, Color.green);
        entityPrefab.GetComponentInChildren<Animator>()?.SetBool("Move", false);
    }
}