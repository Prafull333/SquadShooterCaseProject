using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Watermelon.Upgrades;

namespace Watermelon.SquadShooter
{
    public class DroneBehavior : BaseGunBehavior, IDroneBehavior
    {
        [SerializeField] GameObject dronePrefab; // Prefab of the drone to spawn
        [SerializeField] int maxDroneCount = 3; // Maximum number of drones that can be spawned
        [SerializeField] float droneSpawnInterval = 1f; // Interval between spawning drones (in seconds)
        [SerializeField] float droneLifetime = 10f; // How long each drone will last (in seconds)

        private float nextDroneSpawnTime;
        private List<Drone> activeDrones = new List<Drone>();

        private DronesUpgrade upgrade;

        public override void Initialise(CharacterBehaviour characterBehaviour, WeaponData data)
        {
            base.Initialise(characterBehaviour, data);

            upgrade = UpgradesController.GetUpgrade<DronesUpgrade>(data.UpgradeType);
        }

        public override void GunUpdate()
        {
            if (Time.time >= nextDroneSpawnTime && activeDrones.Count < maxDroneCount)
            {
                SpawnDrone();
                nextDroneSpawnTime = Time.time + droneSpawnInterval;
            }

            // Update and manage active drones
            for (int i = activeDrones.Count - 1; i >= 0; i--)
            {
                Drone drone = activeDrones[i];
                if (drone.Update(characterBehaviour.ClosestEnemyBehaviour))
                {
                    activeDrones.RemoveAt(i);
                    Destroy(drone.gameObject);
                }
            }
        }

        private void SpawnDrone()
        {
            if (dronePrefab == null)
            {
                Debug.LogError("DroneBehavior: dronePrefab is not assigned!");
                return;
            }

            GameObject droneObject = Instantiate(dronePrefab, transform.position, transform.rotation);
            Drone drone = droneObject.GetComponent<Drone>();
            if (drone == null)
            {
                Debug.LogError("DroneBehavior: dronePrefab does not have a Drone component!");
                Destroy(droneObject);
                return;
            }

            drone.Initialize(characterBehaviour, droneLifetime);
            activeDrones.Add(drone);
        }

        public override void Reload()
        {
            throw new System.NotImplementedException();
        }

        public override void OnGunUnloaded()
        {
            throw new System.NotImplementedException();
        }

        public override void PlaceGun(BaseCharacterGraphics characterGraphics)
        {
            throw new System.NotImplementedException();
        }

        public override void RecalculateDamage()
        {
            throw new System.NotImplementedException();
        }
    }
}
