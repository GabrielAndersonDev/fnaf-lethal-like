using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    public void EnemyBatchcastCheck(Enemy enemy, GameObject eyePoint, Player player)
    {
        int raycastCount = 5;

        if (player == null)
        {
            Debug.LogError("Player reference is null in EnemyBatchcastCheck.");
            Debug.Break();
        }

        if (eyePoint == null)
        {
            Debug.LogError("EyePoint reference is null in EnemyBatchcastCheck.");
            Debug.Break();
        }

        var commands = new NativeArray<RaycastCommand>(raycastCount, Allocator.TempJob);
        var results = new NativeArray<RaycastHit>(raycastCount, Allocator.TempJob);

        for (int i = 0; i < raycastCount; i++)
        {
            Vector3 origin = eyePoint.transform.position;
            Vector3 direction = (player.raycastNodes[i].transform.position - origin).normalized;
            float distance = Vector3.Distance(origin, player.raycastNodes[i].transform.position);
            commands[i] = new RaycastCommand(origin, direction, QueryParameters.Default, distance);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default);
        handle.Complete();

        enemy.ProcessRaycastHit(eyePoint, player, results);

        commands.Dispose();
        results.Dispose();
    }
}
