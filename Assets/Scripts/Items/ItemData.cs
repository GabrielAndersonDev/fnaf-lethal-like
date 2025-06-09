using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public ItemSpawnType itemSpawnType;
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