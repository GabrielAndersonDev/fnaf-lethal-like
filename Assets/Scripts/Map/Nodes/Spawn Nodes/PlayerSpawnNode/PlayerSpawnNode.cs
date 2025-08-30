using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnNode : SpawnNode
{
    [SerializeField]
    GameObject spawnArea;

    // Spawn location will be randomly generated within the spawn area, make sure to calculate space for player model

    public Vector3 GiveSpawnLocation(Player player)
    {
        if (spawnArea == null)
        {
            Debug.LogError("Spawn area is not assigned in the inspector.");
            Debug.Break();
            return Vector3.zero;
        }

        float playerX = player.gameObject.transform.position.x + player.gameObject.transform.localScale.x;

        float playerZ = player.gameObject.transform.position.z + player.gameObject.transform.localScale.z;

        Vector3 randomPosition = new(
            Random.Range(spawnArea.transform.position.x - spawnArea.transform.localScale.x + playerX / 2, spawnArea.transform.position.x + spawnArea.transform.localScale.x - playerX / 2),
            spawnArea.transform.position.y,
            Random.Range(spawnArea.transform.position.z - spawnArea.transform.localScale.z + playerZ / 2, spawnArea.transform.position.z + spawnArea.transform.localScale.z - playerZ / 2));

        return randomPosition;
    }
}
