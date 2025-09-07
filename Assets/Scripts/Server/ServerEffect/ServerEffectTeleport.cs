using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ServerEffectTeleport : ServerEffectBase
{
    [SerializeField] private bool canSwap;

    public override List<IPacket> Apply(Entity launcher, Spell spell, List<Entity> entities, Vector2Int targetPos, GameState gameState, Map map)
    {
        List<IPacket> clientEffects = new();
        Node node = map.GetNode(targetPos);
        
        if (!canSwap && gameState.GetEntityByGridPosition(node.GridPosition) != null)
            return clientEffects;
        
        gameState.MoveOrSwapEntity(launcher, targetPos);
        clientEffects.Add(new PacketTeleport
        {
            TargetId = launcher.Id,
            GridPosition = targetPos,
        });
        
        return clientEffects;
    }
}
