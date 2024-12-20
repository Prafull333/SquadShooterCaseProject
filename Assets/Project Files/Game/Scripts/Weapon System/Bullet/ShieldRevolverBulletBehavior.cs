using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon.SquadShooter;

public class ShieldRevolverBulletBehavior : PlayerBulletBehavior
{
    //public float speed = 10f;
    public float lifetime = 3f;
    //public int damage;

    private Rigidbody rb;

    public void Initialize(int damage)
    {
        this.damage = damage;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    protected override void OnEnemyHitted(BaseEnemyBehavior baseEnemyBehavior)
    {
        throw new System.NotImplementedException();
        // Apply damage to the enemy
        // other.GetComponent<EnemyBehavior>().TakeDamage(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
           // other.GetComponent<EnemyBehavior>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
