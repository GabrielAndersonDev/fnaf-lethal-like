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
            int itemID = item.itemID;
            if (itemID < 0)
            {
                Debug.LogError("Item ID is invalid");
                return;
            }

            ItemManager.Instance.PlayerPickupItemRpc(itemID, NetworkManager.Singleton.LocalClientId);
            
        }
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[inventorySlot]}");
            Debug.Break();
        }
    }

    public void AddItem(int itemID, int? slot)
    {
        int chosenSlot = inventorySlot;

        if (slot.HasValue
            && slot >= 0
            && slot < inventory.Length
            )
        {
            inventorySlot = slot.Value;
        }

        if (inventory[chosenSlot] != null)
        {
            Debug.Log("Inventory slot is not empty");
        }
        else if (inventory[chosenSlot] == null) 
        {
            inventory[chosenSlot] = ItemManager.Instance.itemDictionary[itemID];
        } 
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[chosenSlot]}");
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

            SerializableItemData itemData = new()
            {
                itemName = inventory[inventorySlot].itemName,
                itemID = inventory[inventorySlot].itemID,
                useCount = inventory[inventorySlot].useCount,
                heldPlayer = inventory[inventorySlot].heldPlayer,
                heldSlot = inventory[inventorySlot].heldSlot
            };
                
            ItemManager.Instance.PlayerDropItemRpc(itemData, rb.transform.position, Quaternion.identity);

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
