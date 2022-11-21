using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _experienceBar;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private Damageable _playerDamageable;

    private PlayerController _instance;

    private void Start()
    {
        _instance = PlayerController.PlayerControllerInstance;
        
        _lifeBar.fillAmount = 1;
        _experienceBar.fillAmount = 0;
        _moneyText.text = "0$";
        _levelText.text = "Level 0";
        
        _playerDamageable = GetComponent<Damageable>();
        
        _playerDamageable.OnLifeChange.AddListener(UpdateLifeUI);
        _instance.OnExperienceChange.AddListener(UpdateExperienceUI);
        _instance.OnLevelUp.AddListener(LevelUpUI);
        _instance.OnMoneyChange.AddListener(UpdateMoneyUI);
    }

    private void OnDestroy()
    {
        _playerDamageable.OnLifeChange.RemoveListener(UpdateLifeUI);
        _instance.OnExperienceChange.RemoveListener(UpdateExperienceUI);
        _instance.OnLevelUp.RemoveListener(LevelUpUI);
        _instance.OnMoneyChange.RemoveListener(UpdateMoneyUI);

    }

    private void UpdateLifeUI()
    {
        float fillAmount = PlayerController.PlayerControllerInstance.CurrentLife / PlayerController.PlayerControllerInstance.Life;
        _lifeBar.DOFillAmount(fillAmount,0.3f);
    }

    private void UpdateExperienceUI()
    {
        float fillAmount = (float)_instance.CurrentExperience / (float)_instance.ExperienceNecessary;
        _experienceBar.DOFillAmount(fillAmount, 0.3f);
        _levelText.text = "Level " + _instance.CurrentLevel;
    }

    private void LevelUpUI()
    {
        _experienceBar.DOFillAmount(1, 0.5f).OnComplete(UpdateExperienceUI);
        //effect
    }

    private void UpdateMoneyUI()
    {
        _moneyText.text = _instance.CurrentMoney + "$";
    }
}
