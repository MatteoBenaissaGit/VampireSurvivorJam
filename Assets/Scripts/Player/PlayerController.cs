using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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
    [SerializeField, Range(0, 10)] private float _speed;
    [SerializeField, Range(0, 50)] private float _detectionRange;
    [SerializeField, Range(0, 50)] private float _weaponPickupRange;
    [SerializeField, Range(0, 3)] private float _attackCooldownSeconds;

    [Header("Experience")] 
    [SerializeField] private int _experienceNecessaryForBaseLevel;
    [SerializeField,Range(1,2)] private float _experienceNecessaryMultiplier;
    
    [Header("References")] 
    [SerializeField] private Damager _attackPrefab;

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
    [SerializeField, ReadOnly] private List<Weapon> _currentWeaponList = new List<Weapon>();
    
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
        _rigidbody2D.velocity = movement * _speed;
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
        attack.Set(direction,this);
    }
    
    private void Shoot()
    {
        foreach (Weapon weapon in _currentWeaponList)
        {
            if (weapon.CanShoot == false)
            {
                return;
            }

            weapon.ShootCooldown = weapon.WeaponInfoData.Cooldown;

            //shoot
            if (_closestEnemy != null)
            {
                Vector3 position = transform.position;
                Vector2 bulletDirection = (_closestEnemy.transform.position - position).normalized;
                Vector2 spawnPosition = new Vector2(position.x + bulletDirection.x*0.5f, position.y + bulletDirection.y*0.5f);
            
                Damager bullet = Instantiate(weapon.WeaponInfoData.Projectile, spawnPosition, Quaternion.identity);
                bullet.Set(bulletDirection, this);
            }
        }
    }

    #endregion

    #region Pick up Weapon & Money

    private void PickupWeapon()
    {
        RaycastHit2D[] Hits = Physics2D.CircleCastAll(transform.position, _weaponPickupRange, transform.forward, 0);
        foreach (RaycastHit2D hit in Hits)
        {
            ObjectWeapon objectWeapon = hit.collider.gameObject.GetComponent<ObjectWeapon>();
            if (objectWeapon != null && objectWeapon.CanPickUp())
            {
                if (_currentWeaponList.Find(x => x.WeaponInfoData == objectWeapon.WeaponData) == false)
                {
                    Weapon weapon = gameObject.AddComponent<Weapon>();
                    weapon.WeaponInfoData = objectWeapon.WeaponData;
                    _currentWeaponList.Add(weapon);
                    Destroy(objectWeapon.gameObject);
                }
            }
            
            ObjectMoney objectMoney = hit.collider.gameObject.GetComponent<ObjectMoney>();
            if (objectMoney != null)
            {
                AddMoney(objectMoney.Value);
                Destroy(objectMoney.gameObject);
            }
        }
    }

    #endregion

    #region Damageable

    public override void TakeDamage(float damage)
    {
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