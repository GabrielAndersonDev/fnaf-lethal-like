using UnityEngine;

[CreateAssetMenu(fileName = "ItemCategoryData", menuName = "Items/ItemCategoryData")]
public class ItemCategoryData : ScriptableObject
{
    public ItemSpawnType itemSpawnType;
    public ItemData[] items;
}
