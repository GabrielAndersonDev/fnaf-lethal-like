using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct SerializableGameInfo : INetworkSerializable, IEquatable<SerializableGameInfo>
{
    public int Seed;
    public bool UseRandSeed;
    public int MaxSegmentCount;
    public int RoomCount;
    public int StaffMin;
    public int BathMin;
    public float DiffSegBoost;

    public readonly bool Equals(SerializableGameInfo other)
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

public class GameInfo : ScriptableObject
{
    public int Seed;
    public bool UseRandSeed;
    public int MaxSegmentCount;
    public int RoomCount;
    public int StaffMin;
    public int BathMin;
    public float DiffSegBoost;

    public SerializableGameInfo GetSerializableGameInfo()
    {
        SerializableGameInfo info = new()
        {
            Seed = Seed,
            UseRandSeed = UseRandSeed,
            MaxSegmentCount = MaxSegmentCount,
            RoomCount = RoomCount,
            StaffMin = StaffMin,
            BathMin = BathMin,
            DiffSegBoost = DiffSegBoost
        };

        return info;
    }

    public GameInfo GetGameInfoFromSerialized(GameInfo gameInfo, SerializableGameInfo serialized)
    {
        if (gameInfo == null)
        {
            Debug.LogError("gameInfo is null");
            Debug.Break();
            return null;
        }

        gameInfo.Seed = serialized.Seed;
        gameInfo.UseRandSeed = serialized.UseRandSeed;
        gameInfo.MaxSegmentCount = serialized.MaxSegmentCount;
        gameInfo.RoomCount = serialized.RoomCount;
        gameInfo.StaffMin = serialized.StaffMin;
        gameInfo.BathMin = serialized.BathMin;
        gameInfo.DiffSegBoost = serialized.DiffSegBoost;

        return gameInfo;
    }
}