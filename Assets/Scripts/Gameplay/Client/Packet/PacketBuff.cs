using System;
using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public readonly struct PacketBuff : IPacket
{
    [Key(0)] public readonly int TargetId;
    [Key(1)] public readonly int LauncherId;
    [Key(2)] public readonly int BuffId;
    
    [SerializationConstructor]
    public PacketBuff(int targetId, int launcherId, int buffId)
    {
        TargetId = targetId;
        LauncherId = launcherId;
        BuffId = buffId;
    }
    
    public Task ApplyAsync()
    {
        Entity target = GameManagerClient.Instance.GameState.GetEntityById(TargetId);
        if(target == null) throw new Exception($"Entity with id {TargetId} not found.");
        
        Entity launcher = GameManagerClient.Instance.GameState.GetEntityById(LauncherId);
        if(launcher == null) throw new Exception($"Entity with id {LauncherId} not found.");
        
        Buff buff = BuffDatabase.GetById(BuffId);
        if(buff == null) throw new Exception($"Buff with id {BuffId} not found.");

        target.Buffs.Add(new ActiveBuff
        {
            Buff = buff,
            TurnDuration = buff.TurnDuration,
            Launcher = launcher
        });
        
        ViewModelFactory.Entity.NotifyUpdate(target);
        GameManagerClient.Instance.SendChatMessage($"<color=#FF0000>{target.Race.Name}</color> gagne <color=#00FF00>{buff.Name}");
        
        return Task.CompletedTask;
    }
}