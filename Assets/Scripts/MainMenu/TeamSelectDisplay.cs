using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class TeamSelectDisplay : NetworkBehaviour
{
    [SerializeField] private RaceDatabase _raceDatabase;
    [SerializeField] private Transform _raceHolder;
    [SerializeField] private RaceSelectButton _raceSelectButton;
    [SerializeField] private List<PlayerCardUI> _playerCards;
    [SerializeField] private Button _lockButton;
    [SerializeField] private TMP_Text _lockButtonText;

    private NetworkList<TeamSelectState> _players;
    private int _selectedRaceIndex = 0;

    private void Awake()
    {
        _players = new();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            foreach (Race race in _raceDatabase.Races)
            {
                RaceSelectButton button = Instantiate(_raceSelectButton, _raceHolder);
                button.SetRace(this, race);
            }

            _players.OnListChanged += HandlePlayersListChanged;
        }

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _players.OnListChanged -= HandlePlayersListChanged;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        _players.Add(new TeamSelectState
        {
            ClientId = clientId,
            RaceId1 = ERace.NONE,
            RaceId2 = ERace.NONE,
            RaceId3 = ERace.NONE,
            Locked = false
        });
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (TryGetPlayer(clientId, out TeamSelectState _, out int index))
        {
            _players.RemoveAt(index);
        }
    }

    public void SelectRaceIndex(int index)
    {
        _selectedRaceIndex = index;
    }

    public void SelectRace(Race race)
    {
        if (_selectedRaceIndex == -1) return;
        SelectRaceServerRpc(race.Enum, _selectedRaceIndex);
    }

    public void Lock()
    {
        if (TryGetPlayer(NetworkManager.Singleton.LocalClientId, out TeamSelectState teamSelectState, out int _))
        {
            if (teamSelectState.Locked || teamSelectState.RaceId1 == ERace.NONE || teamSelectState.RaceId2 == ERace.NONE || teamSelectState.RaceId3 == ERace.NONE) return;
            LockServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (TryGetPlayer(serverRpcParams.Receive.SenderClientId, out TeamSelectState teamSelectState, out int index))
        {
            if (teamSelectState.Locked || teamSelectState.RaceId1 == ERace.NONE || teamSelectState.RaceId2 == ERace.NONE || teamSelectState.RaceId3 == ERace.NONE) return;
            teamSelectState.Locked = true;
            _players[index] = teamSelectState;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectRaceServerRpc(ERace raceEnum, int index, ServerRpcParams serverRpcParams = default)
    {
        if (TryGetPlayer(serverRpcParams.Receive.SenderClientId, out TeamSelectState teamSelectState, out int playerIndex))
        {
            if (teamSelectState.Locked) return;
            if (index == 0) teamSelectState.RaceId1 = raceEnum;
            if (index == 1) teamSelectState.RaceId2 = raceEnum;
            if (index == 2) teamSelectState.RaceId3 = raceEnum;
            _players[playerIndex] = teamSelectState;
        }
    }

    private void HandlePlayersListChanged(NetworkListEvent<TeamSelectState> changeEvent)
    {
        for (int i = 0; i < _playerCards.Count; i++)
        {
            if(i < _players.Count)
            {
                _playerCards[i].UpdateUI(this, _players[i], _players[i].ClientId == NetworkManager.Singleton.LocalClientId);

                if (_players[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    _lockButtonText.text = _players[i].Locked ? "READY" : "LOCK";
                    _lockButton.interactable = !_players[i].Locked && _players[i].RaceId1 != ERace.NONE && _players[i].RaceId2 != ERace.NONE && _players[i].RaceId3 != ERace.NONE;
                }
            }
            else
            {
                _playerCards[i].DisableUI();
            }
        }
    }

    private bool TryGetPlayer(ulong clientId, out TeamSelectState teamSelectState, out int index)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId == clientId)
            {
                teamSelectState = _players[i];
                index = i;
                return true;
            }
        }
        teamSelectState = default;
        index = -1;
        return false;
    }
}
