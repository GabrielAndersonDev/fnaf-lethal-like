using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemSpawnType
{
    Invalid = -2,
    None = -1,
    First,
    Small = First,
    Medium,
    Large,
    Max
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public ItemData itemData;

    public Dictionary<ItemSpawnType, ItemListData> itemSpawnList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ItemManagerInit()
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
