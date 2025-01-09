using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    /* private void Start()
    {
        if (itemData != null)
        {
            Initialize(itemData);
            Debug.Log($"Item Loaded: {itemData.itemName}");
        }
        else
        {
            Debug.LogWarning("No itemData assigned");
        }
    } */

    public void Initialize(ItemData data)
    {
        itemData = data;

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
}
