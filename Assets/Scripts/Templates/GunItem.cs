using UnityEngine;

[CreateAssetMenu(fileName = "GunItem", menuName = "Scriptable Objects/GunItem")]
public class GunItem : Item
{
    public float damage = 40;
    public GameObject prefab;
    public bool equippable = true;
    public GameObject bullet;

    public override void Use(PlayerController player)
    {
        Equip(player);
    }

    public void Equip(PlayerController player)
    {
        player.Equip(this);
    }

    public void ShootRaycast()
    {

    }
}
