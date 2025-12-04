using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    Rigidbody rb;

    PlayerInputManager inputManager;
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction parryAction;
    private InputAction overcommitAction;

    [Header("Movement Control")]
    [SerializeField] bool acceptMoveInput = true;
    bool isMoving = false;
    public float moveSpeed = 5f;

    public enum ParryState
    {
        None, // no parry being performed
        Anticipating, // player has initiated parry input, waiting for attack (automatically times out after a short duration)
        Blocking // player is successfully parrying an attack (state ends when animation finishes)
    }
    public ParryState currentParryState = ParryState.None;

    bool isPerfectParry;
    Vector2 lastParryDirection;

    private void Awake()
    {
        inputManager = PlayerInputManager.Instance;
        rb = GetComponent<Rigidbody>();

        moveAction = playerInput.actions["Move"];
        parryAction = playerInput.actions["Block"];
        overcommitAction = playerInput.actions["Commit"];
    }

    // parry ability
    public void OnInitialParry()
    {
        // when player performs parry input
        // check directional input to determine parry direction (if there's no directional input, return)
        if (lastParryDirection == Vector2.zero) return;

        Debug.Log("Parry initiated in direction: " + lastParryDirection);
        // should have a short delay before input is accepted again
    }
    public void OnOvercommitParry()
    {
        // can be activated on block (otherwise return)
    }

    private void Update()
    {
        if (inputManager.ParryPressed)
        {
            if (inputManager.Movement != Vector2.zero)
            {
                lastParryDirection = inputManager.Movement;
            }
            OnInitialParry();
        }

        if (acceptMoveInput)
        {
            GetMoveInput();
        }
    }

    private void FixedUpdate()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        ApplyMovement(movementInput, movement);
    }

    void GetMoveInput()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        // if position will change, apply movement
        if (movement != Vector3.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
    void ApplyMovement(Vector2 movementInput, Vector3 movement)
    {
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
        var isoMovement = matrix.MultiplyPoint3x4(movement);

        // rotate towards movement direction
        if (isoMovement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(isoMovement, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime)); // "maxDegreesDelta" is turn speed
        }

        if (isMoving)
        {
            rb.MovePosition(transform.position + isoMovement * Time.deltaTime * moveSpeed);
        }
    }
}
