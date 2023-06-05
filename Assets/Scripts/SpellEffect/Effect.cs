using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect {

    [Header("COMMON")]
    public int order;
    public bool canTouchLauncher;
    public bool canTouchMate;
    public bool canTouchEnemy;

    public virtual void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos) {
        entities = entities.FindAll(entity => {
            if(entity == null) return false;
            if(!canTouchLauncher && launcher == entity) return false;
            if(!canTouchMate && launcher.team == entity.team) return false;
            if(!canTouchEnemy && launcher.team != entity.team) return false;
            return true;
        });
    }
    
}
