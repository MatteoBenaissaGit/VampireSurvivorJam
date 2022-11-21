using DG.Tweening;
using UnityEngine;

public class BulletController : Damager
{
    private void Start()
    {
        base.Start();

        Destroy(gameObject,5);
        
        Vector2 baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(baseScale, 0.1f);
    }
}
