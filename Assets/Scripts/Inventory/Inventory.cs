using System;
using UnityEngine;

public class Inventory
{
    public Item[] slots;

    public Inventory(int inventorySize, Item[] items = null)
    {
        slots = new Item[inventorySize];

        if (items != null)
        {
            if (items.Length > slots.Length)
            {
                Debug.LogError($"[Inventory] passed more items to Inventory constructor than size ({items.Length} > {inventorySize})");
                throw new ArgumentException();
            }

            for (int i = 0; i < items.Length; i++)
            {
                slots[i] = items[i];
            }
        }
    }
    /// <summary>
    /// Debug method; only logs item descriptions or empty slots.
    /// </summary>
    public void DisplayInventory()
    {
        Debug.Log(slots);
        foreach (Item item in slots)
        {
            if (item != null)
                Debug.Log(item.ReturnDescription());
            else
                Debug.Log("Slot is empty");
        }
    }
}
