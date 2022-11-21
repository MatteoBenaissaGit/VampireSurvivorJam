using UnityEngine;

public class Attack : Damager
{
    private void Start()
    {
        base.Start();
        
        Destroy(gameObject,0.5f);
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
}
