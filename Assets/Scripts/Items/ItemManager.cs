using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField]
    ItemCategoryData[] itemCategoryDataArray;

    [SerializeField]
    ItemData[] baseItemDataArray;

    public Dictionary<int, ItemData> itemDictionary = new();
    public Dictionary<ItemSpawnType, ItemData[]> itemSpawnDictionary = new();
    public Dictionary<string, ItemData> baseItemDataDictionary = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ItemDictionaryInit()
    {
        itemDictionary.Clear();
        itemSpawnDictionary.Clear();
        baseItemDataDictionary.Clear();

        foreach (ItemCategoryData data in itemCategoryDataArray)
        {
            if (itemSpawnDictionary.ContainsKey(data.itemSpawnType))
            {
                Debug.Log($"Dictionary already contains the key: {data.itemSpawnType}");
                continue;
            }

            itemSpawnDictionary.Add(data.itemSpawnType, data.items);
            Debug.Log($"Added {data.items.Length} items to itemSpawnDictionary under key: {data.itemSpawnType}");
        }

        foreach (ItemData itemData in baseItemDataArray)
        {
            Debug.Log(itemData.itemName);
            if (baseItemDataDictionary.ContainsKey(itemData.itemName))
            {
                Debug.Log($"Dictionary already contains the key: {itemData.itemName}");
                continue;
            }
            
            baseItemDataDictionary.Add(itemData.itemName, itemData);
        }
    }

    public void PopulateItems(MapSegment segment)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("PopulateItems can only be called on the server.");
            return;
        }

        foreach (ItemNode node in segment.itemNodes)
        {
            Dictionary<string, float> itemRates = CalcBaseItemRates(node);

            CalcAltItemRates(node, itemRates);

            ItemData selectedItem = SelectItem(itemRates);

            if (selectedItem != null)
            {
                _ = ScriptableObject.CreateInstance<ItemData>();
                ItemSpawn(selectedItem, node.transform.position, node.transform.rotation);
                node.isUsed = true;
            }
            else
            {
                node.isNone = true;
            }
        }
    }

    Dictionary<string, float> CalcBaseItemRates(ItemNode node)
    {
        Dictionary<string, float> itemRates = new();

        foreach (ItemSpawnType spawnType in node.itemSpawnTypes)
        {
            if (itemSpawnDictionary.TryGetValue(spawnType, out ItemData[] itemDataArray))
            {
                foreach (ItemData itemData in itemDataArray)
                {
                    if (itemRates.ContainsKey(itemData.itemName))
                    {
                        continue;
                    }

                    itemRates.Add(itemData.itemName, itemData.spawnRate);
                }
            }
            else
            {
                Debug.LogWarning($"ItemSpawnType {spawnType} not found in itemSpawnDictionary.");
            }
        }

        return itemRates;
    }

    void CalcAltItemRates(ItemNode node, Dictionary<string, float> itemRates)
    {
        Debug.LogWarning("CalcAltItemRates is not implemented yet. This method should calculate alternative item rates based on additional factors like # of certain items already spawned, distance from certain segments, etc.");
    }

    ItemData SelectItem(Dictionary<string, float> itemRates)
    {
        Dictionary<string, int> itemIntPair = new();
        ItemData newItem = null;

        int totalInt = 0;
        int selectedInt;

        foreach (string itemName in itemRates.Keys)
        {
            int segValue = (int)(itemRates[itemName] * 100);
            itemIntPair.Add(itemName, segValue);

            totalInt += segValue;
        }

        if (totalInt > 0)
        {
            selectedInt = UnityEngine.Random.Range(0, totalInt);
            int compareInt = 0;

            foreach (string itemName in itemIntPair.Keys)
            {
                compareInt += itemIntPair[itemName];

                if (compareInt >= selectedInt)
                {
                    newItem = baseItemDataDictionary[itemName];
                    return newItem;
                }
            }
        }
        else
        {
            return newItem;
        }

        Debug.LogError($"item not found. {selectedInt}");
        Debug.Break();
        return newItem;
    }

    [Rpc(SendTo.Server)]
    public void PlayerDropItemRpc(SerializableItemData initItemData, Vector3 location, Quaternion orientation)
    {
        if (location != null
            && orientation != null)
        {
            ItemData itemData = baseItemDataDictionary[initItemData.itemName];

            // if there are any other details that change between pickup/drop, add them here
            itemData.useCount = initItemData.useCount;

            ItemSpawn(itemData, location, orientation);
        }
        else
        {
            Debug.LogError("ItemData, location, or orientation is null");
            Debug.Break();
        }
    }

    [Rpc(SendTo.Server)]
    public void PlayerPickupItemRpc(int itemID, ulong player)
    {
        if (itemID >= 0)
        {
            ItemData itemData = itemDictionary[itemID];

            if (itemData.item.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (networkObject.IsSpawned)
                {
                    PlayerPickupReturnRpc(itemID, player, RpcTarget.Single(player, RpcTargetUse.Temp));
                    Debug.Log($"Player picked up item: {itemData.itemName}, ID: {itemData.itemID}");
                    
                    networkObject.Despawn(true);
                }
                else
                {
                    Debug.LogError("NetworkObject is not spawned for the item being picked up");
                    Debug.Break();
                }
            }
            else
            {
                Debug.LogError("Item does not have a NetworkObject component");
                Debug.Break();
            }
        }
        else
        {
            Debug.LogError("Item is null in PlayerPickupItem RPC");
            Debug.Break();
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void PlayerPickupReturnRpc(int itemID, ulong player, RpcParams rpcParams = default)
    {
        Player client = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(player).GetComponent<Player>();

        if (itemID >= 0)
        {
            client.AddItem(itemID, null);
        }
        else
        {
            Debug.LogError($"Error with itemID: {itemID}");
            Debug.Break();
        }
    }

    void ItemSpawn(ItemData itemData, Vector3 location, Quaternion quaternion)
    {
        ItemData newItemData = itemData;

        if (newItemData != null)
        {
            if (itemDictionary.ContainsKey(newItemData.itemID))
            {
                Debug.Log($"Item with ID {newItemData.itemID} already exists in the dictionary. Overwriting.");
                itemDictionary[newItemData.itemID] = newItemData;
            }
            else
            {
                newItemData.itemID = itemDictionary.Count + 1;
                Debug.Log(newItemData.itemID);
                itemDictionary.Add(newItemData.itemID, newItemData);
            }

            GameObject newItem = Instantiate(newItemData.itemPrefab, location, quaternion);
            
            if (newItem.TryGetComponent<Item>(out var itemComponent))
            {
                itemComponent.ItemInit(newItemData);
                newItem.GetComponent<NetworkObject>().Spawn();
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
