using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    [Header("COMMON")]
    public int order;
    public bool canTouchLauncher;
    public bool canTouchMate;
    public bool canTouchEnemy;

    public virtual void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos)
    {
        entities = entities
            .Where(entity =>
                entity != null &&
                (canTouchLauncher || !canTouchLauncher && launcher != entity) &&
                (canTouchMate || !canTouchMate && launcher.team != entity.team) &&
                (canTouchEnemy || !canTouchEnemy && launcher.team != entity.team))
            .ToList();
    }
}
