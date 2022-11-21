using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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

    protected void TakeDamage(float damage)
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
}
