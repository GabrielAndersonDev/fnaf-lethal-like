using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int enemyID;
    public Team team;
    public bool isDeactivated;
    public RoomType spawnRoom;

    public GameObject enemyPrefab;
}
