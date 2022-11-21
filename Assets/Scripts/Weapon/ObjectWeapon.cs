using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    
    public class ObjectWeapon : MonoBehaviour
    {
        public WeaponData WeaponData;
        [SerializeField] private TextMeshProUGUI _costText;

        private void Start()
        {
            _costText.text = WeaponData.MoneyCost + "$";
        }

        public bool CanPickUp()
        {
            return PlayerController.PlayerControllerInstance.CurrentMoney < WeaponData.MoneyCost == false;
        }
    }
}