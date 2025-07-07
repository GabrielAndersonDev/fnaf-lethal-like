using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum ItemTypeSerializedKind
{
    Invalid = -2,
    None = -1,
    First,
    LaserPointer = First,
    Max
}

public struct SerializableItemData : INetworkSerializable
{
    public ItemTypeSerializedKind kind;

    public string itemName;
    public int itemID;

    public int chargeCount;
    public bool isActive;

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref kind);
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref itemID);

        switch (kind)
        {
            case ItemTypeSerializedKind.LaserPointer:
                serializer.SerializeValue(ref chargeCount);
                serializer.SerializeValue(ref isActive);
                break;
            default:
                Debug.LogWarning($"Unknown ItemTypeSerializedKind: {kind}");
                Debug.Break();
                break;
        } 
    }
}

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public int itemID;
    public ItemTypeSerializedKind itemTypeSerializedKind;
    public float spawnRate;
    public Item item;
    public ItemSpawnType itemSpawnType;
    public Sprite icon;
    public GameObject itemPrefab;
    public string description;
    public UseCount useCount;
    public bool held;
    public ulong heldPlayer;
    public int heldSlot;

    public virtual void ItemAttack()
    {

    }

    public virtual void UseItem()
    {

    }

    public virtual void UseAlt()
    {

    }

    public virtual void UseLight()
    {

    }

    public SerializableItemData GetSerializableItemData()
    {
        SerializableItemData serializableData = new()
        {
            kind = itemTypeSerializedKind,
            itemName = itemName,
            itemID = itemID,
        };
        switch (itemTypeSerializedKind)
        {
            case ItemTypeSerializedKind.LaserPointer:
                if (this is LaserPointerData laserPointerData)
                {
                    serializableData.chargeCount = laserPointerData.chargeCount;
                    serializableData.isActive = laserPointerData.isActive;
                }
                break;
            default:
                Debug.LogWarning($"Unknown ItemTypeSerializedKind: {itemTypeSerializedKind}");
                Debug.Break();
                break;
        }
        return serializableData;
    }

    public ItemData GetItemDataFromSerialized(ItemData itemData, SerializableItemData serializedData)
    {
        itemData.itemTypeSerializedKind = serializedData.kind;

        itemData.itemID = serializedData.itemID;
        itemData.itemName = serializedData.itemName;

        switch (serializedData.kind)
        {
            case ItemTypeSerializedKind.LaserPointer:
                if (itemData is LaserPointerData laserPointerData)
                {
                    laserPointerData.chargeCount = serializedData.chargeCount;
                    laserPointerData.isActive = serializedData.isActive;

                    return laserPointerData;
                }
                else
                {
                    Debug.LogError("ItemData is not of type LaserPointerData");
                    Debug.Break();
                }
                break;
            default:
                Debug.LogWarning($"Unknown ItemTypeSerializedKind: {serializedData.kind}");
                Debug.Break();
                break;
        }

        return itemData;
    }
}