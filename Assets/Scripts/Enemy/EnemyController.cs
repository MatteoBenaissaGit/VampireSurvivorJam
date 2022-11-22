using System;
using System.Collections;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyController : Damageable
{
    #region SerializedFields

    [Header("Enemy")] 
    [SerializeField, Range(0,10)] private float _speed;
    [SerializeField] private float _attackDamage;
    [SerializeField, Range(0, 10)] private float _attackRange;
    [SerializeField] private int _experienceGiven;
    [SerializeField] private int _moneyGiven;

    [Header("References")] 
    [SerializeField] private ObjectMoney _moneyPrefab;
    [SerializeField] private MMF_Player _hurtEffect;

    #endregion

    #region Variables

    private PlayerController _playerController;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _direction;
    private Vector2 _velocity;
    private bool _isPushed;

    [SerializeField, ReadOnly] private float _distanceToPlayer;

    #endregion

    #region UnityEngine Methods

    private void Start()
    {
        base.Start();
        
        _playerController = PlayerController.PlayerControllerInstance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        //Scaling
        Vector3 baseScale = transform.localScale;
        transform.localScale = Vector3.zero; 
        transform.DOScale(baseScale,1f);
    }

    private void Update()
    {
        base.Update();
        
        MoveToPlayer();

        if (_playerController != null)
        {
            AttackPlayer();
        }
    }

    #endregion

    #region Methods

    public override void TakeDamage(float damage)
    {
        _hurtEffect.PlayFeedbacks();
        
        base.TakeDamage(damage);
    }

    private void MoveToPlayer()
    {
        if (_isPushed)
        {
            return;
        }

        if (_playerController != null)
        {
            _direction = (_playerController.transform.position - transform.position);
            _direction = Vector3.Normalize(_direction);
            _velocity = _direction * _speed;
            _rigidbody2D.velocity = _velocity;
        }
        else
        {
            if (_speed >= 0)
            {
                _speed *= _speed > 0.05f ? 0.9f : 0;
            }
        }
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

    private void AttackPlayer()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _playerController.transform.position);
        if (_distanceToPlayer < _attackRange)
        {
            _playerController.GetComponent<Damageable>().TakeDamage(_attackDamage);
        }
    }

    protected override void Die()
    {
        _playerController.AddExperience(_experienceGiven);
        DropMoney();
        
        base.Die();
    }

    private void DropMoney()
    {
        ObjectMoney money = Instantiate(_moneyPrefab,transform.position, Quaternion.identity);
        money.Value = _moneyGiven;
    }

    #endregion

    #region Gizmos

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

#endif

    #endregion
}
