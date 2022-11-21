using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{

    private Slider _slider;
    private ParticleSystem _particleSyt;

    public float MoveSpeedBar = 0.5f;
    private float _sliderProgress = 0;

    private void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _particleSyt = GameObject.Find("PVBarPlayerEffect").GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        MoveSliderBarDamage(1.5f);
    }
    private void Update()
    {
        if (_slider.value < _sliderProgress) // a modif quand on aura les Heal 
        {
            _slider.value += MoveSpeedBar * Time.deltaTime;
            if (!_particleSyt.isPlaying)
                _particleSyt.Play();
        }

        else if (_slider.value > 0) // a modif quand on aura les Damages
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

    public void MoveSliderBarHeal(float Heal)
    {
        _sliderProgress = _slider.value + Heal;
    }    

    public void MoveSliderBarDamage(float Damage)
    {
        _sliderProgress = _slider.value - Damage;
    }
}
