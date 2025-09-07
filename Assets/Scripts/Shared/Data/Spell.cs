using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SFX
{
    public TargetEnum target;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    [Header("GLOBAL")]
    public int id;
    public Sprite iconSprite;
    public string spellName;
    public int paCost;
    
    [Header("FOV")]
    public FovMode fovMode;
    public bool xRay;
    public bool canLaunchOnEntity;
    public int poMin;
    public int poMax;
    
    [Header("ZONE")]
    [SerializeReference] public SpellZone zone;
    
    [Header("EFFECTS")]
    public List<SFX> sfx;
    [SerializeReference] public List<ServerEffectBase> effects;

    public List<Node> GetZoneNodes(Vector2Int launcherGridPosition, Vector2Int targetGridPosition, Map map)
    {
        return zone.GetZonePositions(launcherGridPosition, targetGridPosition)
            .Select(position => map.GetNode(targetGridPosition + position))
            .Where(node => node is { NodeType: NodeType.Ground })
            .ToList();
    }

    public List<IPacket> Launch(Entity launcher, Spell spell, Vector2Int targetGridPosition, GameState gameState, Map map)
    {
        List<Entity> entities = GetZoneNodes(launcher.GridPosition, targetGridPosition, map)
            .Select(node => gameState.GetEntityByGridPosition(node.GridPosition))
            .Where(entity => entity != null)
            .ToList();

        List<IPacket> clientEffects = new();
        foreach (ServerEffectBase effect in effects)
        {
            clientEffects.AddRange(effect.Apply(launcher, spell, entities, targetGridPosition, gameState, map));
        }

        return clientEffects;
    }
}
