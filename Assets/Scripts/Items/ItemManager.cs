using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

public struct OwnedItemObj
{
    public ItemData item;
    public Vector3 position;
    public Quaternion rotation;
}

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Singleton { get; private set; }

    public List<OwnedItemObj> ownedItems = new();

    [SerializeField]
    ItemCategoryData[] itemCategoryDataArray;

    [SerializeField]
    ItemData[] baseItemDataArray;

    public Dictionary<int, ItemData> spawnedItemDictionary = new();
    public Dictionary<ItemSpawnType, ItemData[]> itemSpawnDictionary = new();
    public Dictionary<string, ItemData> baseItemDataDictionary = new();

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }

        DontDestroyOnLoad(gameObject);
        gameObject.GetComponent<NetworkObject>().Spawn();
    }

    public void ItemDictionaryInit()
    {
        spawnedItemDictionary.Clear();
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
        if (!NetworkManager.Singleton.IsServer
            || !NetworkManager.Singleton.IsHost)
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
                ItemData newItemData = Instantiate(selectedItem);

                ItemSpawn(newItemData, node.transform.position, node.transform.rotation);
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
            Debug.LogError("totalInt is not a valid value, cannot select item.");
            Debug.Break();
        }

        return newItem;
    }

    [ServerRpc]
    public void PlayerDropItemServerRpc(SerializableItemData initItemData, Vector3 location, Quaternion orientation)
    {
        if (location != null
            && orientation != null)
        {
            if (baseItemDataDictionary.ContainsKey(initItemData.itemName))
            {
                ItemData itemData = Instantiate(baseItemDataDictionary[initItemData.itemName]);

                if (spawnedItemDictionary.TryGetValue(initItemData.itemID, out var existingItem))
                {
                    Debug.Log("Item already exists in spawned item dic");
                    itemData = existingItem;
                }

                // if there are any other details that change between pickup/drop, add them here
                itemData.GetItemDataFromSerialized(itemData, initItemData);

                itemData.held = false;
                itemData.heldPlayer = null;
                itemData.heldSlot = null;

                ItemSpawn(itemData, location, orientation);
            }
            else
            {
                Debug.Log(initItemData.itemName);
                Debug.Break();
            }
        }
        else
        {
            Debug.LogError("ItemData, location, or orientation is null");
            Debug.Break();
        }
    }

    [ServerRpc]
    public void PlayerPickupItemServerRpc(int itemID, ulong player)
    {
        if (itemID >= 0)
        {
            ItemData itemData = spawnedItemDictionary[itemID];
            SerializableItemData serializedData = itemData.GetSerializableItemData();

            if (itemData.item.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (networkObject.IsSpawned)
                {
                    itemData.item = null;
                    itemData.held = true;
                    itemData.heldPlayer = player;

                    PlayerPickupReturnRpc(serializedData, player, RpcTarget.Single(player, RpcTargetUse.Temp));
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
    void PlayerPickupReturnRpc(SerializableItemData serializedData, ulong player, RpcParams rpcParams = default)
    {
        Player client = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(player).GetComponent<Player>();

        if (serializedData.itemID >= 0)
        {
            client.AddItem(serializedData, null);
        }
        else
        {
            Debug.LogError($"Error with itemID: {serializedData.itemID}");
            Debug.Break();
        }
    }

    // ItemSpawn instantiates using the exact ItemData provided, NOT a copy! If you need to use a copy, ensure to clone the ItemData before passing it in.
    void ItemSpawn(ItemData itemData, Vector3 location, Quaternion quaternion)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("ItemSpawn can only be called on the server.");
            return;
        }

        int itemIDValue;

        if (itemData != null)
        {
            if (spawnedItemDictionary.ContainsKey(itemData.itemID))
            {
                Debug.Log($"Item with ID {itemData.itemID} already exists in the dictionary. Overwriting.");
                spawnedItemDictionary[itemData.itemID] = itemData;
                itemIDValue = itemData.itemID;
            }
            else
            {
                itemIDValue = spawnedItemDictionary.Count + 1;

                spawnedItemDictionary.Add(itemIDValue, itemData);
            }

            GameObject newItem = Instantiate(itemData.itemPrefab, location, quaternion);
            
            if (newItem.TryGetComponent(out Item itemComponent))
            {
                newItem.GetComponent<NetworkObject>().Spawn();
                itemComponent.itemID.Value = itemIDValue;
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

    public void ItemDelete(int itemID)
    {
        if (spawnedItemDictionary.TryGetValue(itemID, out ItemData item))
        {
            if (item.held)
            {
                ulong player = (ulong)item.heldPlayer;
                int heldSlot = (int)item.heldSlot;

                DeleteSingleInventoryItemRpc(player, heldSlot, RpcTarget.Single(player, RpcTargetUse.Temp));
            }

            if (item.item != null)
            {
                item.item.GetComponent<NetworkObject>().Despawn(true);
            }

            spawnedItemDictionary.Remove(itemID);

            Destroy(item);
        }
        else
        {
            Debug.LogWarning("ItemID does not correspond with an item in the dictionary.");
            Debug.Break();
        }
    }

    public void PopulateOwnedItems()
    {
        ownedItems.Clear();

        Debug.LogWarning("PopulateOwnedItems currently not implemented.");
    }

    public void AllDropItems()
    {
        if (!NetworkManager.Singleton.IsHost
            && !NetworkManager.Singleton.IsServer) 
        {
            return;
        }

        foreach (ulong client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            ClientDropAllItemsRpc(client, RpcTarget.Single(client, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ClientDropAllItemsRpc(ulong player, RpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out Player component))
        {
            foreach (ItemData data in component.inventory)
            {
                PlayerDropItemServerRpc(data.GetSerializableItemData(), component.transform.position, component.transform.rotation);
            }
        }
        else
        {
            Debug.Assert(false);
        }
    }

    public void DeleteAllItems(bool isOverride, List<int> safeItemIds)
    {
        List<int> keys = spawnedItemDictionary.Keys.ToList();

        foreach (int key in keys)
        {
            if (spawnedItemDictionary.ContainsKey(key)
                && isOverride)
            {
                ItemDelete(key);
            }

            if (spawnedItemDictionary.ContainsKey(key)
                && !isOverride)
            {
                if (!safeItemIds.Contains(key))
                {
                    ItemDelete(key);
                }
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void DeleteSingleInventoryItemRpc(ulong player, int slot, RpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out Player obj))
        {
            obj.RemoveItem(slot);
        }
        else
        {
            Debug.LogError("Player " + player + " does not have a PlayerObject or Player component.");
            Debug.Break();
        }
    }

    public void OwnedItemsCheck()
    {

    }
}
