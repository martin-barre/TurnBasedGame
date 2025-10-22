using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public readonly struct PacketNextTurn : IPacket
{
    public Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.CurrentEntity;
        entity.Pa = entity.Race.Pa;
        entity.Pm = entity.Race.Pm;
        entity.Buffs.RemoveAll(s => s.TurnDuration is 0 or 1);
        entity.Buffs.ForEach(s =>
        {
            if (s.TurnDuration != -1)
            {
                s.TurnDuration--;
                ViewModelFactory.ActiveBuff.NotifyUpdate(s);
            }
        });
        
        GameManagerClient.Instance.GameState.CurrentEntityIndex =
            GameManagerClient.Instance.GameState.CurrentEntityIndex >= GameManagerClient.Instance.GameState.Entities.Count - 1
                ? 0
                : GameManagerClient.Instance.GameState.CurrentEntityIndex + 1;
        
        ViewModelFactory.Entity.NotifyUpdate(entity);
        ViewModelFactory.Game.NotifyUpdate(GameManagerClient.Instance.GameState);
        
        return Task.CompletedTask;
    }
}