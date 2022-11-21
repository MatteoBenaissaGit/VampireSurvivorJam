using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Damageable : MonoBehaviour
{
    [Header("Damageable")]
    public float Life;

    [SerializeField, ReadOnly] protected float _currentLife;

    protected void Start()
    {
        _currentLife = Life;
    }

    protected void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        _currentLife -= damage;
        if (_currentLife <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damager damager = collision.collider.gameObject.GetComponent<Damager>();
        if (damager == null)
        {
            return;
        }
        
        
    }
}
