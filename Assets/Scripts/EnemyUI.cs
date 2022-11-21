using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyController))]
public class EnemyUI : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Image _lifeBarImage;

    private EnemyController _enemyController;

    private void Start()
    {
        _enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        LifeBar();
    }

    private void LifeBar()
    {
        if (_enemyController.CurrentLife >= _enemyController.Life)
        {
            _lifeBarImage.gameObject.SetActive(false);
            return;
        }

        if (_lifeBarImage.gameObject.activeInHierarchy == false)
        {
            GameObject lifeBar = _lifeBarImage.gameObject;
            lifeBar.SetActive(true);
            Vector2 baseScale = lifeBar.transform.localScale;
            lifeBar.transform.localScale = Vector3.zero;
            lifeBar.transform.DOScale(baseScale, 0.25f);
        }

        float fillPercentage = _enemyController.CurrentLife / _enemyController.Life;
        _lifeBarImage.DOFillAmount(fillPercentage, 0.2f);

        _lifeBarImage.fillAmount = fillPercentage;
    }
}
