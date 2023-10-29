using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectDamage : Effect {

    [Header("SPECIAL")]
    public int damageMin;
    public int damageMax;
    public bool lifeSteal;

    public override void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos) {
        base.Apply(launcher, spell, entities, targetPos);
        
        foreach(Entity entity in entities) {
            int damage = UnityEngine.Random.Range(damageMin, damageMax + 1);
            entity.ApplyDamage(damage);
            
            if(lifeSteal) launcher.CurrentHp += damage / 2;
        }
    }

}