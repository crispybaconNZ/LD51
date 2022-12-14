using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrystalManager : MonoBehaviour, IAbility, IHealth {
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;

    [SerializeField] private Sprite _icon;  // for timeline
    [SerializeField] private GameObject _prefab;    // for enemies

    private LevelSO _currentLevel = null;
    private int hitpoints;
    private const int MAX_HIT_POINTS = 20;
    private List<GameObject> _spawnedEnemies;

    //----- Events -----
    public class SummoningEvent : UnityEvent<EnemySO> { }
    public SummoningEvent OnEnemySummoned;

    public class EnemyAttackEvent : UnityEvent<string> { }
    public EnemyAttackEvent OnEnemyAttack;

    //----- Methods -----
    private void Awake() {
        if (OnEnemySummoned == null) { OnEnemySummoned = new SummoningEvent(); }
        if (OnEnemyAttack == null) { OnEnemyAttack = new EnemyAttackEvent(); }
    }

    private void OnEnable() {        
        _gameManager.OnLevelChanged?.AddListener(HandleLevelChange);
    }

    private void OnDisable() {
        _gameManager.OnLevelChanged.RemoveListener(HandleLevelChange);
    }

    // Start is called before the first frame update
    void Start() {
        _gameManager.OnLevelChanged?.AddListener(HandleLevelChange);
        hitpoints = MAX_HIT_POINTS;
        _spawnedEnemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void HandleLevelChange(LevelSO level) {
        _currentLevel = level;
        hitpoints = MAX_HIT_POINTS;
        _spawnedEnemies.Clear();
    }

    private void HandleEnemyAttack(string msg) {
        OnEnemyAttack?.Invoke(msg);
    }

    public IHealth GetTargetForPlayer() {
        if (_spawnedEnemies.Count == 0) {
            return gameObject.GetComponent<IHealth>();
        } else {
            int index = _spawnedEnemies.Count - 1;
            return _spawnedEnemies[index].GetComponent<IHealth>();
        }
    }

    public void HandleEnemyDeath(EnemyBrain casualty) {
        casualty.OnEnemyDeath.RemoveListener(HandleEnemyDeath);
        _spawnedEnemies.Remove(casualty.gameObject);
        _gameManager.order.Remove(casualty.GetComponent<IAbility>());
        casualty.gameObject.SetActive(false);
    }

    //----- IAbility methods -----
    public void Trigger() {
        // crystal only has one ability, and that's to spawn new enemies
        // but can't if spawnpoints are already full
        int maxSpawnpoints = spawnPoints.Length;
        if (!_currentLevel.boss) { maxSpawnpoints--; }
        if (_spawnedEnemies.Count == maxSpawnpoints) { return; }

        // select an enemy at random to summon from the level's spawn roster
        EnemySO enemy = _currentLevel.spawnRoster[Random.Range(0, _currentLevel.spawnRoster.Count)];

        // figure out which spawnpoint it will occupy
        int index = _spawnedEnemies.Count;

        // spawn a new enemy
        Vector3 position = spawnPoints[index].transform.position;

        GameObject _spawnedEnemy = Instantiate(_prefab, position, Quaternion.identity, this.transform);
        _spawnedEnemy.GetComponent<EnemyBrain>().LoadTemplate(enemy);
        _spawnedEnemy.GetComponent<SpriteRenderer>().sprite = enemy.sprite;
        _spawnedEnemy.GetComponent<EnemyBrain>().OnEnemyAttacks.AddListener(HandleEnemyAttack);
        _spawnedEnemy.GetComponent<EnemyBrain>().SetGameManager(_gameManager);
        _uiManager.SubscribeToDynamicEnemies(_spawnedEnemy.GetComponent<EnemyBrain>());
        _spawnedEnemy.GetComponent<EnemyBrain>().OnEnemyDeath.AddListener(HandleEnemyDeath);
        _spawnedEnemies.Add(_spawnedEnemy);
        _gameManager.order.InsertAt(_gameManager.currentTime + 1, _spawnedEnemy.GetComponent<IAbility>());

        OnEnemySummoned?.Invoke(enemy);
    }

    public Sprite GetIcon() {
        return _icon;
    }

    public string GetTag() {
        return tag;
    }

    //----- IHealth methods ----
    public int CurrentHealth() {
        return hitpoints;
    }

    public int MaxHealth() {
        return MAX_HIT_POINTS;
    }

    public int DoDamage(int amount = -1) {        
        if (amount == -1) {
            hitpoints = 0;
        } else {
            hitpoints = Mathf.Max(0, hitpoints - Mathf.Abs(amount));
        }
        return hitpoints;
    }

    public int HealDamage(int amount = -1) {
        if (amount == -1) {
            hitpoints = MAX_HIT_POINTS;
        } else {
            hitpoints = Mathf.Min(MAX_HIT_POINTS, hitpoints + Mathf.Abs(amount));
        }
        return hitpoints;
    }

    public bool IsDead() {
        return hitpoints <= 0;
    }

    public IHealth.IHealthEvent GetHealthEvent() {
        return null;
    }

}
