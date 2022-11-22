using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData WeaponInfoData;
        public bool CanShoot;
        [ReadOnly] public float ShootCooldown;
        [ReadOnly] public float DurabilityTimer;

        private void Update()
        {
            Cooldown();
        }

        private void Cooldown()
        {
            //shoot
            ShootCooldown -= Time.deltaTime;
            if (ShootCooldown <= 0)
            {
                CanShoot = true;
                return;;
            }
            CanShoot = false;
            
            //durability
            if (DurabilityTimer <= WeaponInfoData.DurabilitySeconds)
            {
                DurabilityTimer += Time.deltaTime;
            }
        }
    }
}