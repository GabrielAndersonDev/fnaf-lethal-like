using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemData itemData;

    private void Start()
    {
        // This is temporary for spawning in the test Laser Pointer.
        if (itemData != null)
        {
            GameObject newItem = Instantiate(itemData.itemPrefab, Vector3.zero, Quaternion.identity);
            
            if (newItem.TryGetComponent<Item>(out var itemComponent))
            {
                itemComponent.ItemInit(itemData);
            }
            else
            {
                Debug.LogError("Missing an item");
            }
        } else
        {
            Debug.LogError("ItemData missing");
        }

    }

    public void ItemGen(ItemData itemData, Vector3 location, Quaternion quaternion)
    {
        if (itemData != null)
        {
            GameObject newItem = Instantiate(itemData.itemPrefab, location, quaternion);

            if (newItem.TryGetComponent<Item>(out var itemComponent))
            {
                itemComponent.ItemInit(itemData);
            }
            else
            {
                Debug.LogError("Missing an item");
                Debug.Break();
            }
        }
        else
        {
            Debug.LogError("ItemData missing");
            Debug.Break();
        }
    }
}
