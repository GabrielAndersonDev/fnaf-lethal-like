using Unity.Entities;
using Unity.Netcode;
using UnityEngine;

public struct SerializableEnemyData : INetworkSerializable
{
    public int enemyID;
    public EnemyType enemyType;
    public Team team;
    public bool isDeactivated;

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref enemyID);
        serializer.SerializeValue(ref enemyType);
        serializer.SerializeValue(ref team);
        serializer.SerializeValue(ref isDeactivated);
    }
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int enemyID;
    public string enemyName;
    public EnemyType enemyType;
    public Team team;
    public bool isDeactivated;

    public RoomType spawnRoom;
}
