using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
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
            if (pathGoal == null)
            {
                pathGoal = transform;
            }

            if (agent.enabled
                && pathGoal != null)
            {
                agent.SetDestination(pathGoal.position);
            }
            else
            {
                Debug.LogWarning("agent is disabled or pathGoal is null in EnemyMove");
            }
        }

        yield return _waitForSeconds0_2;
    }
}
