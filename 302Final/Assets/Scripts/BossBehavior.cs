using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// coroutine for boss attack patterns
// have two main loops: one for initial phase, one for 50% health phase
// first phase is slower, only projectile attacks | second phase is faster, melee attacks added in

public class BossBehavior : MonoBehaviour, IDamageableObj
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public AttackType attackType;
        public float attackInterval;

        public GameObject projectilePrefab;
    }
    public enum AttackType
    {
        DoubleProjectile,
        SimpleProjectile,
        ExplosiveProjectile,
        ThrownProjectile,
        MeleeCharge,
        DelayedMelee,
        MultiHitMelee,
        MonsoonMelee
    }

    // basic settings
    public List<AttackPattern> PhaseOneAttacks;
    public List<AttackPattern> PhaseTwoAttacks;

    public float attackSwitchDelay = 2f;

    // current attack state
    AttackPattern currentAttack;
    public bool isAttacking = false;

    private Coroutine bossAttackLoop;

    private Transform playerTransform;

    [Header("health shit")]
    public int maxHealth = 100;
    public int currentHealth;
    public UnityEvent<int, int> OnTakeDamageEvent;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnTakeDamageEvent?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("uou won wowww!!!");
        }
    }

    public enum BossPhase
    {
        PhaseOne,
        PhaseTwo
    }
    public BossPhase currentPhase = BossPhase.PhaseOne;

    void Start()
    {
        currentHealth = maxHealth;

        playerTransform = FindFirstObjectByType<PlayerCharacter>().transform ;

        StartCoroutine(BossAttackPattern());
    }

    IEnumerator BossAttackPattern()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isAttacking);

            currentAttack = AttackSelector();
            yield return StartCoroutine(ExecuteAttack(currentAttack));

            yield return new WaitForSeconds(attackSwitchDelay);
        }
    }

    AttackPattern previousAttack;
    AttackPattern AttackSelector()
    {
        if (currentPhase == BossPhase.PhaseOne)
        {
            int index = Random.Range(0, PhaseOneAttacks.Count);

            if (previousAttack != null)
            {
                while (PhaseOneAttacks[index] == previousAttack) // make sure not to repeat same attack
                {
                    index = Random.Range(0, PhaseOneAttacks.Count);
                }
            }

            previousAttack = PhaseOneAttacks[index];
            return PhaseOneAttacks[index];
        }
        else
        {
            int index = Random.Range(0, PhaseTwoAttacks.Count);

            if (previousAttack != null)
            {
                while (PhaseTwoAttacks[index] == previousAttack) // make sure not to repeat same attack
                {
                    index = Random.Range(0, PhaseTwoAttacks.Count);
                }
            }

            previousAttack = PhaseTwoAttacks[index];
            return PhaseTwoAttacks[index];
        }
    } // select attack based on phase

    IEnumerator ExecuteAttack(AttackPattern attack)
    {
        isAttacking = true;

        switch (attack.attackType) // MAKE SURE TO CHANGE COROUTINE TO MATCH ATTACK TYPE!!! Windup is just a placeholder
        {
            case AttackType.DoubleProjectile:
                yield return StartCoroutine(DoubleProjectileCoroutine());
                // implement double projectile attack logic here
                Debug.Log("Executing Double Projectile Attack");
                break;
            case AttackType.SimpleProjectile:
                yield return StartCoroutine(AttackWindup(attack));
                // implement simple projectile attack logic here
                Debug.Log("Executing Simple Projectile Attack");
                break;
            case AttackType.ExplosiveProjectile:
                yield return StartCoroutine(AttackWindup(attack));
                // implement explosive projectile attack logic here
                Debug.Log("Executing Explosive Projectile Attack");
                break;
            case AttackType.ThrownProjectile:
                yield return StartCoroutine(AttackWindup(attack));
                // implement thrown projectile attack logic here
                Debug.Log("Executing Thrown Projectile Attack");
                break;
            case AttackType.MeleeCharge:
                yield return StartCoroutine(AttackWindup(attack));
                // implement melee charge attack logic here
                Debug.Log("Executing Melee Charge Attack");
                break;
            case AttackType.DelayedMelee:
                yield return StartCoroutine(AttackWindup(attack));
                // implement delayed melee attack logic here
                Debug.Log("Executing Delayed Melee Attack");
                break;
            case AttackType.MultiHitMelee:
                yield return StartCoroutine(AttackWindup(attack));
                // implement multi-hit melee attack logic here
                Debug.Log("Executing Multi-Hit Melee Attack");
                break;
            case AttackType.MonsoonMelee:
                yield return StartCoroutine(AttackWindup(attack));
                // implement monsoon melee attack logic here
                Debug.Log("Executing Monsoon Melee Attack");
                break;
        }

        // cooldown based on chosen attack
        isAttacking = false;
    }

    IEnumerator AttackWindup(AttackPattern attack)
    {
        // indicate windup with some visual cue (color flashing, some radial ui thing)
        yield return null;
    }

    void SpawnProjectile(Vector3 position, AttackPattern attack)
    {
        Vector3 target = playerTransform.position;
        target.y = position.y; // keep projectile level with spawn position

        // set projectile direction towards player
        Vector3 direction = (target - position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject projectile = Instantiate(attack.projectilePrefab, position, rotation);
        ProjectileBehavior projBehavior = projectile.GetComponent<ProjectileBehavior>();

        if (projBehavior != null)
        {
            projBehavior.direction = Vector3.forward;
        }
    }

    IEnumerator DoubleProjectileCoroutine()
    {
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0)); // isometric conversion matrix

        // randomly choose left or right side to spawn first
        int side = Random.Range(0, 2); // 0 = left, 1 = right

        Vector3 spawnPos = playerTransform.position;
        spawnPos.y = 1f;
        spawnPos.z += 2f; // offset forward
        
        if (side == 0)
            spawnPos.x -= 5f; // offset to the left
        else
            spawnPos.x += 5f; // offset to the right

        var isoSpawn = matrix.MultiplyPoint3x4(spawnPos); // convert to isometric space

        SpawnProjectile(isoSpawn, currentAttack);

        yield return new WaitForSeconds(1.5f);

        spawnPos = playerTransform.position;
        spawnPos.y = 1;
        spawnPos.z += 2f; // offset forward

        if (side == 0)
            spawnPos.x += 5f; // offset to the right
        else
            spawnPos.x -= 5f; // offset to the left

        isoSpawn = matrix.MultiplyPoint3x4(spawnPos); // convert to isometric space

        SpawnProjectile(isoSpawn, currentAttack);

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.GetComponentInParent<ProjectileBehavior>() != null)
        {
            var proj = other.GetComponentInParent<ProjectileBehavior>();
            if (proj.currentState == ProjectileBehavior.ObjectState.Parried)
            {
                TakeDamage(proj.damageToDeal);
            }
        }
    }
}
