using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ServerEffectSummon : ServerEffectBase
{
    public Race Race;

    public override List<IPacket> Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos, GameState gameState, Map map)
    {
        List<IPacket> clientEffects = new ();

        Node node = map.GetNode(targetPos);
        if (node.NodeType == NodeType.Ground && gameState.GetEntityByGridPosition(node.GridPosition) == null)
        {
            clientEffects.Add(GameManagerServer.Instance.SpawnEntity(launcher.Team, Race.Id, targetPos, launcher.IsPlayer, gameState, launcher));
        }

        return clientEffects.Where(e => e != null).ToList();
    }
}