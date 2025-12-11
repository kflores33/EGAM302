using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private ParryGuy inputManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            inputManager = new ParryGuy();
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnEnable()
    {
        inputManager.Enable();

            inputManager.Player.Move.started += DetectScheme;
            inputManager.Player.Look.started += DetectScheme;
            inputManager.Player.Block.started += DetectScheme;
            inputManager.Player.Commit.started += DetectScheme;
    }
    private void OnDisable() 
    {
            inputManager.Player.Move.started -= DetectScheme;
            inputManager.Player.Look.started -= DetectScheme;
            inputManager.Player.Block.started -= DetectScheme;
            inputManager.Player.Commit.started -= DetectScheme;

        inputManager.Disable();
    }

    public Vector2 Movement => inputManager.Player.Move.ReadValue<Vector2>();
    public Vector2 Look => inputManager.Player.Look.ReadValue<Vector2>();
    public bool ParryPressed => inputManager.Player.Block.WasPressedThisFrame();
    public bool OvercommitPressed => inputManager.Player.Commit.WasPressedThisFrame();

    public void EnableGameplayInput() => inputManager.Player.Enable();
    public void DisableGameplayInput() => inputManager.Player.Disable();

    public enum ControlScheme { MouseKeyboard, Gamepad }
    public static ControlScheme CurrentScheme { get; private set; }

    public class Devices
    {
        public Keyboard keyboard;
        public Mouse mouse;
        public Gamepad gamepad;
    }
    public Devices deviceRef { get; private set; } = new Devices();

    public Gamepad CurrentGamepad => deviceRef.gamepad;
    public Keyboard CurrentKeyboard => deviceRef.keyboard;
    public Mouse CurrentMouse => deviceRef.mouse;

    private void DetectScheme(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (device is Gamepad && deviceRef.gamepad == null) deviceRef.gamepad = (Gamepad)device;
        else if (device is Mouse && deviceRef.mouse == null) deviceRef.mouse = (Mouse)device;
        else if (device is Keyboard && deviceRef.keyboard == null) deviceRef.keyboard = (Keyboard)device;

            CurrentScheme = (device is Gamepad) ? ControlScheme.Gamepad : ControlScheme.MouseKeyboard;
    }
}
