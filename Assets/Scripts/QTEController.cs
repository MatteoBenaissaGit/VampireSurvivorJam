using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QTEController : MonoBehaviour
{

    [SerializeField] private Image _QTE_background;
    [SerializeField] private Image _QTE_Middle;
    [SerializeField] private Image _QTE_Top;

    [SerializeField, Range(0, 1)] private float _distance = 0 ;
    [SerializeField, Range(0, 1)] private float _width = 0;



    void Start()
    {

    }

    void Update()
    {
        
        _QTE_Top.fillAmount = _distance;
        _QTE_Middle.fillAmount = _distance + _width;
    }
}
