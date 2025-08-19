using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class HallwayFunction : MonoBehaviour
{
    public GameObject[] onTrueAppear;
    public GameObject onFalseAppear;

    public NavMeshModifier modifier;

    public void CheckHallwayState()
    {
        if (this.GetComponent<MapNode>().isConnected)
        {
            foreach(GameObject gameObject in onTrueAppear)
            {
                gameObject.SetActive(true);
            }

            Destroy(modifier);
            onFalseAppear.SetActive(false);
        }
        else
        {
            foreach (GameObject gameObject in onTrueAppear)
            {
                gameObject.SetActive(false);
            }

            modifier.AffectsAgentType(0);
            onFalseAppear.SetActive(true);
        }
    }
}
