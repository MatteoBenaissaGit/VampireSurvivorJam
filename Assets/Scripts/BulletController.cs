using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class BulletController : MonoBehaviour
{
    [Header("Bullet")] 
    [SerializeField] private float _speed;
    
    private Vector2 _direction;
    private Rigidbody2D _rigidbody2D;

    public BulletController(Vector2 direction)
    {
        _direction = direction;
    }
    
    private void Start()
    {
        Destroy(gameObject,10);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        _rigidbody2D.velocity = _direction * _speed;
    }

    public void DestroyBullet()
    {   
        Destroy(gameObject);
    }
}
