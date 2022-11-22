using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShockWave : Damager
{
    [SerializeField] private float _attackDuration = 1.5f;
    
    private void Start()
    {
        base.Start();
        transform.position = ParentDamageable.transform.position;
        transform.DOScale(transform.localScale * 10, _attackDuration);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(EndShockWave());
    }

    protected override void Move()
    {
        base.Move();

        _rigidbody2D.velocity = ParentDamageable.GetComponent<Rigidbody2D>().velocity;
    }

    protected override void DamageEnemy(Damageable damageable)
    {
        base.DamageEnemy(damageable);
        
        //enemy
        EnemyController enemy = damageable.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.GetPushed();
        }
    }

    private IEnumerator EndShockWave()
    {
        yield return new WaitForSeconds(_attackDuration);
        GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(DestroyObject);
    }
}
