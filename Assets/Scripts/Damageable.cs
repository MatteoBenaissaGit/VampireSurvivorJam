using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public abstract class Damageable : MonoBehaviour
{
    [Header("Damageable")]
    public float Life;
    [Range(0,5)] public float InvincibilityTimeAfterHit;

    [SerializeField, ReadOnly] public float CurrentLife;
    [SerializeField, ReadOnly] public bool IsInvincible;

    [HideInInspector] public UnityEvent OnLifeChange = new UnityEvent();

    protected void Start()
    {
        CurrentLife = Life;
    }

    protected void Update()
    {
        
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsInvincible)
        {
            return;
        }
        
        CurrentLife -= damage;
        OnLifeChange.Invoke();
        
        if (CurrentLife <= 0)
        {
            Die();
        }

        IsInvincible = true;
        StartCoroutine(Invincibility());
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(InvincibilityTimeAfterHit);
        IsInvincible = false;
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
