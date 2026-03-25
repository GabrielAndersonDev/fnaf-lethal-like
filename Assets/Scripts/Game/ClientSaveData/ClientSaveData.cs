using Unity.Netcode;
using UnityEngine;

public struct SerializedClientSaveData : INetworkSerializable
{
    public int sessionSeed;  // int for double-checking session. re-creates each load before proper seeding works.
    public PlayerProfileData playerProfileData;
    public float health;
    public bool isNewSpawn;
    public Vector3 location;
    public Quaternion rotation;
    public bool isDead;

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref sessionSeed);
        serializer.SerializeValue(ref playerProfileData);
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref location);
        serializer.SerializeValue(ref rotation);
        serializer.SerializeValue(ref isDead);
    }
}

[System.Serializable]
public class ClientSaveData
{
    public int sessionSeed;
    public PlayerProfileData playerProfileData;
    public float health;
    public bool isNewSpawn;
    public Vector3 location;
    public Quaternion rotation;
    public bool isDead;

    public SerializedClientSaveData GetSerializedClientData()
    {
        return new SerializedClientSaveData()
        {
            sessionSeed = sessionSeed,
            playerProfileData = playerProfileData,
            health = health,
            location = location,
            rotation = rotation,
            isDead = isDead
        };
    }

    public void GetClientSaveFromSerialized(SerializedClientSaveData serializedClientSaveData)
    {
        sessionSeed = serializedClientSaveData.sessionSeed;
        playerProfileData = serializedClientSaveData.playerProfileData;
        health = serializedClientSaveData.health;
        location = serializedClientSaveData.location;
        rotation = serializedClientSaveData.rotation;
        isDead = serializedClientSaveData.isDead;
    }
}