using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public static class AiManager
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
                packets.AddRange(action.Apply(gameState));
            }
        }
        packets.Add(GameServerAction.NextTurn(gameState));
        return packets;
    }
    
    private static float CalculateSquenceScore(List<IAiAction> sequence, int entityId, GameState gameState, Map map)
    {
        Entity entity = gameState.GetEntityById(entityId);
        float score = 0;

        foreach (IAiAction action in sequence)
        {
            score += CalculateActionScore(action, entityId, gameState);
        }
        
        // CALCULER LA DISTANCE DU JOUEUR LE PLUS PROCHE
        float bestDistance = 0;
        foreach (Entity target in gameState.Entities.Where(e => e.Team != entity.Team))
        {
            List<Node> path = BFS.GetPath(entity.GridPosition, target.GridPosition, gameState, map, true);
            if(path == null) continue;
            float distance = Mathf.Max(0f, 1f - 0.1f * Mathf.Abs(path.Count - entity.Race.Pm));
            if (distance > bestDistance)
            {
                bestDistance = distance;
            }
        }
        score += bestDistance;
        
        // PREND EN COMPTE LA TAILLE DE LA SEQUENCE POUR PREFERE LES PLUS PETITES
        score += sequence.Count == 0 ? 1f : 0.9f / sequence.Count;

        return score;
    }
    
    private static float CalculateActionScore(IAiAction action, int entityId, GameState gameState)
    {
        GameState gameStateClone = gameState.Clone();
        Entity entity = gameState.GetEntityById(entityId);
        List<IPacket> packets = action.Apply(gameState);
        float score = 0;

        // CALCULER LE SCORE
        foreach (IPacket packet in packets)
        {
            if (packet is PacketDamage packetDamage)
            {
                Entity target = gameStateClone.GetEntityById(packetDamage.TargetId);
                score += target.Team == entity.Team ? -packetDamage.Value : packetDamage.Value;
            }
            else if (packet is PacketHeal packetHeal)
            {
                Entity target = gameStateClone.GetEntityById(packetHeal.TargetId);
                score += target.Team == entity.Team ? packetHeal.Value : -packetHeal.Value;
            }
        }

        return score;
    }
    
    private static List<List<IAiAction>> GenerateSequencesRecursive(int entityId, GameState gameState, Map map, List<IAiAction> currentSequence)
    {
        List<List<IAiAction>> sequences = new() { currentSequence };
        List<IAiAction> possibleActions = GetPossibleActions(currentSequence.LastOrDefault(), entityId, gameState, map);
        
        float bestScore = float.MinValue;
        foreach (IAiAction action in possibleActions)
        {
            GameState gameStateClone = gameState.Clone();
            List<IPacket> packets = action.Apply(gameStateClone);
        
            if (packets.Count == 0) continue;
            if (packets.Count == 1 && packets[0] is not PacketMove) continue;
            
            float score = CalculateActionScore(action, entityId, gameState.Clone());
            if (score >= bestScore)
            {
                List<IAiAction> newSequence = new(currentSequence) { action };
                sequences.AddRange(GenerateSequencesRecursive(entityId, gameStateClone, map, newSequence));
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
                .Select(n => new AiActionLaunchSpell { SpellId = spell.id, GridPosition = n.GridPosition })
                .ToList());
        }
        return actions;
    }
}
