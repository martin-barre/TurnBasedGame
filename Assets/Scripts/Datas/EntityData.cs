using System;
using Unity.Netcode;
using UnityEngine;

public struct EntityData : INetworkSerializable, IEquatable<EntityData>
{
    public int Id;
    public ERace RaceEnum;
    public Team Team;
    public Vector2Int Position;
    public int Hp;
    public int Pa;
    public int Pm;
    public int ParentId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref RaceEnum);
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Hp);
        serializer.SerializeValue(ref Pa);
        serializer.SerializeValue(ref Pm);
        serializer.SerializeValue(ref ParentId);
    }

    public bool Equals(EntityData other)
    {
        return Id == other.Id &&
               ParentId == other.ParentId &&
               RaceEnum == other.RaceEnum &&
               Team == other.Team &&
               Position == other.Position &&
               Hp == other.Hp &&
               Pa == other.Pa &&
               Pm == other.Pm;
    }
}
