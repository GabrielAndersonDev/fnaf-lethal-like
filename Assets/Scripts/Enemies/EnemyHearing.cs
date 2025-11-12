using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    [Header("Hearing")]
    public bool canHear;
    public float hearingRange;
    // Is a game object for now to determine the source of the noise and where the enemy will path to if prioritized.
    public GameObject targetNoiseSource;
    public List<GameObject> noisesHeard = new();
    public Collider[] noiseColliders;

    private IEnumerator CheckHearingRoutine()
    {
        while (canHear)
        {
            if ()
            {

            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void CheckHearingRange()
    {
        int layerMask = LayerMask.GetMask("Noise");
        Physics.OverlapSphereNonAlloc(transform.position, hearingRange, noiseColliders, layerMask);

    }
}
