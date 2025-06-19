using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public struct SerializableItemData : INetworkSerializable
{
    public string itemName;
    public int itemID;
    public UseCount useCount;
    public ulong heldPlayer;
    public int heldSlot;

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref itemID);
        serializer.SerializeValue(ref useCount);
        serializer.SerializeValue(ref heldPlayer);
        serializer.SerializeValue(ref heldSlot);
    }
}

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public int itemID;
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
}