using System;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _iconImage;

    [Header("Multipliers")]
    private const float _attackMultiplier = 1.1f;
    private const float _cooldownMultiplier = 0.9f;
    private const float _durabilityMultiplier = 1.1f;
    private const float _speedMultiplier = 1.1f;
    private const float _playerAttackMultiplier = 1.1f;

    private Vector3 _baseScale;
    
    //to set
    [HideInInspector] public Sprite Icon;
    [HideInInspector] public string Text;
    [HideInInspector] public UpgradeType Upgrade;
    [HideInInspector] public WeaponUpgradeType WeaponUpgrade;
    [HideInInspector] public Weapon Weapon;
    [HideInInspector] public PlayerUpgradeType PlayerUpgrade;

    public void Set()
    {
        _iconImage.sprite = Icon;
        _text.text = Text;
        _baseScale = transform.localScale;
    }

    private void OnMouseOver()
    {
        transform.DOKill();
        transform.DOScale(_baseScale + Vector3.one*0.5f, 0.2f);
    }

    private void OnMouseExit()
    {
        transform.DOKill();
        transform.DOScale(_baseScale, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ApplyUpgrade();
        GameManager.GameManagerInstance.HideSlots();
    }

    private void ApplyUpgrade()
    {
        switch (Upgrade)
        {
            case UpgradeType.Weapon:
                switch (WeaponUpgrade)
                {
                    case WeaponUpgradeType.Damage:
                        Weapon.WeaponInfo.Damage *= _attackMultiplier;
                        break;
                    case WeaponUpgradeType.Cooldown:
                        Weapon.WeaponInfo.Cooldown *= _cooldownMultiplier;
                        break;
                    case WeaponUpgradeType.Durability:
                        Weapon.WeaponInfo.DurabilitySeconds *= _durabilityMultiplier;
                        break;
                }
                break;
            case UpgradeType.Player:
                switch (PlayerUpgrade)
                {
                    case PlayerUpgradeType.Speed:
                        PlayerController.PlayerControllerInstance.Speed *= _speedMultiplier;
                        break;
                    case PlayerUpgradeType.Damage:
                        PlayerController.PlayerControllerInstance.AttackDamage *= _playerAttackMultiplier;
                        break;
                }
                break;
        }
    }
}