using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Damager : MonoBehaviour
{
    [Header("Damager")] [SerializeField] protected float _damage;
    [SerializeField] protected float _speed;

    private Vector2 _direction;
    protected Rigidbody2D _rigidbody2D;
    public Damageable ParentDamageable;
    protected List<Damageable> _touchedDamageable = new List<Damageable>();

    public virtual void Set(Vector2 direction, Damageable damageable)
    {
        _direction = direction;
        ParentDamageable = damageable;
        _rigidbody2D = GetComponent<Rigidbody2D>();

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
    
    protected void Update()
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
        if (damageable != null && damageable != ParentDamageable && _touchedDamageable.Contains(damageable) == false)
        {
            _touchedDamageable.Add(damageable);
            damageable.TakeDamage(_damage);
            
            //enemy
            EnemyController enemy = damageable.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.GetPushed();
            }
        }
    }
}