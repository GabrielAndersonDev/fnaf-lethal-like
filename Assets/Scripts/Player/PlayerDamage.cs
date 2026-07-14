using Unity.Netcode;
using UnityEngine;

public partial class Player : NetworkBehaviour
{
    public void TakeDamage(float damageAmount)
    {
        if (!IsServer)
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        currentHealth = 0;
        // Add additional death logic here (e.g., play animation, notify other players, and enter spectator mode)
    }
}
