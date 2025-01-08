using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemData itemData;

    private void Start()
    {
        if (itemData != null)
        {
            GameObject newItem = Instantiate(itemData.itemPrefab, Vector3.zero, Quaternion.identity);
            
            if (newItem.TryGetComponent<Item>(out var itemComponent))
            {
                itemComponent.Initialize(itemData);

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

    public void ItemGen(GameObject itemPrefab)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
