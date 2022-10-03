using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour, IAbility, IHealth {
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Sprite _icon;  // for timeline

    private LevelSO _currentLevel = null;
    private int hitpoints;
    private const int MAX_HIT_POINTS = 20;

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
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void HandleLevelChange(LevelSO level) {
        _currentLevel = level;
        hitpoints = MAX_HIT_POINTS;
    }

    //----- IAbility methods -----
    public void Trigger() {
        // crystal only has one ability, and that's to spawn new enemies
        Debug.Log("Crystal summons an enemy!");
    }

    public Sprite GetIcon() {
        return _icon;
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
}
