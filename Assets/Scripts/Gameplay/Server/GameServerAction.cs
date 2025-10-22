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

        return new PacketMove(entity.Id, path.Count, path.Select(n => n.GridPosition).ToArray());
    }
    
    public static List<IPacket> LaunchSpell(int spellId, Vector2Int targetPos, GameState gameState, Map map)
    {
        List<IPacket> clientEffects = new();
        Spell spell = SpellDatabase.GetById(spellId);
        if (spell == null) return clientEffects;
        
        Entity entity = gameState.CurrentEntity;
        if (entity.Pa < spell.paCost) return clientEffects;
        
        Race race = RaceDatabase.GetById(entity.Race.Id);
        if (race.Spells.All(s => s.Id != spellId)) return clientEffects;
        
        List<Node> fovNodes = FOV.GetDisplacement(entity, spell, gameState, map);
        if(fovNodes.All(n => n.GridPosition != targetPos))
            return clientEffects;
        
        entity.Pa -= spell.paCost;
        
        clientEffects.Add(new PacketLaunchSpell(entity.Id, spellId, targetPos));
        clientEffects.AddRange(spell.Launch(entity, spell, targetPos, gameState, map));

        return clientEffects;
    }
    
    public static List<IPacket> NextTurn(GameState gameState)
    {
        Entity entity = gameState.CurrentEntity;
        entity.Pa = entity.Race.Pa;
        entity.Pm = entity.Race.Pm;
        entity.Buffs.RemoveAll(s => s.TurnDuration is 0 or 1);
        entity.Buffs.ForEach(s =>
        {
            if (s.TurnDuration != -1)
            {
                s.TurnDuration--;
            }
        });
        
        gameState.CurrentEntityIndex =
            gameState.CurrentEntityIndex >= gameState.Entities.Count - 1
                ? 0
                : gameState.CurrentEntityIndex + 1;
        
        List<IPacket> packets = new() { new PacketNextTurn() };

        // APPLY START TURN BUFF EFFECTS
        List<ActiveBuff> serverEffects = gameState.CurrentEntity.Buffs
            .Where(b => b.Buff.StartTurnEffects.Any())
            .ToList();

        foreach (ActiveBuff activeBuff in serverEffects)
        {
            foreach (ServerEffectBase startTurnEffect in activeBuff.Buff.StartTurnEffects)
            {
                packets.AddRange(startTurnEffect.Apply(
                    activeBuff.Launcher,
                    new List<Entity> { gameState.CurrentEntity },
                    gameState.CurrentEntity.GridPosition,
                    gameState,
                    GameManagerServer.Instance.Map));
            }
        }
        
        return packets;
    }
}
