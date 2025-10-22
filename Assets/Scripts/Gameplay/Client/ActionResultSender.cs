using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using Unity.Netcode;

public class ActionResultSender : NetworkSingleton<ActionResultSender>
{
    private readonly Queue<IPacket[]> _bufferEffects = new();
    private bool _isProcessing;

    private void Update()
    {
        if (!_isProcessing && _bufferEffects.Count > 0)
        {
            StartCoroutine(ApplyEffectsCoroutine(_bufferEffects.Dequeue()));
        }
    }

    [ClientRpc]
    public void SendEffectClientRpc(byte[] serializedEffect, ClientRpcParams _ = default)
    {
        IPacket effect = MessagePackSerializer.Deserialize<IPacket>(serializedEffect);
        _bufferEffects.Enqueue(new []{ effect });
    }
    
    [ClientRpc]
    public void SendEffectsClientRpc(byte[] serializedEffects, ClientRpcParams _ = default)
    {
        IPacket[] effects = MessagePackSerializer.Deserialize<IPacket[]>(serializedEffects);
        _bufferEffects.Enqueue(effects);
    }

    private IEnumerator ApplyEffectsCoroutine(IPacket[] effects)
    {
        _isProcessing = true;
        foreach (IPacket effect in effects)
        {
            Task task = effect.ApplyAsync();
            while (!task.IsCompleted) yield return null;
        }
        _isProcessing = false;

        InteractionManager.Instance.DisplayMovementNode();
    }
}
