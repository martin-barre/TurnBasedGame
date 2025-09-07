using System.Threading.Tasks;
using MessagePack;

[Union(0, typeof(PacketBuff))]
[Union(1, typeof(PacketDamage))]
[Union(2, typeof(PacketHeal))]
[Union(3, typeof(PacketKillEntity))]
[Union(4, typeof(PacketLaunchSpell))]
[Union(5, typeof(PacketMove))]
[Union(6, typeof(PacketNextTurn))]
[Union(7, typeof(PacketSetGameLogic))]
[Union(8, typeof(PacketSetTeam))]
[Union(9, typeof(PacketSummonEntity))]
[Union(10, typeof(PacketTeleport))]
public interface IPacket
{
    public Task ApplyAsync();
}
