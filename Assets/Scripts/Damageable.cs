using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Damageable : MonoBehaviour
{
    [Header("Damageable")]
    public float Life;

    [SerializeField, ReadOnly] public float CurrentLife;

    protected void Start()
    {
        CurrentLife = Life;
    }

    protected void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        CurrentLife -= damage;
        if (CurrentLife <= 0)
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
