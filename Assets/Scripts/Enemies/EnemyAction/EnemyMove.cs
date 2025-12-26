using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    private Coroutine EnemyMoveCoroutine;
    // Enemy movement towards pathGoal, this includes animation control later on

    // Add changes to motion when idle, turning, etc

    private IEnumerator EnemyMove()
    {
        while (gameObject.activeSelf)
        {
            if (agent.enabled)
            {
                agent.SetDestination(pathGoal.position);
            }
        }

        yield return _waitForSeconds0_2;
    }
}
