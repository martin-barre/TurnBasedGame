using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class ServerEffectBase
{
    public bool canTouchLauncher;
    public bool canTouchMate;
    public bool canTouchEnemy;

    public abstract List<IPacket> Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos, GameState gameState, Map map);

    protected List<Entity> GetFilteredEntities(Entity launcher, List<Entity> entities)
    {
        return entities
            .Where(entity =>
                entity != null &&
                (canTouchLauncher || launcher != entity) &&
                (canTouchMate || launcher.Team != entity.Team) &&
                (canTouchEnemy || launcher.Team == entity.Team))
            .ToList();
    }
}
