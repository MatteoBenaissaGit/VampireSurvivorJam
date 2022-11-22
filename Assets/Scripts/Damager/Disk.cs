using DG.Tweening;
using UnityEngine;

public class Disk : Damager
{
    private void Start()
    {
        base.Start();

        Destroy(gameObject,5);
        
        Vector2 baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(baseScale, 0.25f);
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0,0,1));
    }

    public override void Set(Vector2 direction, Damageable damageable, float damage)
    {
        Vector2 position = (Vector2)transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newDirection = (mousePosition - position).normalized;

        base.Set(newDirection, damageable, damage);
    }
}
