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
    [SerializeField] LayerMask parryableLayerMask;

    bool applyRootMotion;

    // unity input system
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction parryAction;
    private InputAction overcommitAction;

    [Header("Movement Control")]
    [SerializeField] bool acceptMoveInput = true;
    [SerializeField] bool isMoving = false;
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
    public void OnInitialParry() // called when parry input is detected
    {
        // check directional input to determine parry direction (if there's no directional input, return)
        if (lastParryDirection == Vector2.zero) return;

        // should have a short delay before input is accepted again
        PlayAnimation("ParryStance", false, 0.1f);
        
        currentParryState = ParryState.Anticipating;
    }
    public void OnOvercommitParry()
    {
        // can be activated on block (otherwise return)
        if (lastParryDirection == Vector2.zero) return;

        ResetParryState();
        PlayAnimation("Overcommit", true, 0.1f);
    }

    void DetectParryableObject() // called during Anticipating state in Update()
    {
        // boxcast in front of player (player rotation is already aligned with block direction)
        if (currentParryState != ParryState.Anticipating) return;

        Vector3 boxCenter = transform.position + transform.forward * 0.3f + Vector3.up * 1f;
        Vector3 halfExtents = new Vector3(0.5f, 1, 0.3f);
        float maxDistance = 1f;

        bool HitDetect = Physics.BoxCast(boxCenter, halfExtents, transform.forward, out RaycastHit hitInfo, Quaternion.identity, maxDistance, parryableLayerMask.value, QueryTriggerInteraction.Collide);
        if (HitDetect)
        {
            if (hitInfo.collider != null)
            {
                hitInfo.collider.GetComponentInParent<Parryable>()?.OnBlock();

                currentParryState = ParryState.Blocking;
                PlayAnimation("ParryReact", true, 0.1f);
            }
        }
    }

    #region Animation Event Callbacks
    public void OnAnimationEnd()
    {
        Debug.Log("Current animation ended.");

        // Check which animation is currently playing
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log($"Current animation: {GetAnimationName(stateInfo)}");

        if (currentParryState != ParryState.None)
            currentParryState = ParryState.None;

        if (canStartMoveAnim == false)
            canStartMoveAnim = true;
    }
    public void OnApplyRunLoop(bool apply)
    {
        canStartMoveAnim = apply ? true : false;
    }

    #endregion

    void ResetParryState()
    {
        currentParryState = ParryState.None;
        lastParryDirection = Vector2.zero;
    }

    private void Update()
    {
        if (parryAction.WasPressedThisFrame())
        {
            if (currentParryState == ParryState.Anticipating) return; // already parrying

            if (moveAction.ReadValue<Vector2>() != Vector2.zero)
            {
                lastParryDirection = moveAction.ReadValue<Vector2>();
            }
            OnInitialParry();
        }
        if (overcommitAction.WasPressedThisFrame())
        {
            if (currentParryState != ParryState.Blocking) return; // can only overcommit when blocking

            if (moveAction.ReadValue<Vector2>() != Vector2.zero)
            {
                lastParryDirection = moveAction.ReadValue<Vector2>();
            }
            OnOvercommitParry();
        }

        HandleParryState();

        if (acceptMoveInput)
            GetMoveInput();

        // handle move/idle animations
        if (isMoving && canStartMoveAnim)
        {
            canStartMoveAnim = false;
            PlayAnimation("Movement", false, 0.2f);
            canStartIdleAnim = true;
        }
        else if (!isMoving && canStartIdleAnim)
        {
            canStartIdleAnim = false;
            PlayAnimation("Idle", true, 0.2f);
            canStartMoveAnim = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        ApplyMovement(movementInput, movement);
    }

    private void HandleParryState() // to be called in Update() | manages input acceptance and parryable object detection
    {
        if (currentParryState == ParryState.Anticipating)
        {
            DetectParryableObject();

            // Debug current animation progress
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ParryStance"))
            {
                float normalizedTime = stateInfo.normalizedTime;
                //Debug.Log($"ParryStance animation progress: {normalizedTime * 100}%");

                // If animation is interrupted (normalizedTime < 1), manually reset
                if (normalizedTime < 1 && !animator.IsInTransition(0))
                {
                    // Check if we switched to a different animation
                    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                    if (clipInfo.Length > 0 && clipInfo[0].clip.name != "ParryStance")
                    {
                        Debug.Log("Parry animation was interrupted!");
                        ResetParryState();
                    }
                }
            }

        }

        if (currentParryState == ParryState.Blocking)
        {
            // Debug current animation progress
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ParryReact"))
            {
                float normalizedTime = stateInfo.normalizedTime;
                //Debug.Log($"ParryStance animation progress: {normalizedTime * 100}%");

                // If animation is interrupted (normalizedTime < 1), manually reset
                if (normalizedTime < 1 && !animator.IsInTransition(0))
                {
                    // Check if we switched to a different animation
                    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                    if (clipInfo.Length > 0 && clipInfo[0].clip.name != "ParryReact")
                    {
                        //Debug.Log("Parry animation was interrupted!");
                        ResetParryState();
                    }
                }
            }
        }

        if (currentParryState == ParryState.None)
        {
            if (acceptMoveInput == false)
                acceptMoveInput = true;
        }
        else
        {
            if (acceptMoveInput == true)
                acceptMoveInput = false;
        }
    } 

    #region Movement Functions
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

        if (isMoving && currentParryState != ParryState.Anticipating)
        {
            characterController.Move(isoMovement * Time.deltaTime * finalMoveSpeed);
        }
    }
    #endregion

    #region Animation Playback
    private void OnAnimatorMove()
    {
        if (applyRootMotion)
        {
            Vector3 velocity = animator.deltaPosition;
            characterController.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }
    private void PlayAnimation(string animName, bool applyRootMotion = false, float transitionAmount = 0)
    {
        this.applyRootMotion = applyRootMotion;

        if (transitionAmount > 0)
        {
            animator.CrossFade(animName, transitionAmount);
            return;
        }
        else
            animator.Play(animName);
    }
    #endregion

    #region Debugging
    [Header("Animation Debug")]
    [SerializeField] bool debugAnimationEvents = true;

    private float GetCurrentAnimationFrame()
    {
        if (animator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            var clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime * clip.frameRate * clip.length;
        }
        return 0;
    }
    private string GetAnimationName(AnimatorStateInfo stateInfo)
    {
        // You might need a better way to identify animations
        return stateInfo.IsName("ParryStance") ? "ParryStance" :
               stateInfo.IsName("Movement") ? "Movement" :
               stateInfo.IsName("Idle") ? "Idle" : "Unknown Animation";
    }

    private void OnGUI()
    {
        if (!debugAnimationEvents) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"Current Parry State: {currentParryState}");
        GUILayout.Label($"Last Animation Frame: {GetCurrentAnimationFrame()}");
        GUILayout.Label($"Is Animator Playing: {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
        GUILayout.EndArea();
    }
    private void OnDrawGizmosSelected()
    {
        // draw boxcast area
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * 0.3f + Vector3.up * 1f;
        Vector3 halfExtents = new Vector3(0.5f, 1, 0.3f);
        Gizmos.DrawWireCube(boxCenter, halfExtents * 2);
    }
    #endregion
}
