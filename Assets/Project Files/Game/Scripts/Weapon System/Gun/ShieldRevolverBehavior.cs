using UnityEngine;
using System.Collections;
using Watermelon;

namespace Watermelon.SquadShooter
{
    public class ShieldRevolverBehavior : BaseGunBehavior
    {
        [SerializeField] private GameObject shieldPrefab;
        [SerializeField] private float shieldRaiseTime = 0.5f;
        [SerializeField] private float shieldLowerTime = 0.25f;
        [SerializeField] private float revolverFireRate = 1.0f; // Shots per second

        private GameObject shieldInstance;
        private bool isShieldRaised;
        private float lastShotTime;
        private float bulletSpreadAngle;
        private float attackDelay;
        private float bulletSpeed;


        public ShieldAndRevolverUpdrage upgrade;
        public GameObject projectilePrefab;
        public bool isReloading;
        public int currentAmmo;
        public int maxAmmo = 10;
        public float reloadTime = 1.0f;


        public override void Initialise(CharacterBehaviour characterBehaviour, WeaponData data)
        {
            base.Initialise(characterBehaviour, data);
            shieldInstance = null;
            isShieldRaised = false;
            lastShotTime = Time.time;
        }

        public override void GunUpdate()
        {
            if (characterBehaviour.IsActive)
            {
                LowerShield();
                TryFireRevolver();
            }
            else
            {
                RaiseShield();
            }
        }

        private void RaiseShield()
        {
            if (!isShieldRaised)
            {
                if (shieldPrefab != null)
                {
                    shieldInstance = Instantiate(shieldPrefab, transform.position, transform.rotation);
                    shieldInstance.transform.SetParent(transform);
                    isShieldRaised = true;
                    StartCoroutine(RaiseShieldCoroutine());
                }
            }
        }

        private void LowerShield()
        {
            if (isShieldRaised)
            {
                if (shieldInstance != null)
                {
                    Destroy(shieldInstance, shieldLowerTime);
                    isShieldRaised = false;
                    StartCoroutine(LowerShieldCoroutine());
                }
            }
        }

        private IEnumerator RaiseShieldCoroutine()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < shieldRaiseTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator LowerShieldCoroutine()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < shieldLowerTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private void TryFireRevolver()
        {
            if (Time.time - lastShotTime >= 1.0f / revolverFireRate && currentAmmo > 0)
            {
                // RecalculateDamage
                var stage = upgrade.GetCurrentStage();
                damage = stage.Damage;
                bulletSpreadAngle = stage.Spread;
                attackDelay = 1f / stage.FireRate;
                bulletSpeed = stage.BulletSpeed.firstValue;

                // Instantiate a projectile
                GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
                ShieldRevolverBulletBehavior projectileScript = projectile.GetComponent<ShieldRevolverBulletBehavior>();
                projectileScript.Initialize(damage.firstValue);

                currentAmmo--;
                lastShotTime = Time.time;
            }
        }

        public override void Reload()
        {
            if (currentAmmo < maxAmmo && !isReloading)
            {
                isReloading = true;
                StartCoroutine(ReloadCoroutine());
            }
        }

        private IEnumerator ReloadCoroutine()
        {
            // Play reload animation and sound effects
            // Play reload sound effect

            yield return new WaitForSeconds(reloadTime);

            currentAmmo = maxAmmo;
            isReloading = false;
        }

        public override void OnGunUnloaded()
        {
            // Play empty click sound effect
            // Trigger out-of-ammo animation or visual effect
        }

        public override void PlaceGun(BaseCharacterGraphics characterGraphics)
        {
            transform.SetParent(characterGraphics.ShieldRevolverHolderTransform);
            transform.ResetLocal();
        }

        public override void RecalculateDamage()
        {
           
        }
    }
}