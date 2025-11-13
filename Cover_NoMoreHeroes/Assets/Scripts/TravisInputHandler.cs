using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class TravisInputHandler : MonoBehaviour
{
    Animator animator;
    Keyboard kbd;
    Rigidbody rb;

    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lowATKAction;
    private InputAction highATKAction;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        kbd = Keyboard.current;
        rb = GetComponent<Rigidbody>();

        moveAction = playerInput.actions["Move"];
        lowATKAction = playerInput.actions["LowAttack"];
        highATKAction = playerInput.actions["HighAttack"];
    }

    public int combo = 0;
    public bool attackQd; // is there an attack queued up?
    public bool canPlayQueued; // can the queued attack be played?

    [Header("Movement Control")]
    [SerializeField] bool canMove = true;
    public float moveSpeed = 5f;

    public enum Stance
    {
        Low,
        High
    }

    [Header("Stance Info")]
    public Stance currentStance = Stance.Low;
    public Stance previousStance = Stance.Low;

    [Header("Attack Animations")]
    [SerializeField] List<string> lowATKStrings = new List<string> 
    { 
        "ATTACK_LOW_1", 
        "ATTACK_LOW_2",
        "ATTACK_LOW_3",
        "ATTACK_LOW_4"
    };
    [SerializeField] List<string> highATKStrings = new List<string>
    {
        "ATTACK_HIGH_1",
        "ATTACK_HIGH_2",
        "ATTACK_HIGH_1",
        "ATTACK_HIGH_2"
    };
    [SerializeField] int attackInString;

    [Header("Move Animations")]
    bool canLoopRun = false;
    bool canStartIdle = true;
    bool canStartRun = false;

    private void Update()
    {
        if (lowATKAction.WasPressedThisFrame())
        {
            HandleAttackInput(0); // Low stance
        }
        if (highATKAction.WasPressedThisFrame())
        {
            HandleAttackInput(1); // High stance
        }

        if (canMove)
            GetMoveInput();

        if (canPlayQueued)
        {
            PlayQueuedAttack();
        }

        if (isMoving && canStartRun)
        {
            //animator.Play("RUN");
            canStartRun = false;
            animator.Play("RUN");
            canStartIdle = true;
        }
        else if (!isMoving && canStartIdle)
        {
            canStartIdle = false;
            animator.Play("IdleBlend");
            canStartRun = true;
        }
    }

    bool isMoving;
    bool applyAttackMovement;
    float attackMoveAmount = 0.5f;
    private void FixedUpdate()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        ApplyMovement(movementInput, movement);

        if (applyAttackMovement)
        {
            // apply slight forward movement during attack
            applyAttackMovement = false;
            rb.MovePosition(transform.position + transform.forward * attackMoveAmount);
        }
    }

    //public void OnRunAnimUpdate(bool canLoop)
    //{
    //    if (canLoop)
    //    {
    //        canLoopRun = true;
    //    }
    //    else
    //    {
    //        canLoopRun = false;
    //    }
    //}

    public void OnApplyAttackMovement(float moveAmount)
    {
        applyAttackMovement = true;
        attackMoveAmount = moveAmount;

        //Debug.Log("Applying attack movement: " + moveAmount);
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
        // change direction based on camera
        Vector3 camForward = Camera.main.transform.forward; // cam reference
                camForward.y = 0; // flatten on y axis
        Quaternion camRotation = Quaternion.LookRotation(camForward); // find the rotation of the camera on the y axis only

        movement = camRotation * movement; // update movement vector to be relative to camera

        // rotate towards movement direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime)); // "maxDegreesDelta" is turn speed
        }

        if (isMoving)
        {
            rb.MovePosition(transform.position + movement * Time.deltaTime * moveSpeed);
            canStartRun = true;
        }
    }

    void HandleAttackInput(int newStance)
    {
        previousStance = currentStance;

        currentStance = (Stance)newStance;

        // check if no combo
        if (combo == 0)
        {
            PlayFirstAttack();
        }
        else if (combo > 0)
        {
            attackQd = true;
        }
    }
    public void OnAttackStart()
    {
        canPlayQueued = false;
        canMove = false;
        isMoving = false;
    }
    public void OnCanPlayQueued()
    {
        canPlayQueued = true;
        canMove = true;
    }
    public void OnComboBreak()
    {
        combo = 0;
        attackInString = 0;

        animator.SetFloat("HLState", currentStance == Stance.Low ? 0f : 1f);
    }

    public void PlayFirstAttack()
    {
        combo++;
        
        if (previousStance == Stance.Low)
        {
            animator.Play(lowATKStrings[0]);
        }
        else if (previousStance == Stance.High)
        {
            animator.Play(highATKStrings[0]);
        }

        if (currentStance != previousStance) attackInString = 1;
        
        //Debug.Log("Starting new combo");
    }

    public void PlayQueuedAttack()
    {
        if (!attackQd) return;

        combo++;
        attackQd = false;
            
        if (currentStance != previousStance)
        {
            attackInString = 0;
        }
        else
        {
            attackInString++;
        }

        if (attackInString >= 4)
        {
            attackInString = 0; // loop back to first attack
        }

        // play animation based on stance
        if (currentStance == Stance.Low)
        {
            animator.Play(lowATKStrings[attackInString]);
        }
        else if (currentStance == Stance.High)
        {
            animator.Play(highATKStrings[attackInString]);
        }
    }
}
