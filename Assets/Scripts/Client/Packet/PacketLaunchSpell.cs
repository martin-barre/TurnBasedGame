using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class PacketLaunchSpell : IPacket
{
    [Key(0)] public int LauncherId { get; set; }
    [Key(1)] public int SpellId { get; set; }
    [Key(2)] public Vector2Int TargetPos { get; set; }
    
    public async Task ApplyAsync()
    {
        Spell spell = SpellDatabase.GetById(SpellId);
        if (spell == null) return;
        
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(LauncherId);
        if (entity.Pa < spell.paCost) return;
        
        GameManagerClient.Instance.SendChatMessage($"{entity.Race.Name} lance {spell.spellName}");
        
        EntityPrefabController entityPrefabController = GameManagerClient.Instance.GetEntityPrefab(entity.Id);
        await entityPrefabController.TriggerAnimAndWaitAsync("Attack");
        
        InteractionManager.ShowInfo(spell.paCost.ToString(), entityPrefabController.transform.position + Vector3.up * 1f, Color.yellow);
        entity.Pa -= spell.paCost;
        ViewModelFactory.Entity.NotifyUpdate(entity);
        foreach (SFX effect in spell.sfx)
        {
            Vector3 position = effect.target == TargetEnum.Launcher
                ? GameManagerClient.Instance.Map.GetNode(entity.GridPosition).WorldPosition
                : GameManagerClient.Instance.Map.GetNode(TargetPos).WorldPosition;
            GameManagerClient.Instance.InstantiateObject(effect.prefab, position, Quaternion.identity);
        }
    }
}