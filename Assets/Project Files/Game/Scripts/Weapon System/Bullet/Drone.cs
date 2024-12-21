using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon.SquadShooter;

public class Drone : MonoBehaviour
{
    private CharacterBehaviour characterBehaviour;
    private float lifetime;

    public float nextAttackTime = 5f;
    public float damage = 10f;
    public int attackCooldown = 1;
    public int attackRange = 10;

    public void Initialize(CharacterBehaviour characterBehaviour, float lifetime)
    {
        this.characterBehaviour = characterBehaviour;
        this.lifetime = lifetime;
    }

    public bool Update(BaseEnemyBehavior targetEnemy)
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f || targetEnemy == null || targetEnemy.IsDead)
        {
            return true; // Destroy the drone
        }

        // Move the drone towards the target enemy
        transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange && Time.time >= nextAttackTime)
        {
            AttackEnemy(targetEnemy);
            nextAttackTime = Time.time + attackCooldown;
        }

        return false;
    }


    private void AttackEnemy(BaseEnemyBehavior enemy)
    {
        //enemy.TakeDamage(characterBehaviour.WeaponData.Damage);

        // Deal damage to enemy
        enemy.TakeDamage(CharacterBehaviour.NoDamage ? 0 : damage, transform.position, transform.forward);

        // Call hit callback
        enemy.Stun(0.1f);
    }

}
