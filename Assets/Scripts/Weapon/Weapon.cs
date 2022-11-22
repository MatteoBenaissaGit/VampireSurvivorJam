using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData WeaponDataReference;
        public WeaponData WeaponInfo;
        public bool CanShoot;
        [ReadOnly] public float ShootCooldown;
        [ReadOnly] public float DurabilityTimer;

        public void Set()
        {
            WeaponInfo = ScriptableObject.CreateInstance<WeaponData>();
            
            WeaponInfo.Id = WeaponDataReference.Id;
            WeaponInfo.DisplayName = WeaponDataReference.DisplayName;
            WeaponInfo.PlayerWeaponSprite = WeaponDataReference.PlayerWeaponSprite;
            WeaponInfo.PlayerWeaponIcon = WeaponDataReference.PlayerWeaponIcon;
            WeaponInfo.Projectile = WeaponDataReference.Projectile;
            WeaponInfo.Cooldown = WeaponDataReference.Cooldown;
            WeaponInfo.MoneyCost = WeaponDataReference.MoneyCost;
            WeaponInfo.Damage = WeaponDataReference.Damage;
            WeaponInfo.DurabilitySeconds = WeaponDataReference.DurabilitySeconds;
        }

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
                return;
            }

            CanShoot = false;

            //durability
            if (DurabilityTimer <= WeaponInfo.DurabilitySeconds)
            {
                DurabilityTimer += Time.deltaTime;
            }
        }
    }
}