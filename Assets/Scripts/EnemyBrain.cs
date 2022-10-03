using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBrain : MonoBehaviour, IAbility, IHealth {
    [SerializeField] private Sprite _icon;
    [SerializeField] private EnemySO _template;
    private GameManager _gameManager;

    private int _hitpoints;

    public class AttackEvent : UnityEvent<string> { }
    public AttackEvent OnEnemyAttacks;
    public AttackEvent OnEnemyDamaged;
    public AttackEvent OnEnemyKilled;

    public class DeathEvent: UnityEvent<EnemyBrain> { }
    public DeathEvent OnEnemyDeath;

    // Start is called before the first frame update
    void Awake() {
        if (OnEnemyAttacks == null) { OnEnemyAttacks = new AttackEvent(); }
        if (OnEnemyDamaged == null) { OnEnemyDamaged = new AttackEvent(); } 
        if (OnEnemyKilled == null) { OnEnemyKilled = new AttackEvent(); }
        if (OnEnemyDeath == null) { OnEnemyDeath = new DeathEvent(); }
    }

    public void LoadTemplate(EnemySO template) {
        _template = template;
        _hitpoints = _template.max_hitpoints;
    }

    public void SetGameManager(GameManager gm) {
        _gameManager = gm;
    }

    //----- IAbility methods -----
    public Sprite GetIcon() {
        return _icon;
    }

    public void Trigger() {
        AbilitySO ab = _template.primaryAbility;
        if (ab.type == AbilityType.DamageDealing) {
            // get target
            Debug.Log(_gameManager != null);
            List<IHealth> targetList = _gameManager.GetTargetsForEnemies();
            int index = Random.Range(0, targetList.Count);
            IHealth target = targetList[index];

            // do damage to target
            target.DoDamage(ab.damageDealt);
            OnEnemyAttacks?.Invoke($"{_template.enemyName} attacked with {ab.abilityName} for {ab.damageDealt} damage!");
        }
    }

    public string GetTag() {
        return tag;
    }


    //----- IHealth methods -----
    public int CurrentHealth() {
        return _hitpoints;
    }

    public int MaxHealth() {
        return _template.max_hitpoints;
    }

    public int DoDamage(int amount = -1) {
        if (amount == -1) {
             _hitpoints = 0;
            OnEnemyKilled?.Invoke($"The {_template.enemyName} has been killed!");
            OnEnemyDeath?.Invoke(this);
        } else {
            _hitpoints = Mathf.Max(0, _hitpoints - Mathf.Abs(amount));
            if (_hitpoints > 0) {
                OnEnemyDamaged?.Invoke($"The {_template.enemyName} has been hurt!");
            } else {
                OnEnemyKilled?.Invoke($"The {_template.enemyName} has been killed!");
                OnEnemyDeath?.Invoke(this);
            }
        }
        
        return _hitpoints;
    }

    public int HealDamage(int amount = -1) {
        if (amount == -1) {
            _hitpoints = _template.max_hitpoints;
        } else {
            _hitpoints = Mathf.Min(_template.max_hitpoints, _hitpoints + Mathf.Abs(amount));
        }
        return _hitpoints;
    }

    public bool IsDead() {
        return _hitpoints <= 0;
    }

    public IHealth.IHealthEvent GetHealthEvent() {
        return null;
    }
}
