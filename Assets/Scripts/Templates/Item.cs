using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;

    public Sprite icon;
    public float weight;
    public UnityEvent onUse;

    public virtual void Use(PlayerController player)
    {
        onUse?.Invoke();
    }

    public string ReturnDescription()
    {
        return $"{name}\nWeight: {weight}\nID: {id}";
    }
}
