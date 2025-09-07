using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ServerEffectBuff : ServerEffectBase
{
    [SerializeField] private Buff buff;

    public override List<IPacket> Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos, GameState gameState, Map map)
    {
        List<IPacket> clientEffects = new();
        List<Entity> filteredEntities = GetFilteredEntities(launcher, entities);

        foreach (Entity entity in filteredEntities)
        {
            entity.Buffs.Add(new ActiveBuff
            {
                Buff = buff,
                TurnDuration = buff.TurnDuration,
                Launcher = launcher
            });
            clientEffects.Add(new PacketBuff
            {
                TargetId = entity.Id,
                LauncherId = launcher.Id,
                BuffId = buff.Id
            });
        }

        return clientEffects.ToList();
    }
}