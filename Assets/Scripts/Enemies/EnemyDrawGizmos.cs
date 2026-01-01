using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrawGizmos : MonoBehaviour
{
    [SerializeField]
    List<GameObject> eyePoints = new();

    private void OnDrawGizmosSelected()
    {
        foreach (GameObject eye in eyePoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(eye.transform.position, eye.transform.forward * 5f);
        }
    }
}
