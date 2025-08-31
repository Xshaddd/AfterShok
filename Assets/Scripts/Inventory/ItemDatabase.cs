using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("All Items")]
    public List<Item> Items;

    private Dictionary<int, Item> itemLookup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        itemLookup = new Dictionary<int, Item>();
        foreach (var item in Items)
        {
            itemLookup[item.id] = item;
        }
    }

    public Item GetItemByID(int id)
    {
        if (itemLookup.TryGetValue(id, out var item))
            return item;
        return null;
    }
}
