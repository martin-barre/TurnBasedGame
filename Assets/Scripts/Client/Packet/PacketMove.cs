using System;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class PacketMove : IPacket
{
    [Key(0)] public int TargetId { get; set; }
    [Key(1)] public int PmCost { get; set; }
    [Key(2)] public Vector2Int[] Path { get; set; }
    
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