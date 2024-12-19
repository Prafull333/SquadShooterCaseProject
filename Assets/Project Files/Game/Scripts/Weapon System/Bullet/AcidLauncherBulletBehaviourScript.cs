using UnityEngine;
using System.Collections;

namespace Watermelon.SquadShooter
{

  public class AcidLauncherBulletBehaviourScript : PlayerBulletBehavior
    {
/*        private static readonly int PARTICLE_HIT_HASH = "AcidLauncher Hit".GetHashCode();
        private static readonly int PARTICLE_WAll_HIT_HASH = "AcidLauncher Wall Hit".GetHashCode();

        [SerializeField] TrailRenderer trailRenderer;*/


        [SerializeField] private GameObject acidPoolPrefab;
        [SerializeField] private float acidPoolDuration;
        [SerializeField] private float acidPoolDamagePerSecond;

        public override void Initialise(float damage, float speed, BaseEnemyBehavior currentTarget, float autoDisableTime, bool autoDisableOnHit = true)
        {
            base.Initialise(damage, speed, currentTarget, autoDisableTime, autoDisableOnHit);

/*            trailRenderer.Clear();*/
        }

        protected override void OnEnemyHitted(BaseEnemyBehavior baseEnemyBehavior)
        {
            // Disable the projectile on hit (override the base behavior)
            gameObject.SetActive(false);

            // Instantiate an acid pool at the hit point
            GameObject acidPool = Instantiate(acidPoolPrefab, transform.position, Quaternion.identity);

            // Set up the acid pool's behavior directly
            Destroy(acidPool, acidPoolDuration); // Destroy the pool after its duration

            Collider[] colliders = Physics.OverlapSphere(transform.position, acidPoolPrefab.transform.localScale.x / 2);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == PhysicsHelper.LAYER_ENEMY)
                {
                    BaseEnemyBehavior enemy = collider.GetComponent<BaseEnemyBehavior>();
                    if (enemy != null && !enemy.IsDead)
                    {
                        StartCoroutine(DealContinuousDamage(enemy, acidPoolDuration, acidPoolDamagePerSecond));
                    }
                }
            }
        }

        protected override void OnObstacleHitted()
        {
            // Same as the base behavior, disable the projectile
            base.OnObstacleHitted();

            // Instantiate an acid pool at the hit point
            GameObject acidPool = Instantiate(acidPoolPrefab, transform.position, Quaternion.identity);

            // Set up the acid pool's behavior directly
            Destroy(acidPool, acidPoolDuration);
        }

        private IEnumerator DealContinuousDamage(BaseEnemyBehavior enemy, float duration, float damagePerSecond)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                enemy.TakeDamage(damagePerSecond * Time.deltaTime, transform.position, transform.forward);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

/*        protected override void OnEnemyHitted(BaseEnemyBehavior baseEnemyBehavior)
        {
            ParticlesController.PlayParticle(PARTICLE_HIT_HASH).SetPosition(transform.position);

            trailRenderer.Clear();
        }

        protected override void OnObstacleHitted()
        {
            base.OnObstacleHitted();

            ParticlesController.PlayParticle(PARTICLE_WAll_HIT_HASH).SetPosition(transform.position);
            trailRenderer.Clear();
        }*/
    }
}

