using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite PlayerWeaponSprite;
        public Damager Projectile;
        public float Cooldown;
        public float MoneyCost;
    }
}