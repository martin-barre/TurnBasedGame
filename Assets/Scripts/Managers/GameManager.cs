using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public static event Action OnEntitiesChanged;
    public static event Action OnDataInitialized;

    [SerializeField] private List<Race> allRaces;
    [SerializeField] private List<ERace> blueEntities;
    [SerializeField] private List<ERace> redEntities;

    public NetworkVariable<int> CurrentPlayerIndex = new();
    public NetworkVariable<bool> RedTeamReady = new();
    public NetworkVariable<bool> BlueTeamReady = new();

    public Team CurrentTeam;

    private List<Entity> entities = new();
    private int entityId = 0;

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            for (int i = 0; i < blueEntities.Count; i++)
            {
                Node blueNode = MapManager.Instance.GetRandomSpawns(Team.BLUE);
                Node redNode = MapManager.Instance.GetRandomSpawns(Team.RED);

                AddEntity(new EntityData
                {
                    RaceEnum = blueEntities[i],
                    Team = Team.BLUE,
                    Position = blueNode.gridPosition,
                    Hp = GetRace(blueEntities[i]).Hp,
                    Pa = GetRace(blueEntities[i]).Pa,
                    Pm = GetRace(blueEntities[i]).Pm,
                    ParentId = -1
                });

                AddEntity(new EntityData
                {
                    RaceEnum = redEntities[i],
                    Team = Team.RED,
                    Position = redNode.gridPosition,
                    Hp = GetRace(blueEntities[i]).Hp,
                    Pa = GetRace(blueEntities[i]).Pa,
                    Pm = GetRace(blueEntities[i]).Pm,
                    ParentId = -1
                });
            }
        }

        OnEntitiesChanged?.Invoke();
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Team team = NetworkManager.ConnectedClients.Count % 2 == 0 ? Team.BLUE : Team.RED;
        EntityData[] entitiesData = entities.Select(e => e.data).ToArray();
        SendDataClientRpc(team, entitiesData, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    public void AddEntity(EntityData entityData)
    {
        int index = entityData.ParentId < 0 ? entities.Count : entities.FindIndex(e => e.data.Id == entityData.ParentId) + 1;
        entityData.Id = entityId++;
        Race race = GetRace(entityData.RaceEnum);
        Node node = MapManager.Instance.GetNode(entityData.Position);
        Entity entity = Instantiate(race.Prefab, node.worldPosition, Quaternion.identity);
        entity.data = entityData;
        node.entity = entity;
        entities.Insert(index, entity);
        OnEntitiesChanged?.Invoke();
    }

    public void RemoveEntity(Entity entity)
    {
        List<Entity> children = entities.Where(e => e.data.ParentId == entity.data.Id).ToList();
        foreach (Entity child in children) RemoveEntity(child);

        entity.Node.entity = null;
        entities.Remove(entity);
        Destroy(entity.gameObject);
        OnEntitiesChanged?.Invoke();
    }

    public List<Entity> GetEntities()
    {
        return entities;
    }

    public Race GetRace(ERace raceEnum)
    {
        return allRaces.SingleOrDefault(r => r.Enum == raceEnum);
    }

    [ClientRpc]
    public void SendDataClientRpc(Team team, EntityData[] entitiesData, ClientRpcParams _ = default)
    {
        Debug.Log("SendDataClientRpc");
        Debug.Log("SendDataClientRpc : team : " + team);
        Debug.Log("SendDataClientRpc : entitiesData : " + entitiesData);

        CurrentTeam = team;

        if (!IsOwner)
        {
            foreach (var entityData in entitiesData)
            {
                AddEntity(entityData);
            }
        }
        OnDataInitialized?.Invoke();
    }
}
