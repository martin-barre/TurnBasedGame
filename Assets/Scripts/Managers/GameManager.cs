using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action OnEntitiesChanged;

    [SerializeField] private List<Race> blueEntities;
    [SerializeField] private List<Race> redEntities;

    private List<Entity> entities;

    private void Start()
    {
        entities = new List<Entity>();

        float random = UnityEngine.Random.Range(0, 1);
        var startTeam = random < .5 ? Team.BLUE : Team.RED;

        for (int i = 0; i < blueEntities.Count; i++)
        {
            Node blueNode = MapManager.Instance.GetRandomSpawns(Team.BLUE);
            Node redNode = MapManager.Instance.GetRandomSpawns(Team.RED);

            if (startTeam == Team.BLUE)
            {
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
                AddEntity(redEntities[i], Team.RED, redNode);
            }
            else
            {
                AddEntity(redEntities[i], Team.RED, redNode);
                AddEntity(blueEntities[i], Team.BLUE, blueNode);
            }
        }

        OnEntitiesChanged?.Invoke();
    }

    public void AddEntity(Race race, Team team, Node spawnNode)
    {
        GameObject newPlayer = Instantiate(race.prefab, spawnNode.worldPosition, Quaternion.identity);
        Entity entity = newPlayer.GetComponent<Entity>();
        entity.team = team;
        entity.node = spawnNode;
        entity.CurrentHp = entity.race.hp;
        entity.CurrentPa = entity.race.pa;
        entity.CurrentPm = entity.race.pm;
        spawnNode.entity = entity;
        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entity.node.entity = null;
        Destroy(entity.gameObject);
    }

    public List<Entity> GetEntities()
    {
        return entities;
    }
}
