using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "base_LaserPointer", menuName = "Items/LaserPointer")]
public class LaserPointerData : ItemData
{
    public int chargeCount = 100;

    public void UseLaser()
    {
        Debug.Log("Laser successfully used trust");
    }
}
