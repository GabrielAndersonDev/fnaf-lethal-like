using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum UseCount
{
    Invalid = -2,
    None = -1,
    First,
    SingleUse = First,
    Infinite,
    Recharge,
    Reload,
    Max
}

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public string itemName;
    public Sprite icon;
    public GameObject itemPrefab;
    public string description;
    public UseCount useCount;
    public bool held;
    public string heldName;
    public int heldSlot;

    public void Initialize(ItemData data)
    {
        itemData = data;

        if (itemData != null)
        {

            itemName = data.itemName;
            icon = data.icon;
            itemPrefab = data.itemPrefab;
            description = data.description;
            useCount = data.useCount;
            held = data.held;
            heldName = data.heldName;
            heldSlot = data.heldSlot;

            Debug.Log($"Item initialized: {itemData.itemName}");
        } 
        else
        {
            Debug.LogError($"itemData is {itemData}");
            Debug.Break();
        }
    }
}
