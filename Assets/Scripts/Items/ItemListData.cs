using UnityEngine;

[CreateAssetMenu(fileName = "ItemListData", menuName = "Items/ItemListData")]
public class ItemListData : ScriptableObject
{
    public ItemData[] items;
}
