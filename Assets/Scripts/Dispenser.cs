using Mirror.BouncyCastle.Asn1.Cmp;
using UnityEngine;

public class Dispenser : MonoBehaviour, IInteractable
{
    [SerializeField] Item dispensedItem;

    public void Interact(PlayerController player)
    {
        if (player.GiveItem(dispensedItem))
            Debug.Log($"Successfully given {dispensedItem.name} to player {player.connectionToClient}");
        else
            Debug.Log($"Failed to give {dispensedItem.name} to player {player.connectionToClient} (inventory full)");
    }
}
