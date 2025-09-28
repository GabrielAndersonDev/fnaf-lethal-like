using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSegment : MapSegment
{
    public PlayerSpawnNode[] playerSpawnNodes;

    public List<Player> playersInVan;
    public List<Item> itemsInVan;

    // Whether the entrance is open for players to enter or exit
    public bool isOpen;

    private void Awake()
    {
        playersInVan = new();
        itemsInVan = new();
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        Item item = other.gameObject.GetComponentInParent<Item>();

        if (player != null
            && !player.isDead)
        {
            playersInVan.Add(player);
        }

        if (item != null)
        {
            itemsInVan.Add(item);
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        Debug.Log(other.gameObject.ToString());

        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (player != null)
            {
                playersInVan.Remove(player);
            }
        }

        if (other.gameObject.TryGetComponent(out Item item))
        {
            if (item != null)
            {
                itemsInVan.Remove((item));
            }
        }
    }

    public void PlayerItemCollisionCheck()
    {
        List<ulong> safePlayers = new();
        List<int> safeItemIds = new();

        foreach (Player player in playersInVan)
        {
            ulong clientID = NetworkScript.Singleton.steamIdToClientId[player.steamID];

            safePlayers.Add(clientID);

            foreach (ItemData item in player.inventory)
            {
                if (item == null) continue;

                safeItemIds.Add(item.itemID);
            }
        }

        foreach (Item item in itemsInVan)
        {
            safeItemIds.Add(item.itemID.Value);
        }

        ItemManager.Singleton.DeleteAllItems(false, safeItemIds);
        PlayerManager.Singleton.SetAllPlayersDead(false, safePlayers);

        ItemManager.Singleton.PopulateOwnedItems(safeItemIds);
    }
}
