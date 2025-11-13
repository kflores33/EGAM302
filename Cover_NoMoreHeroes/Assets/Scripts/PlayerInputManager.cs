using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private NMHCover inputManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            inputManager = new NMHCover();
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnEnable() => inputManager.Enable();
    private void OnDisable() => inputManager.Disable();

    public Vector2 Movement => inputManager.Travis.Move.ReadValue<Vector2>();
    public bool LowAttackPressed => inputManager.Travis.LowAttack.WasPressedThisFrame();
    public bool HighAttackPressed => inputManager.Travis.HighAttack.WasPressedThisFrame();

    public void EnableGameplayInput() => inputManager.Travis.Enable();
    public void DisableGameplayInput() => inputManager.Travis.Disable();
}
