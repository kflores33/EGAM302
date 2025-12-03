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

    private void OnEnable() => inputManager.Enable();
    private void OnDisable() => inputManager.Disable();

    public Vector2 Movement => inputManager.Player.Move.ReadValue<Vector2>();
    public bool ParryPressed => inputManager.Player.Block.WasPressedThisFrame();
    public bool OvercommitPressed => inputManager.Player.Commit.WasPressedThisFrame();

    public void EnableGameplayInput() => inputManager.Player.Enable();
    public void DisableGameplayInput() => inputManager.Player.Disable();
}
