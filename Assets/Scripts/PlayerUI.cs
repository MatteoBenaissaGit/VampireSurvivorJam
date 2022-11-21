using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    private PlayerController _playerController;


    private Slider _slider;
    private ParticleSystem _particleSyt;

    public int Heal = 5;
    public int Damage = 5;

    public float MoveSpeedBar = 0.5f;
    private float _sliderProgress = 0;

    private void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _particleSyt = GameObject.Find("PVBarPlayerEffect").GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        MoveSliderBarDamage();
    }
    private void Update()
    {
        if (_slider.value != (_slider.value + Heal)) // a modif quand on aura les Heal 
        {
            _slider.value += MoveSpeedBar * Time.deltaTime;
            if (!_particleSyt.isPlaying)
                _particleSyt.Play();
        }

        else if (_slider.value > (_slider.value - Damage)) // a modif quand on aura les Damages
        {
            _slider.value -= MoveSpeedBar * Time.deltaTime;
            if (!_particleSyt.isPlaying)
                _particleSyt.Play();
        }
        else
        {
            _particleSyt.Stop();
        }
    }

    public void MoveSliderBarHeal()
    {
        _sliderProgress = _slider.value + Heal;
    }    

    public void MoveSliderBarDamage()
    {
        _sliderProgress = _slider.value - Damage;
    }
}
