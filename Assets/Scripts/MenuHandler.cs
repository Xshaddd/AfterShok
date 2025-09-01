using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] Canvas menuCanvas;
    InputSystem_Actions actions;

    public MenuHandler Instance { get; internal set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        actions = new InputSystem_Actions();
        actions.Enable();
        actions.Player.OpenMenu.performed += ctx => HandleMenu();
    }

    void HandleMenu()
    {
        InputManager.Instance.SetMenuState(!InputManager.Instance.MenuOpen);
        if (InputManager.Instance.MenuOpen)
        {
            menuCanvas.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Resume()
    {
        InputManager.Instance.SetMenuState(false);
        menuCanvas.enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
