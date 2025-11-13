using UnityEngine;
using UnityEngine.InputSystem;

// animation events on imported animations: https://discussions.unity.com/t/adding-animation-events-on-imported-clips/663362
public class TravisAnimTestScript : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        kbd = Keyboard.current;
    }

    public int combo = 0;
    Keyboard kbd;
    public bool attackQd; // is there an attack queued up?
    public bool canPlayQueued;

    // call a function for the combo to drop when animation event happens

    private void Update()
    {
        if (kbd.hKey.wasPressedThisFrame)
        {
            Debug.Log("the key was pressed yay");
            TryAttack();
        }

        if (attackQd)
        {
            if (canPlayQueued)
            {
                canPlayQueued = false;
                
                animator.SetTrigger("isCombo");
                combo++;

                attackQd = false;
            }
            else if(combo == 0)
            {
                TryAttack();
            }
        }
    }

    public void TryResetCombo()
    {
        combo = 0;
        animator.ResetTrigger("isCombo");
        //canPlayQueued = false;
        //attackQd = false;
    }

    public void OnCanPlayQueued()
    {
        canPlayQueued = true;
    }

    void TryAttack()
    {
        if (combo == 0)
        {
            attackQd = false;
            Debug.Log("ready yo attack now!");
            StartHighAttackChain();
        }
        else
        {
            attackQd = true;
            Debug.Log($"in the middle of attacking, queueing up the next one.");
        }
    }

    void StartHighAttackChain()
    {
        animator.SetBool("isCombo", false);
        animator.SetBool("HighATKQd", true);
        animator.Play("ATTACK_HIGH_1");
        combo = 1;
    }
}
