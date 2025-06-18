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
    public ItemData itemData;

    [SerializeField]
    ItemCategoryData[] itemCategoryDataArray;

    public Dictionary<ItemSpawnType, ItemData[]> itemSpawnDictionary = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ItemDictionaryInit();
    }

    void ItemDictionaryInit()
    {
        itemSpawnDictionary.Clear();

        foreach (ItemCategoryData data in itemCategoryDataArray)
        {
            if (itemSpawnDictionary.ContainsKey(data.itemSpawnType))
            {
                Debug.Log($"Dictionary already contains the key: {data.itemSpawnType}");
                continue;
            }

            itemSpawnDictionary.Add(data.itemSpawnType, data.items);
        }
    }

    [Rpc(SendTo.Server)]
    public void PlayerDropItemRpc(ItemData itemData, Vector3 location, Quaternion orientation)
    {
        if (itemData != null
            && location != null
            && orientation != null)
        {
            GameObject newItem = Instantiate(itemData.itemPrefab, location, orientation);
            newItem.GetComponent<Item>().ItemInit(itemData);

            if (newItem.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("Missing NetworkObject component on item prefab");
                Debug.Break();
            }
        }
        else
        {
            Debug.LogError("ItemData, location, or orientation is null");
            Debug.Break();
        }
    }

    [Rpc(SendTo.Server)]
    public void PlayerPickupItemRpc(Item item, ulong player)
    {
        if (item != null)
        {
            if (item.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (networkObject.IsSpawned)
                {
                    PlayerPickupReturnRpc(item, player, RpcTarget.Single(player, RpcTargetUse.Temp));
                    Debug.Log($"Player picked up item: {item.itemName}");
                    
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
    void PlayerPickupReturnRpc(Item item, ulong player, RpcParams rpcParams = default)
    {
        Player client = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(player).GetComponent<Player>();

        if (item != null)
        {
            client.AddItem(item);
        }
        else
        {
            Debug.LogError("Item is null in PlayerPickupReturn RPC");
            Debug.Break();
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
