using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class PacketSetTeam : IPacket
{
    [Key(0)] public Team Team { get; set; }
    
    public Task ApplyAsync()
    {
        GameManagerClient.Instance.Team = Team;
        return Task.CompletedTask;
    }
}