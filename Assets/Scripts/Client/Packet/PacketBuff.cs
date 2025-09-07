using System;
using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class PacketBuff : IPacket
{
    [Key(0)] public int TargetId { get; set; }
    [Key(1)] public int LauncherId { get; set; }
    [Key(2)] public int BuffId { get; set; }
    
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
        GameManagerClient.Instance.SendChatMessage($"Buff {buff.Name} added to {target.Race.Name}");
        
        return Task.CompletedTask;
    }
}