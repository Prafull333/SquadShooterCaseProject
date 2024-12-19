using UnityEngine;

namespace Watermelon.SquadShooter
{
    public class FreezingBulletBehavior : PlayerBulletBehavior
    {
        [SerializeField] private TrailRenderer trailRenderer;

        private float freezeDuration;
        private float slowMultiplier;

        public override void Initialise(float damage, float speed, BaseEnemyBehavior currentTarget, float autoDisableTime, bool autoDisableOnHit = true)
        {
            base.Initialise(damage, speed, currentTarget, autoDisableTime, autoDisableOnHit);

            this.freezeDuration = freezeDuration;
            this.slowMultiplier = slowMultiplier;

            trailRenderer.Clear(); // Clear the trail renderer on initialization
        }

        protected override void OnEnemyHitted(BaseEnemyBehavior baseEnemyBehavior)
        {
            // Apply slow effect
            baseEnemyBehavior.ApplySlowdown(slowMultiplier, freezeDuration);

            // Add visual effect (e.g., particle effect)
            // ... (replace with your desired visual effect)

            trailRenderer.Clear(); // Clear the trail renderer on hit
        }

        protected override void OnObstacleHitted()
        {
            base.OnObstacleHitted();

            // Add visual effect for wall hit (e.g., particle effect)
            // ... (replace with your desired visual effect)

            trailRenderer.Clear(); // Clear the trail renderer on wall hit
        }
    }
}