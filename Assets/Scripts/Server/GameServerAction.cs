using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameServerAction
{
    public static PacketMove Move(Vector2Int gridPosition, GameState gameState)
    {
        Entity entity = gameState.CurrentEntity;
        List<Node> path = BFS.GetPath(entity.GridPosition, gridPosition, gameState, GameManagerServer.Instance.Map);
        entity.Pm -= path.Count;
        gameState.MoveOrSwapEntity(entity, gridPosition);

        return new PacketMove
        {
            TargetId = entity.Id,
            PmCost = path.Count,
            Path = path.Select(n => n.GridPosition).ToArray()
        };
    }
    
    public static List<IPacket> LaunchSpell(int spellId, Vector2Int targetPos, GameState gameState)
    {
        List<IPacket> clientEffects = new();
        Spell spell = SpellDatabase.GetById(spellId);
        if (spell == null) return clientEffects;
        
        Entity entity = gameState.CurrentEntity;
        if (entity.Pa < spell.paCost) return clientEffects;
        
        Race race = RaceDatabase.GetById(entity.Race.Id);
        if (race.Spells.All(s => s.id != spellId)) return clientEffects;
        
        List<Node> fovNodes = FOV.GetDisplacement(entity, spell, gameState, GameManagerServer.Instance.Map);
        if(fovNodes.All(n => n.GridPosition != targetPos))
            return clientEffects;
        
        entity.Pa -= spell.paCost;
        
        clientEffects.Add(new PacketLaunchSpell
        {
            LauncherId = entity.Id,
            SpellId = spellId,
            TargetPos = targetPos
        });
        clientEffects.AddRange(spell.Launch(entity, spell, targetPos, gameState, GameManagerServer.Instance.Map));

        return clientEffects;
    }
    
    public static PacketNextTurn NextTurn(GameState gameState)
    {
        Entity entity = gameState.CurrentEntity;
        entity.Pa = entity.Race.Pa;
        entity.Pm = entity.Race.Pm;
        entity.Buffs.ForEach(b => b.TurnDuration--);
        entity.Buffs.RemoveAll(b => b.TurnDuration <= 0);
        
        gameState.CurrentEntityIndex =
            gameState.CurrentEntityIndex >= gameState.Entities.Count - 1
                ? 0
                : gameState.CurrentEntityIndex + 1;

        return new PacketNextTurn();
    }
}
