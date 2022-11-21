using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : Damageable
{
    #region Singleton

    public static PlayerController PlayerControllerInstance;

    private void Awake()
    {
        PlayerControllerInstance = this;
    }

    #endregion
    
    #region Serialized Fields

    [Header("Player")] 
    [SerializeField, Range(0, 10)] private float _speed;
    [SerializeField, Range(0, 3)] private float _shootCooldownSeconds;
    [SerializeField, Range(0, 50)] private float _detectionRange;

    [Header("References")] 
    [SerializeField] private BulletController _bulletPrefab;

    #endregion

    #region Variables

    private Rigidbody2D _rigidbody2D;
    private bool _isMoving;
    private float _shootTime;
    private List<EnemyController> _enemyInRangeList;
    private EnemyController _closestEnemy;

    #endregion

    #region UnityEngine Methods

    private void Start()
    {
        base.Start();
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _shootTime = _shootCooldownSeconds;
    }

    private void Update()
    {
        base.Update();
        
        Move();
        EnemyDetection();
        Shoot();
    }

    #endregion
    
    #region Methods

    private void Move()
    {
        //get input 
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        const float deadZone = 0.01f;
        _isMoving = movement.sqrMagnitude > deadZone;
        
        if (_isMoving == false)
        {
            _rigidbody2D.velocity = Vector2.zero;
            return;
        }

        //apply movement
        movement = movement.normalized;
        _rigidbody2D.velocity = movement * _speed;
    }

    private void EnemyDetection()
    {
        RaycastHit2D[] enemyHits = new RaycastHit2D[]{};
        Physics2D.CircleCastAll(transform.position, _detectionRange, Vector2.zero);
        foreach (RaycastHit2D hit in enemyHits)
        {
            Debug.Log("cast");
            EnemyController enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
            if (enemyController == null)
            {
                continue;
            }
            
            _enemyInRangeList.Add(enemyController);
            _closestEnemy = _enemyInRangeList
                .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                .FirstOrDefault();
        }
    }
    
    private void Shoot()
    {
        //cooldown
        _shootTime -= Time.deltaTime;
        if (_shootTime >= 0)
        {
            return;
        }
        _shootTime = _shootCooldownSeconds;
        
        //shoot
        if (_closestEnemy != null)
        {
            Debug.Log("shoot");
            Vector2 bulletDirection = (_closestEnemy.transform.position - transform.position).normalized;
            Instantiate(new BulletController(bulletDirection), transform.position, Quaternion.identity);
        }
    }

    #endregion

    #region Gizmos

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

#endif

    #endregion
}
