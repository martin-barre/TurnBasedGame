using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public readonly struct PacketSetTeam : IPacket
{
    [Key(0)] public readonly Team Team;
    
    [SerializationConstructor]
    public PacketSetTeam(Team team)
    {
        Team = team;
    }
    
    public Task ApplyAsync()
    {
        GameManagerClient.Instance.Team = Team;
        return Task.CompletedTask;
    }
}