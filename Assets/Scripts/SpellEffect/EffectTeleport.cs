using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectTeleport : Effect
{
    private enum EntityToTeleport
    {
        LAUNCHER,
        ENTITIES_IN_ZONE
    }

    private enum Mode
    {
        EXCHANGE,
        SYMETRIC
    }

    private enum Target
    {
        LAUNCHER,
        TARGET
    }

    [Header("SPECIAL")]
    [SerializeField] private EntityToTeleport entityToTeleport;
    [SerializeField] private Mode mode;
    [SerializeField] private Target target;

    public override void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos)
    {
        base.Apply(launcher, spell, entities, targetPos);

        // TELEPORT LAUNCHER
        if (entityToTeleport == EntityToTeleport.LAUNCHER)
        {
            if (mode == Mode.EXCHANGE && target == Target.TARGET)
            {
                Node targetNode = MapManager.Instance.GetNode(targetPos);
                MapManager.Instance.MoveEntity(launcher, targetNode);
                launcher.transform.position = targetNode.worldPosition;
            }
            else if (mode == Mode.SYMETRIC && target == Target.TARGET)
            {
                Vector2Int distance = targetPos - launcher.node.gridPosition;
                Entity targetEntity = MapManager.Instance.GetNode(targetPos).entity;
                Node targetNode = MapManager.Instance.GetNode(targetPos + distance);
                if (targetEntity != null)
                {
                    MapManager.Instance.MoveEntity(launcher, targetNode);
                    launcher.transform.position = targetNode.worldPosition;
                }
            }
        }

        // TELEPORT ENTITIES IN SPELL ZONE
        if (entityToTeleport == EntityToTeleport.ENTITIES_IN_ZONE)
        {
            foreach (Entity entity in entities)
            {
                if (mode == Mode.EXCHANGE)
                {
                    if (target == Target.TARGET)
                    {
                        Node targetNode = MapManager.Instance.GetNode(targetPos);
                        MapManager.Instance.MoveEntity(entity, targetNode);
                        launcher.transform.position = targetNode.worldPosition;
                    }
                    else if (target == Target.LAUNCHER)
                    {
                        MapManager.Instance.MoveEntity(entity, launcher.node);
                        launcher.transform.position = launcher.node.worldPosition;
                    }
                }
                else if (mode == Mode.SYMETRIC)
                {
                    if (target == Target.TARGET)
                    {
                        Vector2Int distance = targetPos - entity.node.gridPosition;
                        Node targetNode = MapManager.Instance.GetNode(targetPos + distance);
                        MapManager.Instance.MoveEntity(entity, targetNode);
                        launcher.transform.position = targetNode.worldPosition;
                    }
                    else if (target == Target.LAUNCHER)
                    {
                        Vector2Int distance = launcher.node.gridPosition - entity.node.gridPosition;
                        Node targetNode = MapManager.Instance.GetNode(launcher.node.gridPosition + distance);
                        MapManager.Instance.MoveEntity(entity, targetNode);
                        launcher.transform.position = targetNode.worldPosition;
                    }
                }
            }
        }
    }
}
