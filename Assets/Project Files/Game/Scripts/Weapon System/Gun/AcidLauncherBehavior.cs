//using UnityEngine;
//using Watermelon;
////using Watermelon.SquadShooter;
//using Watermelon.Upgrades;


//namespace Watermelon.SquadShooter
//{
//    public class AcidLauncherBehavior : BaseGunBehavior
//    {
//        [LineSpacer]
//        [SerializeField] LayerMask targetLayers = (1 << 9) | (1 << 8);
//        [SerializeField] float bulletDisableTime = 5.0f;

//        // Gun variables
//        private float spread;
//        private float attackDelay;
//        private DuoFloat bulletSpeed;

//        // Shooting cooldown
//        private float nextShootTime;

//        // Bullet pool (based on prefab from Upgrade)
//        private Pool bulletPool;

//        // Gun upgrade
//        private AcidLauncherUpgrade upgrade;

//        public override void Initialise(CharacterBehaviour characterBehaviour, WeaponData data)
//        {
//            base.Initialise(characterBehaviour, data);

//            // Get upgrade from database
//            upgrade = UpgradesController.GetUpgrade<AcidLauncherUpgrade>(data.UpgradeType);

//            // Get weapon's current upgrade stage
//            BaseWeaponUpgradeStage currentStage = upgrade.CurrentStage as BaseWeaponUpgradeStage;

//            // Create pool object from bullet prefab
//           // bulletPool = new Pool(new PoolSettings(currentStage.BulletPrefab.name, currentStage.BulletPrefab, 5, true));

//            // Recalculate gun variables
//            RecalculateDamage();
//        }

//        public override void RecalculateDamage()
//        {
//            // Get weapon's current upgrade stage
//            BaseWeaponUpgradeStage stage = upgrade.GetCurrentStage();

//            damage = stage.Damage;
//            attackDelay = 1f / stage.FireRate;
//            spread = stage.Spread;
//            bulletSpeed = stage.BulletSpeed;
//        }

//        public override void GunUpdate()
//        {
//            // Check if any enemy is in shooting range
//            if (!characterBehaviour.IsCloseEnemyFound) return;

//            // Check if shooting cooldown is finished
//            if (nextShootTime >= Time.timeSinceLevelLoad) return;

//            // Get direction of the closest enemy
//            Vector3 shootDirection = characterBehaviour.ClosestEnemyBehaviour.transform.position.SetY(shootPoint.position.y) - shootPoint.position;

//            // Check with raycast if the target is physically reachable and layer of the target is "Enemy"
//            if (Physics.Raycast(transform.position, shootDirection, out var hitInfo, 300f, targetLayers) && hitInfo.collider.gameObject.layer == PhysicsHelper.LAYER_ENEMY)
//            {
//                // Check if a character is looking in the target's direction
//                if (Vector3.Angle(shootDirection, transform.forward.SetY(0f)) < 40f)
//                {
//                    // Activate the highlight circle under the target 
//                    characterBehaviour.SetTargetActive();

//                    // Set next shooting cooldown
//                    nextShootTime = Time.timeSinceLevelLoad + attackDelay;

//                    // Get bullet object from the pool
//                    GameObject bulletObject = bulletPool.GetPooledObject();
//                    bulletObject.transform.position = shootPoint.position;
//                    bulletObject.transform.eulerAngles = characterBehaviour.transform.eulerAngles + Vector3.up * Random.Range((float)-spread, spread);

//                    // Get bullet component and initialise its logic
//                    PlayerBulletBehavior bullet = bulletObject.GetComponent<PlayerBulletBehavior>();
//                    bullet.Initialise(damage.Random() * characterBehaviour.Stats.BulletDamageMultiplier, bulletSpeed.Random(), characterBehaviour.ClosestEnemyBehaviour, bulletDisableTime);

//                    // Invoke the OnGunShooted method to let the character know when to play shooting animation 
//                    characterBehaviour.OnGunShooted();

//                    // Here you can add extra code such as particles, sounds, etc..
//                    // AudioController.PlaySound(audioClipVariable);
//                    // particleVariable.Play();
//                }
//            }
//            else
//            {
//                characterBehaviour.SetTargetUnreachable();
//            }
//        }

//        /// <summary>
//        /// This method is called when the player unselects the gun.
//        /// Use it to reset variables before the prefab is destroyed.
//        /// </summary>
//        public override void OnGunUnloaded()
//        {
//            // Destroy bullets pool
//            if (bulletPool != null)
//            {
//                bulletPool.Clear();
//                bulletPool = null;
//            }
//        }

//        /// <summary>
//        /// This method is called when player selects gun.
//        /// Here you should change parent of the gun prefab and place it to correct position.
//        /// For default guns we added Transforms to BaseCharacterGraphics script to easily modify position of gun in the editor.
//        /// </summary>
//        public override void PlaceGun(BaseCharacterGraphics characterGraphics)
//        {
//            // Use transform variable inside of BaseCharacterGraphics script
//            transform.SetParent(characterGraphics.MinigunHolderTransform);
//            transform.ResetLocal();

//            // OR
//            // Use parent character object and just apply offset to the gun
//            // transform.SetParent(characterBehaviour.transform);
//            // transform.localPosition = new Vector3(1.36f, 4.67f, 2.5f);
//        }

