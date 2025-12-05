using UnityEngine;
using UnityEngine.Events;

public class Parryable : MonoBehaviour
{
    [Tooltip("When an attack is initially blocked (not yet parried)")] public UnityEvent OnAttackBlocked;
    [Tooltip("When a blocked attack is subsequently parried (reflected)")] public UnityEvent OnAttackParried;

    public enum ObjectType
    {
        EnemyAttack,
        Projectile
    }

    public bool wasBlocked = false; // for initial block of attack
    public bool wasParried = false; // for "overcommit" parry of already blocked attack

    public void OnBlock()
    {
        OnAttackBlocked?.Invoke();
        wasBlocked = true;

        Debug.Log("Attack Blocked on " + gameObject.name);
    }
    public void OnParry()
    {
        OnAttackParried?.Invoke();
    }
}
