using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ServerEffectDamage : ServerEffectBase
{
    public int damageMin;
    public int damageMax;
    public bool isLifeSteal;

    public override List<IPacket> Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos, GameState gameState, Map map)
    {
        List<IPacket> clientEffects = new();
        List<Entity> filteredEntities = GetFilteredEntities(launcher, entities);
        List<AddStatsEffect> launcherEffects = launcher.Buffs.SelectMany(b => b.Buff.Effects).OfType<AddStatsEffect>().ToList();
        
        foreach (Entity entity in filteredEntities)
        {
            int damage = Random.Range(damageMin, damageMax + 1);
            damage *= 1 + launcherEffects.Where(e => e.Stats == Stats.DAMAGE_MULTIPLIER).Sum(b => b.Value) / 100;
            damage += launcherEffects.Where(b => b.Stats == Stats.DAMAGE_FLAT).Sum(b => b.Value);

            entity.Hp -= damage;
            clientEffects.Add(new PacketDamage
            {
                TargetId = entity.Id,
                Value = damage
            });

            if (isLifeSteal)
            {
                launcher.Hp += damage / 2;
                clientEffects.Add(new PacketHeal
                {
                    TargetId = launcher.Id,
                    Value = damage / 2
                });
            }
        }

        foreach (Entity entity in filteredEntities)
        {
            if (entity.Hp <= 0)
            {
                clientEffects.Add(GameManagerServer.Instance.KillEntity(entity, gameState));
            }
        }

        return clientEffects.Where(e => e != null).ToList();
    }
}