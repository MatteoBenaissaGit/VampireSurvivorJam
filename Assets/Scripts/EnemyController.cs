using System;
using System.Collections;
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
    private Vector2 _direction;
    private bool _isPushed;

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
        if (_isPushed)
        {
            return;
        }
        
        _direction = (_playerController.transform.position - transform.position).normalized;
        _rigidbody2D.velocity = _direction * _speed;
    }

    public void GetPushed()
    {
        _isPushed = true;
        StartCoroutine(ResetPush());
        
        const float pushForce = 15f;
        Vector2 force = -_direction * pushForce;
        _rigidbody2D.velocity = force;
    }

    private IEnumerator ResetPush()
    {
        yield return new WaitForSeconds(0.4f);
        _isPushed = false;
    }

    #endregion
}
