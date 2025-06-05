using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct GameInfo : IEquatable<GameInfo>, INetworkSerializable
{
    public int Seed;
    public bool UseRandSeed;
    public int MaxSegmentCount;
    public int RoomCount;
    public int StaffMin;
    public int BathMin;
    public float DiffSegBoost;

    public readonly bool Equals(GameInfo other)
    {
        return Seed == other.Seed
            && UseRandSeed == other.UseRandSeed
            && MaxSegmentCount == other.MaxSegmentCount
            && RoomCount == other.RoomCount
            && StaffMin == other.StaffMin
            && BathMin == other.BathMin
            && DiffSegBoost == other.DiffSegBoost;
    }

    public override readonly int GetHashCode()
    {
        return Seed.GetHashCode();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Seed);
        serializer.SerializeValue(ref UseRandSeed);
        serializer.SerializeValue(ref MaxSegmentCount);
        serializer.SerializeValue(ref RoomCount);
        serializer.SerializeValue(ref StaffMin);
        serializer.SerializeValue(ref BathMin);
        serializer.SerializeValue(ref DiffSegBoost);
    }
}
