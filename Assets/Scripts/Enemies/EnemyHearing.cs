using NUnit.Framework;
using System.Collections;
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
            CheckHearingRange();

            yield return _waitForSeconds0_2;
        }
    }

    private void CheckHearingRange()
    {
        noisesHeard.Clear();

        int layerMask = LayerMask.GetMask("Noise");
        Physics.OverlapSphereNonAlloc(transform.position, hearingRange, noiseColliders, layerMask);

        for (int i = 0; i < noiseColliders.Length; i++)
        {
            if (noiseColliders[i] == null)
            {
                return;
            }

            GameObject obj = noiseColliders[i].gameObject;

            if (!noisesHeard.Contains(obj))
            {
                noisesHeard.Add(obj);
            }
        }
    }
}
