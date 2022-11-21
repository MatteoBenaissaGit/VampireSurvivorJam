using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData WeaponInfoData;
        public bool CanShoot;
        [ReadOnly] public float ShootCooldown;

        private void Update()
        {
            Cooldown();
        }

        private void Cooldown()
        {
            ShootCooldown -= Time.deltaTime;
            if (ShootCooldown <= 0)
            {
                CanShoot = true;
                return;;
            }
            CanShoot = false;
        }
    }
}