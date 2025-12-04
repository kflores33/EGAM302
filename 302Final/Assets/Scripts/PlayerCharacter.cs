using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public interface IDamageableObj
{
    public void TakeDamage(float damage);
}

public class PlayerCharacter : MonoBehaviour
{
    UnityEvent OnTakeDamage;

    // references
    Rigidbody rb;
    Animator animator;
    PlayerInputManager inputManager;
    CharacterController characterController;

    bool applyRootMotion;

    // unity input system
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction parryAction;
    private InputAction overcommitAction;

    [Header("Movement Control")]
    [SerializeField] bool acceptMoveInput = true;
    bool isMoving = false;
    public float moveSpeedWalk = 4f;
    public float moveSpeedRun = 8f;

    bool canStartMoveAnim = true;
    bool canStartIdleAnim;

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
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

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
        if (parryAction.WasPressedThisFrame())
        {
            if (moveAction.ReadValue<Vector2>() != Vector2.zero)
            {
                lastParryDirection = moveAction.ReadValue<Vector2>();
            }
            OnInitialParry();
        }

        if (acceptMoveInput)
        {
            GetMoveInput();
        }

        // handle move/idle animations
        if (isMoving && canStartMoveAnim)
        {
            canStartMoveAnim = false;
            animator.Play("Movement");
            canStartIdleAnim = true;
        }
        else if (!isMoving && canStartIdleAnim)
        {
            canStartIdleAnim = false;
            animator.Play("Idle");
            canStartMoveAnim = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        ApplyMovement(movementInput, movement);
    }

    public void OnApplyRunLoop(bool apply)
    {
        canStartMoveAnim = apply ? true : false;
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


        animator.SetFloat("Speed", movementInput.magnitude);
    }
    void ApplyMovement(Vector2 movementInput, Vector3 movement)
    {
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
        var isoMovement = matrix.MultiplyPoint3x4(movement.normalized);

        // rotate towards movement direction
        if (isoMovement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(isoMovement, Vector3.up);
            Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime); // "maxDegreesDelta" is turn speed

            transform.rotation = finalRotation;
        }

       //set speed for walk vs run 
       float finalMoveSpeed = 0;

        if (movementInput.magnitude > 0 && movementInput.magnitude <= 0.5f) finalMoveSpeed = moveSpeedWalk;
        else if (movementInput.magnitude > 0.5) finalMoveSpeed = moveSpeedRun;

        if (isMoving)
        {
            characterController.Move(isoMovement * Time.deltaTime * finalMoveSpeed);
        }
    }

    private void OnAnimatorMove()
    {
        if (applyRootMotion)
        {
            Vector3 velocity = animator.deltaPosition;
            characterController.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }
}
