using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerClient : MonoSingleton<GameManagerClient>
{
    public event Action<string> OnChatMessage;
    public GameState GameState;
    public Map Map;
    public Team Team;
    
    private Dictionary<int, EntityPrefabController> _entitiesPrefabs = new();

    private void Awake()
    {
        MapManager.Instance.InitializeMap();
        Map = MapManager.Instance.GetMap();
        GameState = new GameState
        {
            IsStarted = false,
            CurrentEntityIndex = -1
        };
    }
    
    public void SpawnEntity(int entityId, Team team, int raceId, Vector2Int gridPosition, bool isPlayer, int summonerId = -1)
    {
        Race race = RaceDatabase.GetById(raceId);
        Node node = Map.GetNode(gridPosition);

        if (node.NodeType != NodeType.Ground || GameState.GetEntityByGridPosition(node.GridPosition) != null) return;
        
        Entity entity = new()
        {
            Id = entityId,
            Team = team,
            GridPosition = gridPosition,
            Race = race,
            Hp = race.Hp,
            Pa = race.Pa,
            Pm = race.Pm,
            IsPlayer = isPlayer,
            Summoner = summonerId == -1 ? null : GameState.GetEntityById(summonerId)
        };
        
        if (entity.Summoner != null)
        {
            GameState.Entities.Insert(GameState.CurrentEntityIndex + 1, entity);
        }
        else
        {
            GameState.Entities.Add(entity);
        }
        ViewModelFactory.Game.NotifyUpdate(GameState);
        EntityPrefabController prefab = Instantiate(entity.Race.Prefab, node.WorldPosition, Quaternion.identity);
        _entitiesPrefabs.Add(entityId, prefab);
    }
    
    public void KillEntity(int entityId)
    {
        Entity entity = GameState.GetEntityById(entityId);
        if(entity == null) throw new Exception($"Entity with id {entityId} not found.");
        
        EntityPrefabController entityPrefab = GetEntityPrefab(entityId);
        if(entityPrefab == null) throw new Exception($"EntityPrefab with id {entityId} not found.");
        
        if (GameState.CurrentEntityIndex >= GameState.Entities.IndexOf(entity))
        {
            GameState.CurrentEntityIndex--;
        }
        
        GameState.Entities.ForEach(e =>
        {
            e.Buffs.RemoveAll(b => b.Launcher.Id == entity.Id);
            ViewModelFactory.Entity.NotifyUpdate(e);
        });
        GameState.Entities.Remove(entity);
        
        ViewModelFactory.Game.NotifyUpdate(GameState);
        
        Destroy(entityPrefab.gameObject);
    }

    public EntityPrefabController GetEntityPrefab(int entityId)
    {
        return _entitiesPrefabs[entityId];
    }

    public GameObject InstantiateObject(GameObject original, Vector3 position, Quaternion rotation)
    {
        return Instantiate(original, position, rotation);
    }

    public void SendChatMessage(string message)
    {
        OnChatMessage?.Invoke(message);
    }
}
