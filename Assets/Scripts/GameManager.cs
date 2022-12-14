using System;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private float _timer;
    [SerializeField] private List<Transform> _spawnPointsList;
    [SerializeField] private List<SpawnWave> _spawnWaves;
    [SerializeField] private List<SpawnInfo> _spawnList = new List<SpawnInfo>();

    [Header("Upgrade Slots")] 
    [SerializeField] private GameObject _slots;
    [SerializeField] private UpgradeSlot _firstUpgradeSlot;
    [SerializeField] private UpgradeSlot _secondUpgradeSlot;
    [SerializeField] private UpgradeSlot _thirdUpgradeSlot;
    [SerializeField] private Image _slotBackground;
    [SerializeField] private Sprite _speedIcon;
    [SerializeField] private Sprite _attackIcon;

    private PlayerController _playerController;
    [HideInInspector] public List<EnemyController> EnemiesInScene = new List<EnemyController>();

    public static GameManager GameManagerInstance;
    
    [Header("References")]
    [SerializeField] private MMF_Player _selectUpgradeEffect;

    [Header("Debug")] 
    [ReadOnly] public bool IsUpgrading = false;

    private void Awake()
    {
        GameManagerInstance = this;
    }

    private void Start()
    {
        _playerController = PlayerController.PlayerControllerInstance;
        _playerController.OnLevelUp.AddListener(ShowUpgrade);

        HideSpawnPoints();
        CalculateWave();
        
        _slots.SetActive(false);
    }

    private void OnDestroy()
    {
        _playerController.OnLevelUp.RemoveListener(ShowUpgrade);
    }

    private void Update()
    {
        TimerSpawning();
    }

    private void HideSpawnPoints()
    {
        foreach (Transform point in _spawnPointsList)
        {
            point.localScale = Vector3.zero;
        }
    }

    #region Waves
    
    private void TimerSpawning()
    {
        if (IsUpgrading)
        {
            return;
        }
        
        _timer += Time.deltaTime;
        foreach (SpawnInfo spawn in _spawnList.ToArray())
        {
            if (_timer >= spawn.Time)
            {
                EnemyController enemy = Instantiate(spawn.Enemy, GetRandomSpawnPoint(), Quaternion.identity);
                EnemiesInScene.Add(enemy);
                _spawnList.Remove(spawn);
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 spawnPointPosition = _spawnPointsList[Random.Range(0, _spawnPointsList.Count)].position;
        return new Vector3(spawnPointPosition.x, spawnPointPosition.y,0);
    }

    private void CalculateWave()
    {
        _spawnList.Clear();

        float time = 0;
        foreach (SpawnWave wave in _spawnWaves)
        {
            //create a list with each one of the enemies that will be spawnes
            List<EnemyController> enemiesControllerToSpawnList = new List<EnemyController>();
            foreach (EnemyToSpawn enemyToSpawn in wave.EnemiesToSpawn)
            {
                for (int i = 0; i < enemyToSpawn.Number; i++)
                {
                    enemiesControllerToSpawnList.Add(enemyToSpawn.Enemy);
                }
            }

            enemiesControllerToSpawnList.Sort((a, b) => 1 - 2 * UnityEngine.Random.Range(0, 1));

            float timeFrame =
                wave.SecondsFromTo.y - wave.SecondsFromTo.x; //time frame during which there can be spawns 
            float timeSeparateEachSpawn =
                timeFrame / enemiesControllerToSpawnList.Count; //spawn each timeSeparateEachSpawn

            time = wave.SecondsFromTo.x;

            foreach (EnemyController enemy in enemiesControllerToSpawnList)
            {
                _spawnList.Add(new SpawnInfo() { Enemy = enemy, Time = time });
                time += timeSeparateEachSpawn;
            }
        }
    }

    private void StopEnemiesAndPlayer()
    {
        _playerController.CanMove = false;
        _playerController.CanAttack = false;
        
        EnemiesInScene.ForEach(x => x.CanAttack = false);
        EnemiesInScene.ForEach(x => x.CanMove = false);
    }

    private void RestartEnemiesAndPlayer()
    {
        _playerController.CanMove = true;
        _playerController.CanAttack = true;
        
        EnemiesInScene.ForEach(x => x.CanAttack = true);
        EnemiesInScene.ForEach(x => x.CanMove = true);
    }
    
    #endregion

    #region Upgrades

    private void ShowUpgrade()
    {
        _slots.SetActive(true);
        _slotBackground.DOFade(0, 0);
        _slotBackground.DOFade(0.5f, 0.5f);
        IsUpgrading = true;

        StopEnemiesAndPlayer();
        
        //setup upgrades
            //if player got weapon
            int weaponCount = _playerController.CurrentWeaponList.Count;
            if (weaponCount > 0)
            {
                //get a random weapon
                Weapon weapon = _playerController.CurrentWeaponList[Random.Range(0, weaponCount)];
                
                _firstUpgradeSlot.gameObject.SetActive(true);
                string cooldownText = $"Reduce {weapon.WeaponDataReference.DisplayName} cooldown";
                SetupSlot(_firstUpgradeSlot, weapon.WeaponDataReference.PlayerWeaponSprite, cooldownText,
                    UpgradeType.Weapon, WeaponUpgradeType.Cooldown, weapon, PlayerUpgradeType.Speed);
                
                _secondUpgradeSlot.gameObject.SetActive(true);
                string damageText = $"Increase {weapon.WeaponDataReference.DisplayName} damage";
                SetupSlot(_secondUpgradeSlot, weapon.WeaponDataReference.PlayerWeaponSprite, damageText,
                    UpgradeType.Weapon, WeaponUpgradeType.Damage, weapon, PlayerUpgradeType.Speed);
                
                _thirdUpgradeSlot.gameObject.SetActive(true);
                string durabilityText = $"Increase {weapon.WeaponDataReference.DisplayName} durability";
                SetupSlot(_thirdUpgradeSlot, weapon.WeaponDataReference.PlayerWeaponSprite, durabilityText,
                    UpgradeType.Weapon, WeaponUpgradeType.Durability, weapon, PlayerUpgradeType.Speed);
            }
            //if not, upgrade player
            else
            {
                _firstUpgradeSlot.gameObject.SetActive(true);
                string damageText = $"Increase player's damage";
                SetupSlot(_firstUpgradeSlot, _attackIcon, damageText,
                    UpgradeType.Player, WeaponUpgradeType.Damage, null, PlayerUpgradeType.Damage);
                
                _thirdUpgradeSlot.gameObject.SetActive(true);
                string speedText = $"Increase player's speed";
                SetupSlot(_thirdUpgradeSlot, _speedIcon, speedText,
                    UpgradeType.Player, WeaponUpgradeType.Damage, null, PlayerUpgradeType.Speed);

                _secondUpgradeSlot.gameObject.SetActive(false);

            }
    }

    private void SetupSlot(UpgradeSlot slot, Sprite icon, string text, 
        UpgradeType upgrade, WeaponUpgradeType weaponUpgrade, Weapon weapon, PlayerUpgradeType playerUpgrade)
    {
        slot.Icon = icon;
        slot.Text = text;
        slot.Upgrade = upgrade;
        slot.Weapon = weapon;
        slot.WeaponUpgrade = weaponUpgrade;
        slot.PlayerUpgrade = playerUpgrade;
        slot.Set();
    }
    
    public void HideSlots()
    {
        _slots.gameObject.SetActive(false);
        IsUpgrading = false;
        _selectUpgradeEffect.PlayFeedbacks();
        RestartEnemiesAndPlayer();
    }

    #endregion

    #region Gizmos

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (Transform point in _spawnPointsList)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, 1f);
            }
        }
    }

#endif

    #endregion
}

[Serializable]
public struct SpawnWave
{
    public Vector2 SecondsFromTo;
    public List<EnemyToSpawn> EnemiesToSpawn;
}

[Serializable]
public struct EnemyToSpawn
{
    public EnemyController Enemy;
    public int Number;
}

[Serializable]
public struct SpawnInfo
{
    public float Time;
    public EnemyController Enemy;
}

public enum UpgradeType
{
    Weapon,
    Player
}

public enum WeaponUpgradeType
{
    Damage,
    Cooldown,
    Durability
}
public enum PlayerUpgradeType
{
    Speed,
    Damage
}