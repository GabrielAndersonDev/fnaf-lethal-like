using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

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
            int itemID = item.itemID.Value;
            if (itemID < 0)
            {
                Debug.LogError("Item ID is invalid");
                return;
            }

            ItemManager.Singleton.PlayerPickupItemServerRpc(itemID, NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[inventorySlot]}");
            Debug.Break();
        }
    }

    public void AddItem(SerializableItemData itemData, int? slot)
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
            ItemData newItem = Instantiate(ItemManager.Singleton.baseItemDataDictionary[itemData.itemName]);
            newItem = newItem.GetItemDataFromSerialized(newItem, itemData);

            newItem.heldSlot = chosenSlot;
            newItem.isHeld = true;
            newItem.heldPlayerSteamID = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().steamID;
            Debug.Log(newItem.heldPlayerSteamID);
            Debug.Log(NetworkManager.Singleton.LocalClientId);
            newItem.item = null;

            inventory[chosenSlot] = newItem;

            ItemManager.Singleton.SetItemDataServerRpc(newItem.GetSerializableItemData());
        } 
        else
        {
            Debug.LogError($"Inventory slot error: {inventory[chosenSlot]}");
            Debug.Break();
        }

        UIManager.Singleton.InventoryUIUpdate(inventorySlot);
    }

    public void DropItem()
    {
        if (inventory[inventorySlot] == null)
        {
            Debug.Log("Inventory slot is empty already");
        }
        else if (inventory[inventorySlot] is ItemData)
        {
            SerializableItemData itemData = inventory[inventorySlot].GetSerializableItemData();

            Debug.Log(inventory[inventorySlot]);
            Debug.Log(itemData.itemName);

            Vector3 dropPosition = playerCam.transform.position + playerCam.transform.forward * 2;

            ItemManager.Singleton.PlayerDropItemServerRpc(itemData, dropPosition, Quaternion.identity);

            Debug.Log($"Item {inventory[inventorySlot].itemName} dropped.");

            inventory[inventorySlot] = null;
        } 
        else
        {
            Debug.LogError("RemoveItem invalid.");
            Debug.Break();
        }

        UIManager.Singleton.InventoryUIUpdate(inventorySlot);
    }

    public void RemoveItem(int slot)
    {
        inventory[slot] = null;
    }

    public void DeleteInventory()
    {
        for (int i = 0; inventory.Length > 0; i++)
        {
            inventory[i] = null;
        }
    }
}
