using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectPush : Effect
{

    private enum CenterPoint
    {
        LAUNCHER,
        TARGET
    }

    [Header("SPECIAL")]
    [SerializeField] private int nbOfTile;
    [SerializeField] private CenterPoint centerPoint;

    /*
        7     8     1
          \   |   /
            \ | /
        6 --------- 2
            / | \
          /   |   \
        5     4     3
    */

    public override void Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos)
    {
        base.Apply(launcher, spell, entities, targetPos);

        Vector2Int launcherPosition = centerPoint == CenterPoint.LAUNCHER ? launcher.node.gridPosition : targetPos;

        // SORT ENTITIES WITH DISTANCE ( FOR PATH BUG WHAN ENTITY IS PUSH ON ANOTHER ENTITY )
        List<Entity> cloneEntities = new List<Entity>(entities);
        cloneEntities.Sort(delegate (Entity a, Entity b)
        {
            float distA = Vector2.Distance(launcherPosition, a.node.gridPosition);
            float distB = Vector2.Distance(launcherPosition, b.node.gridPosition);
            return distB.CompareTo(distA);
        });

        foreach (Entity entity in cloneEntities)
        {
            Vector2Int targetPosition = entity.node.gridPosition;
            Vector2Int direction = Utils.GridDirection(launcherPosition, targetPosition);
            bool isDiagonal = Mathf.Sign(direction.x) + Mathf.Sign(direction.y) == 2;

            Node node = null;
            for (int i = 1; i <= nbOfTile; i++)
            {
                Node tmp = MapManager.Instance.GetNode(targetPosition + direction * i);
                if (tmp.type != NodeType.GROUND || tmp.entity != null) break;

                if (isDiagonal)
                {
                    Node node1 = MapManager.Instance.GetNode(targetPosition + direction * i + new Vector2Int(direction.x, 0));
                    Node node2 = MapManager.Instance.GetNode(targetPosition + direction * i + new Vector2Int(0, direction.y));
                    if (node1.type != NodeType.GROUND || node1.entity != null) break;
                    if (node2.type != NodeType.GROUND || node2.entity != null) break;
                }

                node = tmp;
            }

            if (node != null)
            {
                MapManager.Instance.MoveEntity(entity, node);
                entity.SetPath(new List<Node>() { node });
            }

        }
    }

}