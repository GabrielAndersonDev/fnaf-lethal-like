using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "base_LaserPointer", menuName = "Items/LaserPointer")]
public class LaserPointerData : ItemData
{
    public int chargeCount = 100;
    public bool laserOn = false;

    public override void UseLight()
    {
        UseLaser();
    }

    public override void ItemAttack()
    {
        UseLaser();
    }

    public void UseLaser()
    {
        Debug.Log("Laser successfully used trust");
        laserOn = !laserOn;

        chargeCount -= 5;
        Debug.Log(chargeCount);
    }
}
