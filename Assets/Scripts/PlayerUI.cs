using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image _LifeBar;
    private void Update()
    {
        float fillAmount = PlayerController.PlayerControllerInstance.CurrentLife / PlayerController.PlayerControllerInstance.Life;
        _LifeBar.fillAmount = fillAmount;
    }


}
