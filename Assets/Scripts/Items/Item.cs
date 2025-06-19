using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
    Finite,
    Recharge,
    Reload,
    Max
}

public class Item : NetworkBehaviour
{
    public string itemName;
    public int itemID;
    public ItemData itemData;
    public Sprite icon;
    public string description;
    public UseCount useCount;
    public bool held;
    public ulong heldPlayer;
    public int heldSlot;

    public void ItemInit(ItemData data)
    {
        itemData = data;

        if (itemData != null)
        {

            itemName = data.itemName;
            itemID = data.itemID;
            data.item = gameObject.GetComponent<Item>();
            icon = data.icon;
            description = data.description;
            useCount = data.useCount;
            held = data.held;
            heldPlayer = data.heldPlayer;
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
