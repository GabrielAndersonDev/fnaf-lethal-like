using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public ItemData[] inventory;
    int inventorySlot;

    public ItemData[] InventoryInit()
    {
        inventory = new ItemData[4];

        if (inventory == null)
        {
            Debug.LogError("Inventory is null");
            Debug.Break();
        } else
        {
            return inventory;
        }
        return null;
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
            Debug.Log($"Inventory slot {inventorySlot} changed to {inventory[inventorySlot]}");
            Destroy(item.gameObject);
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
        else
        {
            Debug.Log($"Item {inventory[inventorySlot].itemName} dropped.");

            GameObject newItem = Instantiate(inventory[inventorySlot].itemPrefab, rb.transform.position, Quaternion.identity);
            
            if (newItem.TryGetComponent<Item>(out var itemComponent))
            {
                itemComponent.Initialize(inventory[inventorySlot]);
                inventory[inventorySlot] = null;
                Debug.Log("Success!");
                return;
            } else
            {
                Debug.LogError("Missing an item");
                Debug.Break();
            }
        }
    }
}
