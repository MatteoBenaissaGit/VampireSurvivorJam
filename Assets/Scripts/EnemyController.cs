using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyController : Damageable
{
    #region SerializedFields

    [Header("Enemy")] 
    [SerializeField, Range(0,10)] private float _speed;
    [SerializeField] private float _attackDamage;

    #endregion

    #region Variables

    private PlayerController _playerController;
    private Rigidbody2D _rigidbody2D;

    #endregion

    #region UnityEngine Methods

    private void Start()
    {
        base.Start();
        
        _playerController = PlayerController.PlayerControllerInstance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        base.Update();
        
        MoveToPlayer();
    }

    #endregion

    #region Methods

    private void MoveToPlayer()
    {
        Vector2 direction = (_playerController.transform.position - transform.position).normalized;
        _rigidbody2D.velocity = direction * _speed;
    }

    #endregion
}
