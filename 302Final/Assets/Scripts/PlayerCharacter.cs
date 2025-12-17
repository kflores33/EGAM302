using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;

public interface IDamageableObj
{
    public void TakeDamage(int damage);
}

public class PlayerCharacter : MonoBehaviour, IDamageableObj
{
    UnityEvent OnTakeDamage;
    public UnityEvent<bool> OnTimeSlowed;
    public UnityEvent<int, int> OnTakeDamageEvent;

    [Header("References")]
    [SerializeField] GameObject dotsIcon;
    [SerializeField] GameObject toRotate;
    [SerializeField] LayerMask parryableLayerMask;
    Rigidbody rb;
    Animator animator;
    PlayerInputManager inputManager;
    CharacterController characterController;

    bool applyRootMotion;

    // unity input system
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction parryAction;
    private InputAction overcommitAction;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

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

    [Header("Parry Variables")]
    public float parryDetectRadius = 2;
    
    // misc
    Vector2 lastParryDirection;
    Vector2 lastBlockDirection;
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

        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnTakeDamageEvent?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("oh no..,,, you dies....restart?");
        }
    }

    #region Player Actions
    void OnInitialParry() // called when parry input is detected
    {
        // should have a short delay before input is accepted again
        currentParryState = ParryState.Anticipating;
        PlayAnimation("ParryStance", false, 0.1f);
    }
    void OnBlock()
    {
        Debug.Log("Blocked Attack");

        currentParryState = ParryState.None;

        if (dotsIcon != null)
        {
            dotsIcon.SetActive(false);
        }
    }
    void OnDeflect(Vector3 dir) // actually execute parry
    {
        currentParryState = ParryState.None;
        //Debug.Log("Deflected Attack!");

        if (closestParryable != null)
        {
            if (closestParryable.GetComponent<ProjectileBehavior>() != null)
            {
                ProjectileBehavior proj = closestParryable.GetComponent<ProjectileBehavior>();
                proj.OnParried(dir.normalized);
            }
        }
        else
        {
            Debug.LogWarning("no parryable object dumbass lol");
        }


        if (dotsIcon != null)
        {
            dotsIcon.SetActive(false);
        }
    }
    #endregion

    void DetectParryableObject() // called during Anticipating state in Update()
    {
        // boxcast in front of player (player rotation is already aligned with block direction)
        if (currentParryState != ParryState.Anticipating) return;

        Vector3 point1 = transform.position;
                point1.y += 1f;
        Vector3 point2 = point1;

        Vector3 center = transform.position + Vector3.up * 1f; // Center at player's chest height

        closestParryable = null;

        // Check all colliders within radius
        Collider[] hitColliders = Physics.OverlapSphere(center, parryDetectRadius, parryableLayerMask.value, QueryTriggerInteraction.Collide);

        foreach (Collider collider in hitColliders)
        {
            Parryable parryable = collider.GetComponentInParent<Parryable>();
            if (parryable != null)
            {
                // make sure it's actually the closest one
                float distanceFromCurrent = (collider.transform.position - transform.position).magnitude;

                if (closestParryable != null)
                {
                    float distanceFromClosest = (closestParryable.transform.position - transform.position).magnitude;

                    if (distanceFromCurrent < distanceFromClosest)
                        closestParryable = parryable;
                }
                else
                    closestParryable = parryable;

                lastBlockDirection = closestParryable.transform.position - transform.position;

                EnterBlockState();
                return;
            }
        }
    }

    #region Block State Functions
    void EnterBlockState()
    {
        currentParryState = ParryState.Blocking;
        PlayAnimation("ParryReact", true);

        lastParryDirection = Vector2.zero;
        Time.timeScale = 1;

        StartTimeSlow();
    }

    public void StartTimeSlow()
    {
        if (timeSlowCoroutine != null)
            StopCoroutine(timeSlowCoroutine);

        timeSlowCoroutine = StartCoroutine(TimeSlowCoroutine());
    }
    public void StopTimeSlow()
    {
        if (timeSlowCoroutine != null)
        {
            StopCoroutine(timeSlowCoroutine);
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;

            timeSlowCoroutine = null;
        }
        OnTimeSlowed?.Invoke(false);

        if (dotsIcon != null)
        {
            if (dotsIcon.activeInHierarchy)
                dotsIcon.SetActive(false);
        }
        if (lastParryDirection != Vector2.zero)
        {
            lastParryDirection = Vector2.zero;
        }

        //Debug.Log($"Time restored to normal. Scale: {Time.timeScale}");
    }
    IEnumerator TimeSlowCoroutine()
    {
        float targetScale = 0.1f;

        OnTimeSlowed?.Invoke(true);
        
        while (Time.timeScale > targetScale + 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetScale, Time.unscaledDeltaTime * 5f);
            //Debug.Log($"Slowing... Current: {Time.timeScale}");

            Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            yield return null;
        }
        Time.timeScale = targetScale;
        //Debug.Log($"Fully slowed. Scale: {Time.timeScale}");

        yield return new WaitForSecondsRealtime(timeSlowDuration);

        OnTimeSlowed?.Invoke(false);
        
        while (Time.timeScale < 0.99f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.unscaledDeltaTime * 5f);
            //Debug.Log($"Speeding up... Current: {Time.timeScale}");

            Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            yield return null;
        }

        //Time.timeScale = 1f;
        //Time.fixedDeltaTime = 0.02f; // Default Unity value
        //Debug.Log($"Time restored to normal. Scale: {Time.timeScale}");

        StopTimeSlow();
        yield return null;
    }
    #endregion

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
        if (parryAction.WasPressedThisFrame()) // check for parry input
        {
            if (currentParryState == ParryState.Anticipating) return; // already parrying
            if (currentParryState == ParryState.Blocking) // while blocking; end state early and block attack
            {
                StopTimeSlow();

                OnBlock();
                return;
            }

            OnInitialParry();
        }

        if (currentParryState == ParryState.Blocking) // parry related; check for deflect input
        {
            // log direction
            if (PlayerInputManager.CurrentScheme == PlayerInputManager.ControlScheme.Gamepad)
            {
                if (lookAction.ReadValue<Vector2>() != Vector2.zero)
                {
                    lastParryDirection = lookAction.ReadValue<Vector2>();

                    if (dotsIcon != null)
                        if (!dotsIcon.activeInHierarchy)
                            dotsIcon.SetActive(true);
                }
                else if (moveAction.ReadValue<Vector2>() != Vector2.zero)
                {
                    lastParryDirection = moveAction.ReadValue<Vector2>();

                    if (dotsIcon != null)
                        if (!dotsIcon.activeInHierarchy)
                            dotsIcon.SetActive(true);
                }
                else // if they're both zero, compare to last direction
                {
                    if (lastParryDirection != Vector2.zero) // if there's a stored directional input from last frame, reflect attack
                    {
                        // reflect in last direction
                        Vector3 reflectDir = dotsIcon.transform.forward;// new Vector3(lastParryDirection.x, 0, lastParryDirection.y);
                        Debug.DrawLine(transform.position, transform.position + reflectDir * 20, Color.magenta, 1f);

                        OnDeflect(reflectDir);

                        StopTimeSlow();

                        lastParryDirection = Vector2.zero;
                    }
                }
            }
            else if (PlayerInputManager.CurrentScheme == PlayerInputManager.ControlScheme.MouseKeyboard)
            {
                //if (lookAction.ReadValue<Vector2>() != Vector2.zero)
                //{
                //    if (dotsIcon != null)
                //        if (!dotsIcon.activeInHierarchy)
                //            dotsIcon.SetActive(true);
                //}       
                //else if (moveAction.ReadValue<Vector2>() != Vector2.zero)
                //{
                //    lastParryDirection = moveAction.ReadValue<Vector2>();

                //    if (dotsIcon != null)
                //        if (!dotsIcon.activeInHierarchy)
                //            dotsIcon.SetActive(true);
                //}

                //if (overcommitAction.WasPressedThisFrame())
                //{
                //    Vector3 reflectDir = new Vector3(lastBlockDirection.x, 0, lastBlockDirection.y);

                //    var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
                //    var isoDir = matrix.MultiplyPoint3x4(reflectDir.normalized);

                //    OnDeflect(isoDir);

                //    StopTimeSlow();

                //    lastParryDirection = Vector2.zero;
                //    lastBlockDirection = Vector2.zero;
                //}
            }
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

    private void HandleParryState() // to be called in Update() | manages input acceptance and parryable object detection
    {
        // prevent animation cancel mishaps
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

        // only allow movement input when not parrying
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

    private void FixedUpdate()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        ApplyMovement(movementInput, movement);

        if (lookAction.ReadValue<Vector2>() != Vector2.zero && currentParryState == ParryState.None)
        {
            RotateCharacter(lookAction.ReadValue<Vector2>());
        }

        RotateToTarget();

        if (dotsIcon != null)
        {
            if (dotsIcon.activeInHierarchy == true)
            {
                Vector3 targetPos = new Vector3(lastParryDirection.x, 0, lastParryDirection.y);

                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
                var isoDir = matrix.MultiplyPoint3x4(targetPos.normalized);

                Quaternion toRotation = Quaternion.LookRotation(isoDir, Vector3.up);
                Quaternion finalRotation = Quaternion.RotateTowards(dotsIcon.transform.rotation, toRotation, 960 * Time.unscaledDeltaTime); // "maxDegreesDelta" is turn speed

                dotsIcon.transform.rotation = finalRotation;
            }
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
            Quaternion finalRotation = Quaternion.RotateTowards(toRotate.transform.rotation, toRotation, 720 * Time.deltaTime); // "maxDegreesDelta" is turn speed

            toRotate.transform.rotation = finalRotation;
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
    void RotateCharacter(Vector2 lookInput)
    {
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix
        var isoLook = matrix.MultiplyPoint3x4(new Vector3(lookInput.x, 0, lookInput.y).normalized);

        if (isoLook != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(isoLook, Vector3.up);
            Quaternion finalRotation = Quaternion.RotateTowards(toRotate.transform.rotation, toRotation, 720 * Time.deltaTime); // "maxDegreesDelta" is turn speed

            toRotate.transform.rotation = finalRotation;
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
            Quaternion finalRotation = Quaternion.RotateTowards(toRotate.transform.rotation, toRotation, 960 * Time.unscaledDeltaTime); // "maxDegreesDelta" is turn speed

            toRotate.transform.rotation = finalRotation;

            if (finalRotation == toRotation)
            {
                //closestParryable = null; // reset after rotation is complete
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
        //GUILayout.Label($"Last Animation Frame: {GetCurrentAnimationFrame()}");
        //GUILayout.Label($"Is Animator Playing: {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
        GUILayout.EndArea();
    }
    private void OnDrawGizmosSelected()
    {
        // draw boxcast area
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.up * 1f;
        //Vector3 halfExtents = new Vector3(1f, 1, 1f);
        Gizmos.DrawWireSphere(center, parryDetectRadius);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.GetComponentInParent<ProjectileBehavior>() != null)
        {
            var proj = other.GetComponentInParent<ProjectileBehavior>();
            if (proj.currentState == ProjectileBehavior.ObjectState.None && currentParryState != ParryState.Blocking)
            {
                TakeDamage(proj.damageToDeal);
            }
        }
    }
}
