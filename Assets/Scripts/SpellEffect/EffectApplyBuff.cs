using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectApplyBuff : Effect
{
    [Header("SPECIAL")]
    public bool applyOnLauncher;
    public bool applyOnTarget;

    public override void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos)
    {
        base.Apply(launcher, spell, entities, targetPos);
    }
}