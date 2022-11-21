using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;


public class PlayerUI : MonoBehaviour
{
    
    [SerializeField] private Image _LifeBar;

    private Damageable _playerDamageable;

    private void Start()
    {
        _LifeBar.fillAmount = 1;
        _playerDamageable = GetComponent<Damageable>();
        _playerDamageable.OnLifeChange.AddListener(UpdateUI);
    }

    private void OnDestroy()
    {
        _playerDamageable.OnLifeChange.RemoveListener(UpdateUI);
    }

    public void UpdateUI()
    {
        float fillAmount = PlayerController.PlayerControllerInstance.CurrentLife / PlayerController.PlayerControllerInstance.Life;
        _LifeBar.DOFillAmount(fillAmount,0.3f);
    }
}
