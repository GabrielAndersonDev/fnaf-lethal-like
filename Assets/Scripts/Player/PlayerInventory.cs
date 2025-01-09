using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    //public List<Item> held_items = new();
    public ItemData[] inventory = new ItemData[4];
    int inventorySlot;

    // these may not need to remain bools, it just depends if i do something with them after that requires it
    public void AddItem(Item item)
    {
        if (inventory[inventorySlot] != null)
        {
            Debug.Log("Inventory slot is not empty");
        }
        else
        {
            inventory[inventorySlot] = item.itemData;
            Debug.Log($"Inventory slot {inventorySlot} changed to {inventory[inventorySlot]}");
            Destroy(item.gameObject);
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
            }
            Debug.LogError("ItemData missing");
        }
    }
}
