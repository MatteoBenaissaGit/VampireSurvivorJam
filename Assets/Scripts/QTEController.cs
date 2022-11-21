using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;


public class QTEController : MonoBehaviour
{

    [SerializeField] private Image _QTE_background;
    [SerializeField] private Image _QTE_Middle;
    [SerializeField] private Image _QTE_Top;

    [SerializeField] public GameObject _moveBar;

    [SerializeField, Range(0, 1)] private float _distance = 0;
    [SerializeField, Range(0, 1)] private float _width = 0;
    [SerializeField, Range(0, 10)] private float _speedBar = 0;
    [SerializeField] private float _xStart = -205;

    [SerializeField] float _xEnd = 15;


    void Start()
    {

    }

    void Update()
    {
        _QTE_Top.fillAmount = _distance;
        _QTE_Middle.fillAmount = _distance + _width;
        _moveBar.transform.DOMoveX(_xStart + _xEnd, _speedBar);

        if (_moveBar.transform.position.x >= _QTE_Top.fillAmount && _moveBar.transform.position.x <= _QTE_Middle.fillAmount)
        {
            _moveBar.transform.DOMoveX(_xEnd - _xStart, _speedBar);
            
        }
    }
}