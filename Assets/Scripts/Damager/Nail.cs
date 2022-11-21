using DG.Tweening;
using UnityEngine;

public class Nail : Damager
{
    private void Start()
    {
        base.Start();

        Destroy(gameObject,5);
        
        Vector2 baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(baseScale, 0.1f);
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
        
        if (collider.gameObject != ParentDamageable.gameObject)
        {
            DestroyObject();
        }
    }
}
