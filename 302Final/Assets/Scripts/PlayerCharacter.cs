using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;

public interface IDamageableObj
{
    public void TakeDamage(float damage);
}

public class PlayerCharacter : MonoBehaviour
{
    UnityEvent OnTakeDamage;
    public UnityEvent<bool> OnTimeSlowed;

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
    private InputAction lookAction;
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

    [Header("Time Slow Settings")]
    public float timeSlowAmount = 0.5f;
    public float timeSlowDuration = 1;
    public float slowRate = 5;
    public float speedUpRate = 5;
    
    // misc
    Vector2 lastParryDirection;
    Parryable closestParryable;

    Coroutine timeSlowCoroutine = null;   
    float originalFixedDelta;

    private void Awake()
    {
        inputManager = PlayerInputManager.Instance;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        parryAction = playerInput.actions["Block"];
        overcommitAction = playerInput.actions["Commit"];

        originalFixedDelta = Time.fixedDeltaTime;
    }

    // parry ability
    public void OnInitialParry() // called when parry input is detected
    {
        // check directional input to determine parry direction (if there's no directional input, return)
        //if (lastParryDirection == Vector2.zero) return;

        // should have a short delay before input is accepted again
        currentParryState = ParryState.Anticipating;
        PlayAnimation("ParryStance", false, 0.1f);
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

        //Vector3 boxCenter = transform.position + transform.forward * 0.3f + Vector3.up * 1f;
        //Vector3 halfExtents = new Vector3(0.5f, 1, 0.3f);

        Vector3 point1 = transform.position;
                point1.y += 1f;
        Vector3 point2 = point1;

        //float radius = 1f;
        //float maxDistance = 1f;

        //bool HitDetect = Physics.CapsuleCast(point1, point2, radius, Vector3.forward, out RaycastHit hitInfo, maxDistance, parryableLayerMask.value, QueryTriggerInteraction.Collide);
        //if (HitDetect)
        //{
        //    Parryable parryable = hitInfo.collider.GetComponentInParent<Parryable>();
        //    if (parryable != null)
        //    {
        //        Debug.Log($"Found Parryable: {parryable.name}");
        //        closestParryable = parryable;
        //        EnterBlockState();
        //    }
        //    else
        //    {
        //        Debug.Log("No Parryable component found");
        //    }
        //}

        Vector3 center = transform.position + Vector3.up * 1f; // Center at player's chest height
        float radius = 2f;

        // Check all colliders within radius
        Collider[] hitColliders = Physics.OverlapSphere(
            center,
            radius,
            parryableLayerMask.value,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider collider in hitColliders)
        {
            Parryable parryable = collider.GetComponentInParent<Parryable>();
            if (parryable != null)
            {
                closestParryable = parryable;
                EnterBlockState();
                return;
            }
        }
    }

    void EnterBlockState()
    {
        currentParryState = ParryState.Blocking;
        PlayAnimation("ParryReact", true);

        Time.timeScale = 1;

        StartTimeSlow();
    }

    public void StartTimeSlow()
    {
        if (timeSlowCoroutine != null)
            StopCoroutine(timeSlowCoroutine);

        timeSlowCoroutine = StartCoroutine(TimeSlowCoroutine());
    }
    IEnumerator TimeSlowCoroutine()
    {
        float targetScale = 0.1f;

        OnTimeSlowed?.Invoke(true);
        
        while (Time.timeScale > targetScale + 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetScale, Time.unscaledDeltaTime * 5f);
            Debug.Log($"Slowing... Current: {Time.timeScale}");

            Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            yield return null;
        }
        Time.timeScale = targetScale;
        Debug.Log($"Fully slowed. Scale: {Time.timeScale}");

        yield return new WaitForSecondsRealtime(timeSlowDuration);

        OnTimeSlowed?.Invoke(false);
        
        while (Time.timeScale < 0.99f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.unscaledDeltaTime * 5f);
            Debug.Log($"Speeding up... Current: {Time.timeScale}");

            Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Default Unity value
        Debug.Log($"Time restored to normal. Scale: {Time.timeScale}");

        yield return null;
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

            OnInitialParry();
        }
        //if (overcommitAction.WasPressedThisFrame())
        //{
        //    if (currentParryState != ParryState.Blocking) return; // can only overcommit when blocking

        //    if (moveAction.ReadValue<Vector2>() != Vector2.zero)
        //    {
        //        lastParryDirection = moveAction.ReadValue<Vector2>();
        //    }
        //    OnOvercommitParry();
        //}

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

        RotateToTarget();
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

    void RotateToTarget()
    {
        if (closestParryable == null) return;

        Vector3 targetPos = closestParryable.transform.position - transform.position;
        targetPos.y = 0;

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
        var isoMovement = matrix.MultiplyPoint3x4(targetPos.normalized);

        // rotate towards movement direction
        if (isoMovement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(isoMovement, Vector3.up);
            Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, toRotation, 960 * Time.unscaledDeltaTime); // "maxDegreesDelta" is turn speed

            transform.rotation = finalRotation;

            if (finalRotation == toRotation)
            {
                closestParryable = null; // reset after rotation is complete
            }
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
        Vector3 boxCenter = transform.position + Vector3.up * 1f;
        Vector3 halfExtents = new Vector3(1f, 1, 1f);
        Gizmos.DrawWireSphere(boxCenter, 2);
    }
    #endregion
}
