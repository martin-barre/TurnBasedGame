using System.Collections.Generic;
using UnityEngine;

public interface IAiAction
{
    public List<IPacket> Apply(GameState gameState);
}

public class AiActionMove : IAiAction
{
    public Vector2Int GridPosition { get; set; }
    
    public List<IPacket> Apply(GameState gameState)
    {
        return new List<IPacket> { GameServerAction.Move(GridPosition, gameState) };
    }
}

public class AiActionLaunchSpell : IAiAction
{
    public int SpellId { get; set; }
    public Vector2Int GridPosition { get; set; }
    
    public List<IPacket> Apply(GameState gameState)
    {
        return GameServerAction.LaunchSpell(SpellId, GridPosition, gameState);
    }
}