//        /// <summary>
//        /// This method is called when the character enters a room or revives.
//        /// Here you can return the gun to the default state or reset some variables.
//        /// </summary>
//        public override void Reload()
//        {
//            bulletPool.ReturnToPoolEverything();
//        }
//    }
//}


using System.Collections.Generic;
using UnityEngine;
using Watermelon.Upgrades;

namespace Watermelon.SquadShooter
{
    public class AcidLauncherBehavior : BaseGunBehavior
    {
        [LineSpacer]
        [SerializeField] Transform barrelTransform;
        [SerializeField] ParticleSystem shootParticleSystem;

        [SerializeField] LayerMask targetLayers;
        [SerializeField] float bulletDisableTime;

        [Space]
        [SerializeField] float fireRotationSpeed;

        [Space]
        [SerializeField] List<float> bulletStreamAngles;

        private float spread;
        private float attackDelay;
        private DuoFloat bulletSpeed;

        private float nextShootTime;
        private float lastShootTime;

        private Pool bulletPool;

        private Vector3 shootDirection;

        private AcidLauncherUpgrade upgrade;

        private TweenCase shootTweenCase;

        public override void Initialise(CharacterBehaviour characterBehaviour, WeaponData data)
        {
            base.Initialise(characterBehaviour, data);

            upgrade = UpgradesController.GetUpgrade<AcidLauncherUpgrade>(data.UpgradeType);

            GameObject bulletObj = (upgrade.CurrentStage as BaseWeaponUpgradeStage).BulletPrefab;
            bulletPool = new Pool(bulletObj, bulletObj.name);

            RecalculateDamage();
        }

        private void OnDestroy()
        {
            if (bulletPool != null)
                PoolManager.DestroyPool(bulletPool);
        }

        public override void OnLevelLoaded()
        {
            RecalculateDamage();
        }

        public override void RecalculateDamage()
        {
            var stage = upgrade.GetCurrentStage();

            damage = stage.Damage;
            attackDelay = 1f / stage.FireRate;
            spread = stage.Spread;
            bulletSpeed = stage.BulletSpeed;
        }

        public override void GunUpdate()
        {
            if (attackDelay > 0.2f)
                AttackButtonBehavior.SetReloadFill(1 - (Time.timeSinceLevelLoad - lastShootTime) / (nextShootTime - lastShootTime));

            if (!characterBehaviour.IsCloseEnemyFound)
                return;

            barrelTransform.Rotate(Vector3.forward * fireRotationSpeed);

            if (nextShootTime >= Time.timeSinceLevelLoad || !characterBehaviour.IsAttackingAllowed)
            {
                return;
            }

            AttackButtonBehavior.SetReloadFill(0);

            shootDirection = characterBehaviour.ClosestEnemyBehaviour.transform.position.SetY(shootPoint.position.y) - shootPoint.position;

            if (Physics.Raycast(transform.position, shootDirection, out var hitInfo, 300f, targetLayers) && hitInfo.collider.gameObject.layer == PhysicsHelper.LAYER_ENEMY)
            {
                if (Vector3.Angle(shootDirection, transform.forward.SetY(0f)) < 40f)
                {
                    shootTweenCase.KillActive();

                    shootTweenCase = transform.DOLocalMoveZ(-0.0825f, attackDelay * 0.3f).OnComplete(delegate
                    {
                        shootTweenCase = transform.DOLocalMoveZ(0, attackDelay * 0.6f);
                    });

                    characterBehaviour.SetTargetActive();

                    shootParticleSystem.Play();

                    nextShootTime = Time.timeSinceLevelLoad + attackDelay;
                    lastShootTime = Time.timeSinceLevelLoad;

                    if (bulletStreamAngles.IsNullOrEmpty())
                    {
                        bulletStreamAngles = new List<float> { 0 };
                    }

                    int bulletsNumber = upgrade.GetCurrentStage().BulletsPerShot.Random();

                    for (int k = 0; k < bulletsNumber; k++)
                    {
                        for (int i = 0; i < bulletStreamAngles.Count; i++)
                        {
                            var streamAngle = bulletStreamAngles[i];

                            PlayerBulletBehavior bullet = bulletPool
                                .GetPooledObject()
                                .SetPosition(shootPoint.position)
                                .SetEulerAngles(characterBehaviour.transform.eulerAngles + Vector3.up * (Random.Range((float)-spread, spread) + streamAngle))
                                .GetComponent<PlayerBulletBehavior>();
                            bullet.Initialise(damage.Random() * characterBehaviour.Stats.BulletDamageMultiplier, bulletSpeed.Random(), characterBehaviour.ClosestEnemyBehaviour, bulletDisableTime);
                        }
                    }

                    characterBehaviour.OnGunShooted();

                    AudioController.PlaySound(AudioController.AudioClips.shotMinigun);
                }
            }
            else
            {
                characterBehaviour.SetTargetUnreachable();
            }
        }

        public override void OnGunUnloaded()
        {
            if (bulletPool != null)
            {
                PoolManager.DestroyPool(bulletPool);

                bulletPool = null;
            }
        }

        public override void PlaceGun(BaseCharacterGraphics characterGraphics)
        {
            transform.SetParent(characterGraphics.MinigunHolderTransform);
            transform.ResetLocal();
        }

        public override void Reload()
        {
            bulletPool.ReturnToPoolEverything();
        }
    }
}

