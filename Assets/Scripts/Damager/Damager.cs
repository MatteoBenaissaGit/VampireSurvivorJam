using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Damager : MonoBehaviour
{
    [SerializeField] protected float _speed;

    protected float _damage;
    private Vector2 _direction;
    protected Rigidbody2D _rigidbody2D;
    public Damageable ParentDamageable;
    protected List<Damageable> _touchedDamageable = new List<Damageable>();

    public virtual void Set(Vector2 direction, Damageable damageable, float damage)
    {
        _direction = direction;
        ParentDamageable = damageable;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _damage = damage;

        FaceDirection();
    }

    protected void FaceDirection()
    {
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected void Start()
    {
        
    }
    
    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        _rigidbody2D.velocity = _direction * _speed;
    }

    protected virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Damageable damageable = collider.gameObject.GetComponent<Damageable>();
        if (damageable != null &&
            damageable != ParentDamageable &&
            _touchedDamageable.Contains(damageable) == false)
        {
            DamageEnemy(damageable);
        }
    }

    protected virtual void DamageEnemy(Damageable damageable)
    {
        _touchedDamageable.Add(damageable);
        damageable.TakeDamage(_damage);
    }
}