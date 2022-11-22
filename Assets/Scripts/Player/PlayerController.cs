using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PlayerUI))]
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
    [SerializeField, Range(0, 10)] public float Speed;
    [SerializeField, Range(0, 50)] private float _detectionRange;
    [SerializeField, Range(0, 50)] private float _weaponPickupRange;
    [SerializeField, Range(0, 3)] private float _attackCooldownSeconds;
    public float AttackDamage;

    [Header("Experience")] 
    [SerializeField] private int _experienceNecessaryForBaseLevel;
    [SerializeField,Range(1,2)] private float _experienceNecessaryMultiplier;
    
    [Header("References")] 
    [SerializeField] private Damager _attackPrefab;
    [SerializeField] private MMF_Player _hurtEffect;
    [SerializeField] private MMF_Player _hitEffect;
    [SerializeField] private MMF_Player _moneyEffect;

    #endregion

    #region Variables

    [Header("Debug")]
    //references
    private Rigidbody2D _rigidbody2D;
    
    //move
    [SerializeField, ReadOnly] private bool _isMoving;
    
    //attack & enemy
    [SerializeField] private bool _canAttack = true;
    private float _attackTime;
    private List<EnemyController> _enemyInRangeList = new List<EnemyController>();
    private EnemyController _closestEnemy;
    
    //weapons
    [SerializeField, ReadOnly] public List<Weapon> CurrentWeaponList = new List<Weapon>();
    
    //experience
    [SerializeField, ReadOnly] public int CurrentExperience = 0;
    [SerializeField, ReadOnly] public int ExperienceNecessary = 0;
    [SerializeField, ReadOnly] public int CurrentLevel = 0;
    [HideInInspector] public UnityEvent OnExperienceChange = new UnityEvent();
    [HideInInspector] public UnityEvent OnLevelUp = new UnityEvent();
    
    //money
    [SerializeField, ReadOnly] public int CurrentMoney = 0;
    [HideInInspector] public UnityEvent OnMoneyChange = new UnityEvent();
    
    #endregion

    #region UnityEngine Methods

    private void Start()
    {
        base.Start();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        ExperienceNecessary = _experienceNecessaryForBaseLevel;
    }

    private void Update()
    {
        base.Update();

        Move();
        PickupWeapon();
        EnemyDetection();
        Shoot();
        Attack();
    }

    #endregion

    #region Movement & EnemyDetection

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
        _rigidbody2D.velocity = movement * Speed;
    }

    private void EnemyDetection()
    {
        _enemyInRangeList.Clear();
        _closestEnemy = null;
        
        RaycastHit2D[] enemyHits = Physics2D.CircleCastAll(transform.position, _detectionRange, transform.forward, 0);
        foreach (RaycastHit2D hit in enemyHits)
        {
            EnemyController enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                _enemyInRangeList.Add(enemyController);
            }
        }

        if (_enemyInRangeList.Count <= 0)
        {
            return;
        }

        _closestEnemy = _enemyInRangeList
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .FirstOrDefault();
    }

    #endregion
    
    #region Attack

    private void Attack()
    {
        //cooldown
        _attackTime -= Time.deltaTime;
        if (_attackTime >= 0 || _canAttack == false || Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        _attackTime = _attackCooldownSeconds;

        Vector2 position = (Vector2)transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - position).normalized;
        Vector2 spawnPosition = position + direction * 2f;
        
        Damager attack = Instantiate(_attackPrefab, spawnPosition, Quaternion.identity);
        attack.Set(direction,this, AttackDamage);
        
        _hitEffect.PlayFeedbacks();
    }
    
    private void Shoot()
    {
        foreach (Weapon weapon in CurrentWeaponList)
        {
            if (weapon.CanShoot == false)
            {
                continue;
            }

            weapon.ShootCooldown = weapon.WeaponInfo.Cooldown;

            //shoot
            if (_closestEnemy != null)
            {
                Vector3 position = transform.position;
                Vector2 shootDirection = (_closestEnemy.transform.position - position).normalized;
                Vector2 spawnPosition = new Vector2(position.x + shootDirection.x*0.5f, position.y + shootDirection.y*0.5f);
            
                Damager projectile = Instantiate(weapon.WeaponInfo.Projectile, spawnPosition, Quaternion.identity);
                projectile.Set(shootDirection, this, weapon.WeaponInfo.Damage);
            }
        }
    }

    #endregion

    #region Pickup/Remove Weapon & Money

    private void PickupWeapon()
    {
        RaycastHit2D[] Hits = Physics2D.CircleCastAll(transform.position, _weaponPickupRange, transform.forward, 0);
        foreach (RaycastHit2D hit in Hits)
        {
            ObjectWeapon objectWeapon = hit.collider.gameObject.GetComponent<ObjectWeapon>();
            if (objectWeapon != null && objectWeapon.CanPickUp())
            {
                if (CurrentWeaponList.Find(x => x.WeaponDataReference == objectWeapon.WeaponData) == false &&
                    CurrentWeaponList.Count < 2)
                {
                    Weapon weapon = gameObject.AddComponent<Weapon>();
                    weapon.WeaponDataReference = objectWeapon.WeaponData;
                    weapon.Set();
                    CurrentWeaponList.Add(weapon);
                    AddMoney(-objectWeapon.WeaponData.MoneyCost);
                    Destroy(objectWeapon.gameObject);
                }
            }
            
            ObjectMoney objectMoney = hit.collider.gameObject.GetComponent<ObjectMoney>();
            if (objectMoney != null && objectMoney.isPickedUp == false)
            {
                AddMoney(objectMoney.Value);
                objectMoney.GetPickedUp();
            }
        }
    }

    private void LoseWeapon(Weapon weapon)
    {
        CurrentWeaponList.Remove(weapon);
    }

    #endregion

    #region Weapons Durability & QTEs

    

    #endregion

    #region Damageable

    public override void TakeDamage(float damage)
    {
        if (IsInvincible == false)
        {
            _hurtEffect.PlayFeedbacks();
        }
        base.TakeDamage(damage);
    }

    #endregion

    #region Experience

    public void AddExperience(int experience)
    {
        int newExperience = CurrentExperience + experience;
        if (newExperience >= ExperienceNecessary)
        {
            CurrentLevel++;
            CurrentExperience = newExperience - ExperienceNecessary;
            ExperienceNecessary = (int)(ExperienceNecessary * _experienceNecessaryMultiplier);
            OnLevelUp.Invoke();
        }
        else
        {
            CurrentExperience = newExperience;
            OnExperienceChange.Invoke();
        }
    }

    #endregion

    #region Money

    private void AddMoney(int money)
    {
        int newMoney = CurrentMoney + money;
        CurrentMoney = newMoney >= 0 ? newMoney : 0;
        _moneyEffect.PlayFeedbacks();
        OnMoneyChange.Invoke();
    }

    #endregion

    #region Gizmos

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, _detectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, _weaponPickupRange);

        if (_closestEnemy != null)
        {
            foreach (EnemyController enemy  in _enemyInRangeList)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(position, enemy.transform.position);
            }
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(position, _closestEnemy.transform.position);
        }
    }

#endif

    #endregion
}