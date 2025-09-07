using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class RaceToSpawn
{
    public Team Team;
    public Race Race;
    public bool IsPlayer;
}

public class GameManagerServer : MonoSingleton<GameManagerServer>
{
    public GameState GameState;
    public Map Map;
    public List<RaceToSpawn> RacesToSpawn;
    
    private void Awake()
    {
        MapManager.Instance.InitializeMap();
        Map = MapManager.Instance.GetMap();
        GameState = new GameState
        {
            IsStarted = false,
            CurrentEntityIndex = -1
        };
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
    }

    private void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        // Si deux joueur lance la partie
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2 && !GameState.IsStarted)
        {
            SessionManager<SessionPlayerData>.Instance.OnSessionStarted();

            for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
            {
                ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];
                SessionPlayerData sessionData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId) ?? default;
                sessionData.Team = i % 2 == 0 ? Team.Red : Team.Blue;
                SessionManager<SessionPlayerData>.Instance.SetPlayerData(clientId, sessionData);
                IPacket packetSetTeam = new PacketSetTeam { Team = sessionData.Team };
                ActionResultSender.Instance.SendEffectClientRpc(MessagePackSerializer.Serialize(packetSetTeam), new ClientRpcParams 
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { clientId }
                    }
                });
            }
            
            List<IPacket> effects = new();
            foreach (RaceToSpawn raceToSpawn in RacesToSpawn)
            {
                List<Node> nodes = raceToSpawn.Team == Team.Blue ? Map.SpawnsBlue : Map.SpawnsRed;
                Node node = nodes.FirstOrDefault(node => GameState.GetEntityByGridPosition(node.GridPosition) == null);
                effects.Add(SpawnEntity(raceToSpawn.Team, raceToSpawn.Race.Id, node.GridPosition, raceToSpawn.IsPlayer, GameState));
            }
            
            GameState.IsStarted = true;
            GameState.CurrentEntityIndex = 0;
            effects.Add(new PacketSetGameLogic
            {
                IsStarted = GameState.IsStarted,
                CurrentPlayer = GameState.CurrentEntityIndex
            });
            
            ActionResultSender.Instance.SendEffectsClientRpc(MessagePackSerializer.Serialize(effects.Where(e => e != null).ToArray()));
        }
    }

    public PacketSummonEntity SpawnEntity(Team team, int raceId, Vector2Int gridPosition, bool isPlayer, GameState gameState, Entity summoner = null)
    {
        Race race = RaceDatabase.GetById(raceId);
        Node node = Map.GetNode(gridPosition);

        if (node.NodeType != NodeType.Ground || gameState.GetEntityByGridPosition(node.GridPosition) != null) return null;
        
        Entity entity = new()
        {
            Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0),
            Team = team,
            GridPosition = gridPosition,
            Race = race,
            Hp = race.Hp,
            Pa = race.Pa,
            Pm = race.Pm,
            IsPlayer = isPlayer,
            Summoner = summoner
        };
        
        if (entity.Summoner != null)
        {
            gameState.Entities.Insert(gameState.CurrentEntityIndex + 1, entity);
        }
        else
        {
            gameState.Entities.Add(entity);
        }

        return new PacketSummonEntity
        {
            EntityId = entity.Id,
            Team = team,
            RaceId = race.Id,
            GridPosition = gridPosition,
            IsPlayer = isPlayer,
            SummonerId = summoner?.Id ?? -1,
        };
    }
    
    public PacketKillEntity KillEntity(Entity entity, GameState gameState)
    {
        if (gameState.CurrentEntityIndex >= gameState.Entities.IndexOf(entity))
        {
            gameState.CurrentEntityIndex--;
        }
        
        gameState.Entities.ForEach(e => e.Buffs.RemoveAll(b => b.Launcher.Id == entity.Id));
        gameState.Entities.Remove(entity);

        return new PacketKillEntity
        {
            TargetId = entity.Id
        };
    }

    public void TestPlayIA()
    {
        StartCoroutine(PlayAI());
    }

    private IEnumerator PlayAI()
    {
        while(!GameState.CurrentEntity.IsPlayer)
        {
            List<IPacket> clientEffects = AiManager.Play(GameState.CurrentEntity, GameState, Map);
            ActionResultSender.Instance.SendEffectsClientRpc(MessagePackSerializer.Serialize(clientEffects).ToArray());
            yield return null;
        }
    }
}
