using System;
using Unity.Netcode;

public struct TeamSelectState : INetworkSerializable, IEquatable<TeamSelectState>
{
    public ulong ClientId;
    public ERace RaceId1;
    public ERace RaceId2;
    public ERace RaceId3;
    public bool Locked;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref RaceId1);
        serializer.SerializeValue(ref RaceId2);
        serializer.SerializeValue(ref RaceId3);
        serializer.SerializeValue(ref Locked);
    }

    public bool Equals(TeamSelectState other)
    {
        return ClientId == other.ClientId
            && RaceId1 == other.RaceId1
            && RaceId2 == other.RaceId2
            && RaceId3 == other.RaceId3
            && Locked == other.Locked;
    }
}