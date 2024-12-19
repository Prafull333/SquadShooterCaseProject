using UnityEngine;
using Watermelon;

namespace Watermelon.SquadShooter
{
    public class FrostWeaponBehavior : BaseGunBehavior
    {
        [Header("Frost Weapon Properties")]
        public float attackRate = 1.0f; // Shots per second
        public float lightIceDamage = 10.0f;
        public float freezeDuration = 0.5f;
        public float bulletSpeed = 10f; // Adjust bullet speed as needed
        public float slowMultiplier = 0.5f; // Adjust slow multiplier as needed

        private float nextFireTime = 0.0f;

        private void Awake()
        {
            damage = new DuoInt((int)lightIceDamage, (int)lightIceDamage); // Set base damage
        }

        public override void GunUpdate()
        {
            if (Time.time >= nextFireTime && CanFire())
            {
                Fire();
                nextFireTime = Time.time + 1.0f / attackRate;
            }
        }

        public override void OnGunUnloaded()
        {
            // Handle weapon-specific logic when unloaded (optional)
        }

        private bool CanFire()
        {
            // Implement logic to check if weapon can fire (e.g., ammo check, cooldown)
            return true; // Replace with your firing logic
        }

        public override void PlaceGun(BaseCharacterGraphics characterGraphics)
        {
            transform.SetParent(characterGraphics.ShootGunHolderTransform);
            transform.ResetLocal();
        }

        public override void RecalculateDamage()
        {
            // ... (your existing damage recalculation logic)
        }

        private void Fire()
        {
            // Instantiate projectile or handle firing logic
            var projectile = Instantiate(Resources.Load<GameObject>("FreezingBullet"), shootPoint.position, shootPoint.rotation);
           // projectile.GetComponent<FreezingBulletBehavior>().Initialize(lightIceDamage, bulletSpeed, currentTarget, freezeDuration, slowMultiplier);
        }

        public void OnTargetHit(GameObject target, int slowStep)
        {
            // Check if target has a suitable component (modify based on your implementation)
            var targetable = target.GetComponent<BaseEnemyBehavior>();

            if (targetable != null)
            {
                // Calculate slow multiplier based on slowStep (if needed)
                // float slowMultiplier = (slowStep == 1) ? slowStep1Multiplier : slowStep2Multiplier;

                // Apply slow down effect to the target using the interface method
                targetable.ApplySlowdown(slowMultiplier, freezeDuration);
            }
        }

        public override void Reload()
        {
            throw new System.NotImplementedException();
        }
    }
}