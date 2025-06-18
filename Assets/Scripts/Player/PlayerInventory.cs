using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public partial class Player : NetworkBehaviour
{
    public ItemData[] inventory;
    int inventorySlot;

    // TEMPORARY!! Until I apply more of the Game functionality. Unless this is how you're supposed to do it lol

    public ItemData[] InventoryInit()
    {
        inventory = new ItemData[4];

        if (inventory == null)
        {
            Debug.LogError("Inventory is null");
            Debug.Break();
        } 
        else if (inventory[0]  == null)
        {
            return inventory;
        }
        Debug.LogError($"Inventory is not null or correct {inventory}.");
        Debug.Break();
        return null;
    }

    public void AddItemSlotCheck(Item item)
    {
        if (inventory[inventorySlot] != null)
        {
            Debug.Log("Inventory slot is not empty");
        }
        else if (inventory[inventorySlot] == null)
        {
            ItemManager.Instance.PlayerPickupItemRpc(item, NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[inventorySlot]}");
            Debug.Break();
        }
    }

    public void AddItem(Item item)
    {
        if (inventory[inventorySlot] != null)
        {
            Debug.Log("Inventory slot is not empty");
        }
        else if (inventory[inventorySlot] == null) 
        {
            inventory[inventorySlot] = item.itemData;
        } 
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[inventorySlot]}");
            Debug.Break();
        }
    }

    public void RemoveItem()
    {
        if (inventory[inventorySlot] == null)
        {
            Debug.Log("Inventory slot is empty already");
        }
        else if (inventory[inventorySlot] is ItemData)
        {
            ItemManager.Instance.ItemGen(inventory[inventorySlot], rb.transform.position, Quaternion.identity);

            Debug.Log($"Item {inventory[inventorySlot].itemName} dropped.");

            inventory[inventorySlot] = null;
        } 
        else
        {
            Debug.LogError("RemoveItem invalid.");
            Debug.Break();
        }
    }
}
