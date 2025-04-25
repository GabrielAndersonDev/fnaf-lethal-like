using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayFunction : MonoBehaviour
{
    public GameObject[] onTrueAppear;
    public GameObject onFalseAppear;

    void Update()
    {
        if (this.GetComponent<MapNode>().isConnected)
        {
            foreach(GameObject gameObject in onTrueAppear)
            {
                gameObject.SetActive(true);
            }

            onFalseAppear.SetActive(false);
        }
        else
        {
            foreach (GameObject gameObject in onTrueAppear)
            {
                gameObject.SetActive(false);
            }

            onFalseAppear.SetActive(true);
        }
    }
}
