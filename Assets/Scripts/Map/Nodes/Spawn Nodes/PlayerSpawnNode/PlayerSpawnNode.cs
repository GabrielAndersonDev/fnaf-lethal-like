using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnNode : SpawnNode
{
    [SerializeField]
    GameObject spawnArea;

    private void Awake()
    {
        PlayerManager.Singleton.spawnNode = this;
    }

    // Spawn location will be randomly generated within the spawn area, make sure to calculate space for player model

    public Vector3 GiveSpawnLocation(GameObject player)
    {
        if (spawnArea == null)
        {
            Debug.LogError("Spawn area is not assigned in the inspector.");
            Debug.Break();
            return Vector3.zero;
        }

        float playerX = player.transform.position.x + player.transform.localScale.x;

        float playerY = player.transform.position.y + player.transform.localScale.y;

        float playerZ = player.transform.position.z + player.transform.localScale.z;

        Vector3 randomPosition = new(
            Random.Range(spawnArea.transform.position.x - spawnArea.transform.localScale.x / 2 + playerX, spawnArea.transform.position.x + spawnArea.transform.localScale.x / 2 - playerX),
            spawnArea.transform.position.y + playerY,
            Random.Range(spawnArea.transform.position.z - spawnArea.transform.localScale.z / 2 + playerZ, spawnArea.transform.position.z + spawnArea.transform.localScale.z / 2 - playerZ));

        Debug.Log(randomPosition);

        return randomPosition;
    }
}
