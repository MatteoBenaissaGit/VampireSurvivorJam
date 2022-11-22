using System;
using System.Collections;
using DG.Tweening;
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

    private bool _canBeTouched = true;

    protected void Start()
    {
        CurrentLife = Life;
    }

    protected void Update()
    {
        
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsInvincible || _canBeTouched == false)
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
        _canBeTouched = false;
        transform.DOScale(0, 0.5f).OnComplete(DestroyObject);
    }

    protected virtual void DestroyObject()
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
