using System;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class Damager : MonoBehaviour
{
    [Header("Damager")] [SerializeField] protected float _damage;
    [SerializeField] protected float _speed;

    private Vector2 _direction;
    protected Rigidbody2D _rigidbody2D;
    public Damageable ParentDamageable;

    public virtual void Set(Vector2 direction, Damageable damageable)
    {
        _direction = direction;
        ParentDamageable = damageable;
        _rigidbody2D = GetComponent<Rigidbody2D>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damageable damageable = collision.collider.gameObject.GetComponent<Damageable>();
        if (damageable != null && damageable != ParentDamageable)
        {
            damageable.TakeDamage(_damage);
        }

        if (collision.collider.gameObject != ParentDamageable.gameObject)
        {
            DestroyObject();
        }
    }
}