using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRaycast : MonoBehaviour
{
    public List<EnemyVisionCone> activeVisionCones;

    private void Start()
    {
        activeVisionCones = new List<EnemyVisionCone>();
    }

    private void Update()
    {
        EnemySpherecastJob();
    }

    public void EnemySpherecastJob()
    {
        if (activeVisionCones.Count > 0)
        {
            int raycastCount = 0;

            foreach (EnemyVisionCone cone in activeVisionCones)
            {
                raycastCount += cone.playersInVision.Count;
            }

            QueryParameters hitMultipleFaces = new()
            {
                hitMultipleFaces = true
            };

            var commands = new NativeArray<SpherecastCommand>(raycastCount, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(raycastCount, Allocator.TempJob);

            for (int i = 0, index = 0; i < activeVisionCones.Count; i++)
            {
                EnemyVisionCone cone = activeVisionCones[i];

                foreach (Player player in cone.playersInVision)
                {
                    Vector3 origin = cone.eyePoint.transform.position;
                    Vector3 direction = (player.transform.position - origin).normalized;
                    float distance = Vector3.Distance(origin, player.transform.position);
                    float radius = 0.5f;
                    commands[index] = new SpherecastCommand(origin, radius, direction, hitMultipleFaces, distance);
                    index++;
                }
            }

            JobHandle handle = SpherecastCommand.ScheduleBatch(commands, results, 1, default);
            handle.Complete();

            for (int i = 0, resultIndex = 0; i < activeVisionCones.Count; i++)
            {
                EnemyVisionCone cone = activeVisionCones[i];

                for (int j = 0; j < cone.playersInVision.Count; j++, resultIndex++)
                {
                    RaycastHit hit = results[resultIndex];

                    if (cone.enemy != null)
                    {
                        Debug.Log(hit);
                        Debug.Log(cone);
                        Debug.Log(cone.enemy);
                        cone.enemy.ProcessRaycastHit(cone, hit);
                    }
                    else
                    {
                        Debug.LogError("Enemy reference in EnemyVisionCone is null.");
                        Debug.Break();
                    }
                }
            }

            results.Dispose();
            commands.Dispose();
        }
    }
}
