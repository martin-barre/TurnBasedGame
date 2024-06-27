using System;
using Unity.Netcode;
using UnityEngine;

public class GameCommand : NetworkSingleton<GameCommand>
{
    public static event Action<Team> OnTeamReady;

    public void SendReadyEvent(Team team) => ReadyServerRpc(team);

    [ServerRpc(RequireOwnership = false)]
    private void ReadyServerRpc(Team team) => ReadyClientRpc(team);

    [ClientRpc]
    private void ReadyClientRpc(Team team)
    {
        Debug.Log("ReadyClientRpc : " + team);
        OnTeamReady?.Invoke(team);
    }

    public static event Action<int, Vector2Int> OnSpellLaunch;

    public void SendLaunchSpellEvent(int spellId, Vector2Int cellPos) => LaunchSpellServerRpc(spellId, cellPos);

    [ServerRpc(RequireOwnership = false)]
    private void LaunchSpellServerRpc(int spellId, Vector2Int cellPos) => LaunchSpellClientRpc(spellId, cellPos);

    [ClientRpc]
    private void LaunchSpellClientRpc(int spellId, Vector2Int cellPos)
    {
        Debug.Log("LaunchSpellClientRpc : " + spellId + ", " + cellPos);
        OnSpellLaunch?.Invoke(spellId, cellPos);
    }

    public static event Action<Vector2Int> OnMoveClick;

    public void SendMoveClickEvent(Vector2Int cellPos) => LaunchMoveClickServerRpc(cellPos);

    [ServerRpc(RequireOwnership = false)]
    private void LaunchMoveClickServerRpc(Vector2Int cellPos) => LaunchMoveClickClientRpc(cellPos);

    [ClientRpc]
    private void LaunchMoveClickClientRpc(Vector2Int cellPos)
    {
        Debug.Log("LaunchMoveClickClientRpc : " + cellPos);
        OnMoveClick?.Invoke(cellPos);
    }

    public static event Action OnNextTurn;

    public void SendNextTurnEvent() => NextTurnServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void NextTurnServerRpc() => NextTurnClientRpc();

    [ClientRpc]
    private void NextTurnClientRpc()
    {
        Debug.Log("NextTurnClientRpc");
        OnNextTurn?.Invoke();
    }

    public static event Action<Node, Node> OnSelectSpawn;

    public void SendSelectSpawnEvent(Node node1, Node node2) => SelectSpawnServerRpc(node1.gridPosition, node2.gridPosition);

    [ServerRpc(RequireOwnership = false)]
    private void SelectSpawnServerRpc(Vector2Int nodePos1, Vector2Int nodePos2) => SelectSpawnClientRpc(nodePos1, nodePos2);

    [ClientRpc]
    private void SelectSpawnClientRpc(Vector2Int nodePos1, Vector2Int nodePos2)
    {
        Debug.Log("SelectSpawnClientRpc");
        OnSelectSpawn?.Invoke(MapManager.Instance.GetNode(nodePos1), MapManager.Instance.GetNode(nodePos2));
    }
}
