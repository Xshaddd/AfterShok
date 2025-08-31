using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotHandler : MonoBehaviour
{
    public Item associatedItem;
    public TextMeshProUGUI descriptionBox;
    public Button useButton;
    public PlayerController player;

    public void ShowAssociatedItemDescription()
    {
        string description = associatedItem?.ReturnDescription();
        descriptionBox.text = description;
        useButton.onClick.RemoveAllListeners();
        if (associatedItem.onUse == null)
        {
            useButton.enabled = false;
        }
        else
        {
            useButton.onClick.AddListener(() => associatedItem.Use(player));
            useButton.gameObject.SetActive(true);
        }
    }
}
