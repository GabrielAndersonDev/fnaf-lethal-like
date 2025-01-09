using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
public enum UseCount
    {
        SingleUse,
        Infinite,
        Recharge,
        Reload,
        None
    }

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject itemPrefab;
    public string description;
    public UseCount useCount;
    public bool held;
    public string heldName;
    public int heldSlot;

    public virtual void ItemAttack()
    {

    }

    public virtual void UseItem()
    {

    }

    public virtual void UseAlt()
    {

    }

    public virtual void UseLight()
    {

    }
}