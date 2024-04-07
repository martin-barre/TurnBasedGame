using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public static event Action OnEntitiesChanged;

    [SerializeField] private List<Race> blueEntities;
    [SerializeField] private List<Race> redEntities;

    public Team CurrentTeam;

    private List<Entity> entities;

    public override void OnNetworkSpawn()
    {
        GetTeamServerRpc();
        entities = new List<Entity>();

        for (int i = 0; i < blueEntities.Count; i++)
        {
            Node blueNode = MapManager.Instance.GetRandomSpawns(Team.BLUE);
            Node redNode = MapManager.Instance.GetRandomSpawns(Team.RED);

            AddEntity(redEntities[i], Team.RED, redNode);
            AddEntity(blueEntities[i], Team.BLUE, blueNode);
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

    [ServerRpc(RequireOwnership = false)]
    private void GetTeamServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (!IsOwner) return;

        var number = NetworkManager.ConnectedClients.Count;
        GetTeamClientRpc(number % 2 == 0 ? Team.BLUE : Team.RED, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        });
    }

    [ClientRpc]
    private void GetTeamClientRpc(Team team, ClientRpcParams _ = default)
    {
        Debug.Log("GetTeamClientRpc : " + team);
        CurrentTeam = team;
    }
}
