using Mirror;
using StinkySteak.MirrorBenchmark;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    InputSystem_Actions actions;
    

    [Header("Camera")]
    [SerializeField] Camera playerCamera;
    [SerializeField] AudioListener playerAudioListener;
    [SerializeField] float sensitivity = 10f;
    [SerializeField] float minY = -80f;
    [SerializeField] float maxY = 80f;

    Vector2 lookVector;

    bool isInventoryOpen = false;

    [Header("Movement")]
    [SerializeField] CharacterController characterController;
    [SerializeField] float maxMovementSpeed = 1f;
    [SerializeField] float maxStrafeSpeed = 0.6f;
    [SerializeField] float jumpForce = 2f;
    [SerializeField] float gravity = 2f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float deceleration = 3f;
    [SerializeField] float baseModifier = 1f;
    [SerializeField] float sprintModifier = 2f;

    Vector3 currentVelocity;
    Vector3 targetVelocity;
    float verticalVelocity;

    [Header("Inventory")]
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Transform inventoryContainer;
    [SerializeField] Item[] startingItems;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] Button useButton;
    [SerializeField] EquipMount equipMount;
    Inventory inventory;
    Item equippedSlot;
    GameObject equippedItem;

    [Header("Interaction")]
    [SerializeField] float maxInteractRange;

    public override void OnStartLocalPlayer()
    {
        Application.targetFrameRate = 120;
        actions = new InputSystem_Actions();
        actions.Enable();

        actions.Player.Inventory.performed += ctx => HandleInventory();
        actions.Player.Respawn.performed += ctx => NetworkingManager.Instance.RespawnPlayer(connectionToClient);
        actions.Player.Interact.performed += ctx => Interact();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inventory = new Inventory(10, startingItems);
        inventoryCanvas.enabled = false;

        playerCamera.enabled = true;
        playerAudioListener.enabled = true;
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            playerCamera.enabled = false;
            playerAudioListener.enabled = false;
            inventoryCanvas.enabled = false;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        if (!InputManager.Instance.MenuOpen && !isInventoryOpen)
        {
            HandleLook();
        }
    }
    void HandleLook()
    {
        lookVector += actions.Player.Look.ReadValue<Vector2>() * Time.deltaTime * sensitivity;
        lookVector.y = Mathf.Clamp(lookVector.y, minY, maxY);
        playerCamera.transform.localRotation = Quaternion.Euler(-lookVector.y, 0, 0);
        gameObject.transform.rotation = Quaternion.Euler(0, lookVector.x, 0);
    }

    void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;
        if(!InputManager.Instance.MenuOpen)
        {
            moveInput = actions.Player.Move.ReadValue<Vector2>();
        }

        if (characterController.isGrounded)
        {
            float movementModifier = (actions.Player.Sprint.ReadValue<float>() > 0) ? sprintModifier : baseModifier;

            targetVelocity = (transform.right * moveInput.x * maxStrafeSpeed) + (transform.forward * moveInput.y * maxMovementSpeed * movementModifier);

            float speedChange = (targetVelocity.magnitude > currentVelocity.magnitude) ? acceleration : deceleration;

            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, speedChange);

            verticalVelocity = -1f;

            if (actions.Player.Jump.triggered && !InputManager.Instance.MenuOpen)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity;
        }

        currentVelocity.y = verticalVelocity;
        characterController.Move(currentVelocity * Time.fixedDeltaTime);
    }

    void HandleInventory()
    {
        if (!isLocalPlayer) return;

        inventoryCanvas.enabled = !inventoryCanvas.enabled;
        if (inventoryCanvas.enabled)
        {
            isInventoryOpen = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (Item slot in inventory.slots)
            {
                GameObject slotInstance = Instantiate(inventorySlot, inventoryContainer);
                Image slotImage = slotInstance.GetComponent<Image>();
                InventorySlotHandler slotInstanceHandler = slotInstance.GetComponent<InventorySlotHandler>();

                slotInstanceHandler.associatedItem = slot;
                slotInstanceHandler.descriptionBox = itemDescription;
                slotInstanceHandler.useButton = useButton;
                slotInstanceHandler.player = this;

                useButton.gameObject.SetActive(false);

                if (slot != null && slot.icon != null)
                    slotImage.sprite = slot.icon;
                else Destroy(slotInstance);
            }
        }
        else
        {
            isInventoryOpen = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            foreach (Transform child in inventoryContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void Interact()
    {
        if (!isLocalPlayer || InputManager.Instance.MenuOpen || isInventoryOpen) return;
        if (Physics.Raycast(new Ray(playerCamera.transform.position, playerCamera.transform.forward), out RaycastHit hit, maxInteractRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact(this);
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.Disable();
            actions.Player.Inventory.performed -= ctx => HandleInventory();
            actions.Player.Respawn.performed -= ctx => NetworkingManager.Instance.RespawnPlayer(connectionToClient);
        }
    }
    public bool GiveItem(Item item)
    {
        if (!isLocalPlayer) return false;

        int index = Array.IndexOf(inventory.slots, null);
        if (index != -1)
        {
            inventory.slots[index] = item;
            return true;
        }
        return false;
    }

    public void Equip(Item item)
    {
        if (!isLocalPlayer) return;
        if(item is GunItem gun)
        {
            if (gun.equippable)
            {
                equippedSlot = item;
                CmdEquipItem(gun.id);
            }

        }
        Debug.Log($"Equipped {item.name}");
    }

    [Command]
    public void CmdEquipItem(int id)
    {
        var item = ItemDatabase.Instance?.GetItemByID(id);
        if (item is GunItem gun)
        {
            if (gun == null || !gun.equippable) return;
            if (equippedItem != null)
            {
                NetworkServer.Destroy(equippedItem);
                equippedItem = null;
            }
            var spawnPos = equipMount != null ? equipMount.transform.position : transform.position;
            var spawnRot = equipMount != null ? equipMount.transform.rotation : transform.rotation;

            var weaponInstance = Instantiate(gun.prefab, spawnPos, spawnRot);

            var held = weaponInstance.GetComponent<HeldItem>();
            if (held != null)
                held.ownerNetId = netIdentity.netId;
            NetworkServer.Spawn(weaponInstance, connectionToClient);

            equippedItem = weaponInstance;
        }
    }
    //TODO:
    //Shooting mechanic
}
