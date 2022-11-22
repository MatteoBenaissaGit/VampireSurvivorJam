using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntitySprite : MonoBehaviour
{
    [SerializeField] private GameObject _sprite;

    private Rigidbody2D _rigidbody2D;
    private float _scaleX;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _scaleX = _sprite.transform.localScale.x;
        ScaleDown();
    }

    private void Update()
    {
        if (_rigidbody2D.velocity.magnitude > 0.1f)
        {
            bool invert = _rigidbody2D.velocity.x <= 0;
            _sprite.transform.DOScaleX(invert ? -_scaleX : _scaleX, 0.3f);
        }
    }

    private void ScaleDown()
    {
        _sprite.transform.DOScaleY(_sprite.transform.localScale.y - 0.02f, 0.1f).OnComplete(ScaleUp);
    }

    private void ScaleUp()
    {
        _sprite.transform.DOScaleY(_sprite.transform.localScale.y + 0.02f, 0.1f).OnComplete(ScaleDown);

    }
}
