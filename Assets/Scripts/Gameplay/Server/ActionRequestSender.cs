using System.Collections.Generic;
using MessagePack;
using Unity.Netcode;
using UnityEngine;

public class ActionRequestSender : NetworkSingleton<ActionRequestSender>
{
    [ServerRpc(RequireOwnership = false)]
    public void NotifyClientReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        GameManagerServer.Instance.SetupClientData(clientId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void MoveServerRpc(Vector2Int gridPosition, ServerRpcParams rpcParams = default)
    {
        if (!CanDoAction(rpcParams)) return;
        IPacket packet = GameServerAction.Move(gridPosition, GameManagerServer.Instance.GameState);
        ActionResultSender.Instance.SendEffectClientRpc(MessagePackSerializer.Serialize(packet));
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void LaunchSpellServerRpc(int spellId, Vector2Int targetPos, ServerRpcParams rpcParams = default)
    {
        if (!CanDoAction(rpcParams)) return;
        List<IPacket> clientEffects = GameServerAction.LaunchSpell(spellId, targetPos, GameManagerServer.Instance.GameState, GameManagerServer.Instance.Map);
        ActionResultSender.Instance.SendEffectsClientRpc(MessagePackSerializer.Serialize(clientEffects));
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void NextTurnServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!CanDoAction(rpcParams)) return;
        List<IPacket> packets = GameServerAction.NextTurn(GameManagerServer.Instance.GameState);
        ActionResultSender.Instance.SendEffectsClientRpc(MessagePackSerializer.Serialize(packets));
        GameManagerServer.Instance.TestPlayIA();
    }
    
    private bool CanDoAction(ServerRpcParams rpcParams)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        SessionPlayerData? sessionData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
        if (sessionData == null) return false;
        if (sessionData.Value.Team != GameManagerServer.Instance.GameState.CurrentEntity.Team) return false;
        return true;
    }
}
