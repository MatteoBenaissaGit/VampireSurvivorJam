using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;
using DefaultNamespace;
using DG.Tweening;
using MoreMountains.Feedbacks;
using TMPro;
using Random = UnityEngine.Random;


public class PlayerUI : MonoBehaviour
{
    
    [SerializeField] private Image _lifeBar;
    [SerializeField] private Image _experienceBar;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyText;

    [SerializeField] private WeaponDurabilityBar _firstWeaponDurabilityBar;
    [SerializeField] private WeaponDurabilityBar _secondWeaponDurabilityBar;
    
    private Damageable _playerDamageable;
    private PlayerController _instance;
    
    [Header("Effets")]
    [SerializeField] private MMF_Player _launchQTE;
    [SerializeField] private MMF_Player _quitQTE;
    [SerializeField] private float _zoomAmplitude = 1f;

    [Header("Reference")] 
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    
    //QTE
    private float _distance;
    private float _width;
    private float _barSpeedSeconds;

    private bool _zoom;

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

        SetupBars();
    }

    private void OnDestroy()
    {
        _playerDamageable.OnLifeChange.RemoveListener(UpdateLifeUI);
        _instance.OnExperienceChange.RemoveListener(UpdateExperienceUI);
        _instance.OnLevelUp.RemoveListener(LevelUpUI);
        _instance.OnMoneyChange.RemoveListener(UpdateMoneyUI);

    }

    private void Update()
    {
        UpdateBar(_firstWeaponDurabilityBar, 0);
        UpdateBar(_secondWeaponDurabilityBar, 1);

        Zoom();
    }

    #region Life, Experience and Money
    
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
        _levelText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);
    }

    private void UpdateMoneyUI()
    {
        _moneyText.text = _instance.CurrentMoney + "$";
    }
    
    #endregion

    #region Weapons

    private void SetupBars()
    {
        _firstWeaponDurabilityBar.QteGameObject.SetActive(false);
        _secondWeaponDurabilityBar.QteGameObject.SetActive(false);
        
        _firstWeaponDurabilityBar.BarGameObject.SetActive(false);
        _secondWeaponDurabilityBar.BarGameObject.SetActive(false);
    }
    
    private void UpdateBar(WeaponDurabilityBar bar, int count)
    {
        List<Weapon> weapons = _instance.CurrentWeaponList;
        
        if (weapons.Count >= count+1)
        {
            Weapon weapon = weapons[count];
            
            if (bar.BarGameObject.activeInHierarchy == false) //show bar
            {
                bar.BarGameObject.SetActive(true);
                Vector3 baseScale = bar.BarGameObject.transform.localScale;
                bar.BarGameObject.transform.localScale = Vector3.zero;
                bar.BarGameObject.transform.DOScale(baseScale, 0.3f);
                
                bar.Icon.sprite = weapon.WeaponDataReference.PlayerWeaponIcon;
            }

            float fillAmount = weapon.DurabilityTimer / weapon.WeaponInfo.DurabilitySeconds;
            bar.DurabilityFillBar.fillAmount = fillAmount;
            
            //when durability is attained
            if (fillAmount >= 1)
            {
                //avoid both QTE at same time
                if (_firstWeaponDurabilityBar.IsMakingQTE == false && _secondWeaponDurabilityBar.IsMakingQTE == false && 
                    GameManager.GameManagerInstance.IsUpgrading == false)
                {
                    //launch QTE
                    bar.IsMakingQTE = true;
                    _zoom = true;
                    _launchQTE.PlayFeedbacks();
                }
            }
        }
        else if (bar.BarGameObject.activeInHierarchy)
        {
            bar.BarGameObject.SetActive(false);
        }

        if (bar.IsMakingQTE)
        {
            WeaponQTE(bar, count);
        }
    }

    private void WeaponQTE(WeaponDurabilityBar bar, int count)
    {
        //get values
        float cursorStartX = bar.CursorStartPos.transform.localPosition.x;
        float cursorEndX = bar.CursorEndPos.transform.localPosition.x;

        //setup bar
        if (bar.QteGameObject.activeInHierarchy == false)
        {
            bar.QteGameObject.SetActive(true);
            
            _distance = (float)Random.Range(25,75)/100;
            _width = (float)Random.Range(15,25)/100;
            _barSpeedSeconds = (float)Random.Range(3,5);

            bar.TopImage.fillAmount = _distance;
            bar.MiddleImage.fillAmount = _distance + _width;

            Vector3 cursorPosition = bar.Cursor.transform.localPosition;
            bar.Cursor.transform.localPosition = new Vector3(cursorStartX, cursorPosition.y, cursorPosition.z);
            bar.Cursor.transform.DOLocalMoveX(cursorEndX, _barSpeedSeconds);
        }
        
        //press space
        float currentCursorPosition = (bar.Cursor.transform.localPosition.x - cursorStartX) / (cursorEndX - cursorStartX);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentCursorPosition >= _distance && currentCursorPosition <= _distance + _width)
            {
                WinQTE(bar,count);
            }
            else
            {
                LoseQTE(bar,count);
            }
        }

        //cursor goes to end
        if (currentCursorPosition > 0.99f)
        {
            LoseQTE(bar,count);
        }

    }

    private void LoseQTE(WeaponDurabilityBar bar, int count)
    {
        _quitQTE.PlayFeedbacks();
        _zoom = false;
        
        _instance.CurrentWeaponList.Remove(_instance.CurrentWeaponList[count]);
        bar.IsMakingQTE = false;
        bar.QteGameObject.SetActive(false);
        bar.BarGameObject.SetActive(false);
    }

    private void WinQTE(WeaponDurabilityBar bar, int count)
    {
        _quitQTE.PlayFeedbacks();
        _zoom = false;
        
        _instance.CurrentWeaponList[count].DurabilityTimer = 0;
        bar.IsMakingQTE = false;
        bar.QteGameObject.SetActive(false);

    }

    private void Zoom()
    {
        float size = _cinemachineVirtualCamera.m_Lens.OrthographicSize;
        _cinemachineVirtualCamera.m_Lens.OrthographicSize = _zoom ? 
            Mathf.Lerp(size,7f - _zoomAmplitude,0.1f) :
            Mathf.Lerp(size,7f,0.1f);
    }

    #endregion
}

[Serializable]
public class WeaponDurabilityBar
{
    [Header("Durability")]
    public GameObject BarGameObject;
    public Image DurabilityFillBar;
    public Image Icon;
    
    [Header("QTE")]
    public GameObject QteGameObject;
    public Image MiddleImage;
    public Image TopImage;
    public Transform Cursor;
    public Transform CursorStartPos;
    public Transform CursorEndPos;
    public bool IsMakingQTE;
}
