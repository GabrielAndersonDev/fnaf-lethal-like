using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    private static WaitForSeconds AttackDelay;

    [Header("Enemy Attack Settings")]
    public float attackDamage;
    public float attackRange;
    public float attackDelay;

    private IEnumerator EnemyAttack(Player player)
    {
        // play attack animation + logic
        yield return AttackDelay;

        if (player != null
            && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            player.TakeDamage(attackDamage);
        }
    }
}
