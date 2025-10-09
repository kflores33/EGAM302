using System.Collections.Generic;
using TMPro;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyScriptable enemyData;

    float maxHP;
    float currentHP;
    List<CharacterBehavior> damageSources = new List<CharacterBehavior>();

    private void Start()
    {
        maxHP = enemyData.enemy_HP;
        currentHP = maxHP;
    }

    private void Update()
    {
        if (currentHP <= 0)
        {
            OnDeath();
        }
    }

    public void TakeDamage(float damage, CharacterBehavior damageSource)
    {
        currentHP -= damage;
        Debug.Log($"Dealt {damage} damge to {this.name}!");

        GeneralUIHandler.instance.SpawnDamagePopup(transform.position, damage);

        if (!damageSources.Contains(damageSource))
        {
            damageSources.Add(damageSource);
        }
    }
    void OnDeath()
    {
        GameManager.instance.DeregisterActiveEnemy(this);
        foreach (CharacterBehavior damageSource in damageSources)
        {
            foreach (WeaponBehavior weapon in damageSource.heldWeapons)
            {
                PlayerInvManager.instance.RegisterKill(weapon.weaponData.weapon_id);
            }
            damageSource.DeregisterTarget(this);
        }

        Debug.Log($"Defeated {this.name}!");
        Destroy(gameObject);
    }
}
