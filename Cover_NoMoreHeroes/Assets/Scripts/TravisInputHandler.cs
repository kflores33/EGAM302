using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class TravisInputHandler : MonoBehaviour
{
    Animator animator;
    Keyboard kbd;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        kbd = Keyboard.current;
    }

    public int combo = 0;
    public bool attackQd; // is there an attack queued up?
    public bool canPlayQueued; // can the queued attack be played?

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

    private void Update()
    {
        if (kbd.qKey.wasPressedThisFrame)
        {
            HandleAttackInput(0); // Low stance
        }
        if (kbd.eKey.wasPressedThisFrame)
        {
            HandleAttackInput(1); // High stance
        }

        if (canPlayQueued)
        {
            PlayQueuedAttack();
        }
        else if (attackQd && !canPlayQueued)
        {
            Debug.Log("Attack has been queued but cannot play yet.");
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
    }
    public void OnCanPlayQueued()
    {
        canPlayQueued = true;
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
        
        Debug.Log("Starting new combo");
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
