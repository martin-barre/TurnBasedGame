using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public readonly struct PacketLaunchSpell : IPacket
{
    [Key(0)] public readonly int LauncherId;
    [Key(1)] public readonly int SpellId;
    [Key(2)] public readonly Vector2Int TargetPos;
    
    [SerializationConstructor]
    public PacketLaunchSpell(int launcherId, int spellId, Vector2Int targetPos)
    {
        LauncherId = launcherId;
        SpellId = spellId;
        TargetPos = targetPos;
    }
    
    public async Task ApplyAsync()
    {
        Spell spell = SpellDatabase.GetById(SpellId);
        if (spell == null) return;
        
        Entity entity = GameManagerClient.Instance.GameState.GetEntityById(LauncherId);
        if (entity.Pa < spell.paCost) return;
        
        GameManagerClient.Instance.SendChatMessage($"<color=#FF0000>{entity.Race.Name}</color> lance <color=#00FF00>{spell.spellName}</color>");
        
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