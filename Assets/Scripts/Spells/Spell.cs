using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    public Sprite iconSprite;
    public bool isProjectile;
    public string spellName;
    public FovMode fovMode;
    public bool xRay;
    public bool canLaunchOnEntity;
    public int poMin;
    public int poMax;
    public int paCost;
    [SerializeField] private SpellZone zone;

    [Header("ALL VFXs")]
    [SerializeField] private GameObject launcherVfx;
    [SerializeField] private GameObject projectileVfx;
    [SerializeField] private GameObject targetVfx;

    [Header("ALL EFFECTS")]
    [SerializeField] private List<EffectDamage> damage;
    [SerializeField] private List<EffectPush> push;
    [SerializeField] private List<EffectAttract> attract;
    [SerializeField] private List<EffectTeleport> teleport;
    [SerializeField] private List<EffectApplyBuff> applyBuff;

    public List<Node> GetZoneNodes(Entity launcher, Node targetNode)
    {
        List<Node> nodes = new List<Node>();
        foreach (Vector2Int position in zone.GetPositions(launcher, targetNode))
        {
            Node node = MapManager.Instance.GetNode(targetNode.gridPosition + position);
            if (node.type != NodeType.GROUND) continue;
            nodes.Add(node);
        }
        return nodes;
    }

    public void Launch(Entity launcher, Spell spell, Vector2Int targetPos)
    {
        LaunchVfx(launcher, targetPos);

        SortedDictionary<int, Effect> effects = new SortedDictionary<int, Effect>();
        foreach (Effect effect in damage) effects.Add(effect.order, effect);
        foreach (Effect effect in push) effects.Add(effect.order, effect);
        foreach (Effect effect in attract) effects.Add(effect.order, effect);
        foreach (Effect effect in teleport) effects.Add(effect.order, effect);
        foreach (Effect effect in applyBuff) effects.Add(effect.order, effect);

        List<Entity> entities = new List<Entity>();
        List<Node> nodes = GetZoneNodes(launcher, MapManager.Instance.GetNode(targetPos));
        foreach (Node node in nodes)
        {
            if (node.entity != null)
            {
                entities.Add(node.entity);
            }
        }
        foreach (KeyValuePair<int, Effect> entry in effects)
        {
            entry.Value.Apply(launcher, spell, entities, targetPos);
        }
    }

    private void LaunchVfx(Entity launcher, Vector2Int targetPos)
    {
        if (launcherVfx != null)
        {
            Instantiate(launcherVfx, launcher.transform.position + new Vector3(0, 0, 200), Quaternion.identity);
        }

        if (projectileVfx != null)
        {
            Vector3 start = launcher.transform.position + new Vector3(0, 0, 200);
            Vector3 end = MapManager.Instance.GetNode(targetPos).worldPosition + new Vector3(0, 0, 200);
            var vfx = Instantiate(projectileVfx, start, Quaternion.identity);
            vfx.transform
                .DOMove(end, 0.4f)
                .OnComplete(() =>
                {
                    if (targetVfx != null)
                    {
                        Instantiate(targetVfx, MapManager.Instance.GetNode(targetPos).worldPosition, Quaternion.identity);
                    }
                    Destroy(vfx);
                });
        }

        if (targetVfx != null && projectileVfx == null)
        {
            Instantiate(targetVfx, MapManager.Instance.GetNode(targetPos).worldPosition + new Vector3(0, 0, 200), Quaternion.identity);
        }
    }
}
