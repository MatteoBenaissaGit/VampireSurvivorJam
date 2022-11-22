using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite PlayerWeaponSprite;
        public Sprite PlayerWeaponIcon;
        public Damager Projectile;
        public float Cooldown;
        public int MoneyCost;
        public float Damage;
        public float DurabilitySeconds;
    }
}