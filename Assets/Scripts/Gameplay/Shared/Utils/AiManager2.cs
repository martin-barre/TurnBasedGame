using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AiManager2
{
    public static List<IPacket> Play(Entity entity, GameState gameState, Map map)
    {
        List<List<IAiAction>> sequences = GenerateSequencesRecursive(entity.Id, gameState, map, new List<IAiAction>());
        
        List<IAiAction> bestSequence = null;
        float bestScore = float.MinValue;
        
        foreach (List<IAiAction> sequence in sequences)
        {
            float score = CalculateSquenceScore(sequence, entity.Id, gameState.Clone(), map);

            if (score > bestScore)
            {
                bestScore = score;
                bestSequence = sequence;
            }
        }

        // Exécuter la meilleure sequence d'action + passer le tour
        List<IPacket> packets = new ();
        if (bestSequence != null)
        {
            foreach (IAiAction action in bestSequence)
            {
                packets.AddRange(action.Apply(gameState, map));
            }
        }
        packets.AddRange(GameServerAction.NextTurn(gameState));
        return packets;
    }
    
    private static float CalculateSquenceScore(List<IAiAction> sequence, int entityId, GameState gameState, Map map)
    {
        Entity entity = gameState.GetEntityById(entityId);
        float score = 0;

        foreach (IAiAction action in sequence)
        {
            score += CalculateActionScore(action, entityId, gameState, map);
        }
        
        // CALCULER LA DISTANCE DU JOUEUR LE PLUS PROCHE
        float bestDistance = 0;
        foreach (Entity target in gameState.Entities.Where(e => e.Team != entity.Team))
        {
            List<Node> path = BFS.GetPath(entity.GridPosition, target.GridPosition, gameState, map, true);
            if(path == null) continue;
            float distance = 100f / path.Count;
            if (distance > bestDistance)
            {
                bestDistance = distance;
            }
        }
        score += bestDistance;
        
        // PREND EN COMPTE LA TAILLE DE LA SEQUENCE POUR PREFERE LES PLUS PETITES
        //score += sequence.Count == 0 ? 1f : 0.9f / sequence.Count;

        score += entity.Pm;
        score += entity.Pa;

        return score;
    }
    
    private static float CalculateActionScore(IAiAction action, int entityId, GameState gameState, Map map)
    {
        GameState gameStateClone = gameState.Clone();
        Entity entity = gameState.GetEntityById(entityId);
        List<IPacket> packets = action.Apply(gameState, map);
        float score = 0;

        // CALCULER LE SCORE
        foreach (IPacket packet in packets)
        {
            if (packet is PacketDamage packetDamage)
            {
                Entity target = gameStateClone.GetEntityById(packetDamage.TargetId);
                score += 2 * (target.Team == entity.Team ? -packetDamage.Value : packetDamage.Value);
            }
            else if (packet is PacketHeal packetHeal)
            {
                Entity target = gameStateClone.GetEntityById(packetHeal.TargetId);
                score += 2 * (target.Team == entity.Team ? packetHeal.Value : -packetHeal.Value);
            }
        }

        return score;
    }
    
    private static List<List<IAiAction>> GenerateSequencesRecursive(int entityId, GameState gameState, Map map, List<IAiAction> currentSequence, int depth = 0)
    {
        List<List<IAiAction>> sequences = new() { currentSequence };
        List<IAiAction> possibleActions = GetPossibleActions(currentSequence.LastOrDefault(), entityId, gameState, map);
        
        float bestScore = float.MinValue;
        foreach (IAiAction action in possibleActions)
        {
            GameState gameStateClone = gameState.Clone();
            List<IPacket> packets = action.Apply(gameStateClone, map);
        
            if (packets.Count == 0) continue;
            if (packets.Count == 1 && packets[0] is PacketMove) continue;
            
            float score = CalculateActionScore(action, entityId, gameState.Clone(), map);
            if (score >= bestScore && depth < 6)
            {
                List<IAiAction> newSequence = new(currentSequence) { action };
                sequences.AddRange(GenerateSequencesRecursive(entityId, gameStateClone, map, newSequence, ++depth));
                bestScore = score;
            }
        }

        return sequences;
    }
    
    private static List<IAiAction> GetPossibleActions(IAiAction lastActions, int entityId, GameState gameState, Map map)
    {
        Entity entity = gameState.GetEntityById(entityId);
        List<IAiAction> actions = new();
        if (lastActions is not AiActionMove)
        {
            actions.AddRange(BFS.GetDisplacement(entity.GridPosition, entity.Pm, gameState, map)
                .Select(n => new AiActionMove { GridPosition = n.GridPosition })
                .ToList());
        }
        
        foreach (Spell spell in entity.Race.Spells.Where(spell => entity.Pa >= spell.paCost))
        {
            List<Node> test = FOV.GetDisplacement(entity, spell, gameState, map);
            actions.AddRange(test
                .Where(n => spell.GetTouchedEntities(entity.GridPosition, n.GridPosition, gameState, map).Any())
                .Select(n => new AiActionLaunchSpell { SpellId = spell.Id, GridPosition = n.GridPosition })
                .ToList());
        }
        return actions;
    }
}
