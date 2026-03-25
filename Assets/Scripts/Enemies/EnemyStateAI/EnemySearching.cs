using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    private Coroutine SearchLastKnownCoroutine;
    private IEnumerator EnemyStateSearching()
    {
        while (true)
        {
            Debug.LogWarning("Searching for last known location of player... Currently unimplemented.");

            SearchLastKnownCoroutine ??= StartCoroutine(SearchLastKnownLocation());

            yield return _waitForSeconds0_2;
        }
    }

    // logic for searching last known location of player
    private IEnumerator SearchLastKnownLocation()
    {
        // add logic for going to location and searching based on direction player was running
        while (true)
        {
            Debug.LogWarning("SearchLastKnownLocation not implemented.");
            yield return _waitForSeconds0_2;
        }
    }
}
