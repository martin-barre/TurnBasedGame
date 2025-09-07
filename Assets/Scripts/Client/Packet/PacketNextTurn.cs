using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class PacketNextTurn : IPacket
{
    public Task ApplyAsync()
    {
        Entity entity = GameManagerClient.Instance.GameState.CurrentEntity;
        entity.Pa = entity.Race.Pa;
        entity.Pm = entity.Race.Pm;
        entity.Buffs.ForEach(s =>
        {
            s.TurnDuration--;
            ViewModelFactory.ActiveBuff.NotifyUpdate(s);
        });
        entity.Buffs.RemoveAll(s => s.TurnDuration <= 0);
        
        GameManagerClient.Instance.GameState.CurrentEntityIndex =
            GameManagerClient.Instance.GameState.CurrentEntityIndex >= GameManagerClient.Instance.GameState.Entities.Count - 1
                ? 0
                : GameManagerClient.Instance.GameState.CurrentEntityIndex + 1;
        
        ViewModelFactory.Entity.NotifyUpdate(entity);
        ViewModelFactory.Game.NotifyUpdate(GameManagerClient.Instance.GameState);
        
        return Task.CompletedTask;
    }
}