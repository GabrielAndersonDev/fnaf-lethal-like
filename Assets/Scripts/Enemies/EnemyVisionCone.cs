using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    public Enemy enemy;
    public GameObject eyePoint;

    public List<Player> playersInVision;

    private void Start()
    {
        playersInVision = new List<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemy.isDeactivated)
        {
            Debug.Log("enemy is deactivated, returning");
            return;
        }

        Player player = other.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            Debug.Log(player);
            if (enemy != null
                && !playersInVision.Contains(player))
            {
                playersInVision.Add(player);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemy.isDeactivated)
        {
            Debug.Log("enemy is deactivated, returning");
            return;
        }

        Player player = other.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            if (enemy != null
                && playersInVision.Contains(player))
            {
                playersInVision.Remove(player);
            }
        }
    }
}
