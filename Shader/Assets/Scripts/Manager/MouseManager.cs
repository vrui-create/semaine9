using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Paramètres initiaux")]
    public bool lockOnStart = false;
    public bool allowLockSwitch = true;
    public bool allowLock = true;

    private InputSystem_Actions inputActions;
    private bool isLocked;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new InputSystem_Actions();
        ApplyCursorState(lockOnStart);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Echap.performed += OnEchap;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        inputActions.Player.Echap.performed -= OnEchap;
        inputActions.Player.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":
                ApplyCursorState(false);
                allowLockSwitch = false;
                break;

            case "Village":
                ApplyCursorState(true);
                allowLockSwitch = true;
                break;

            case "Base":
                ApplyCursorState(true);
                allowLockSwitch = true;
                break;

            default:
                ApplyCursorState(false);
                break;
        }
    }

    private void OnEchap(InputAction.CallbackContext context)
    {
        if (context.performed && allowLockSwitch)
        {
            ToggleCursor();
            SceneLoader.Instance.LoadScene("Menu");
        }
    }

    public void ToggleCursor()
    {
        if (!allowLock)
        {
            ApplyCursorState(false);
            return;
        }

        ApplyCursorState(!isLocked);
    }

    public void ApplyCursorState(bool lockCursor)
    {
        if (lockCursor && !allowLock)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isLocked = false;
            return;
        }

        isLocked = lockCursor;
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    public void SetLockPermission(bool allowed)
    {
        allowLock = allowed;

        if (!allowLock)
        {
            ApplyCursorState(false);
        }
    }

    public bool IsLocked() => isLocked;
}
