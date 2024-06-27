using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectInvocation : Effect
{
    [Header("SPECIAL")]
    public ERace RaceEnum;

    public override void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos)
    {
        base.Apply(launcher, spell, entities, targetPos);

        Race race = GameManager.Instance.GetRace(RaceEnum);
        GameManager.Instance.AddEntity(new EntityData
        {
            RaceEnum = RaceEnum,
            Team = launcher.data.Team,
            Position = targetPos,
            Hp = race.Hp,
            Pa = race.Pa,
            Pm = race.Pm,
            ParentId = launcher.data.Id
        });
    }
}