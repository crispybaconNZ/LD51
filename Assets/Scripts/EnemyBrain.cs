using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBrain : MonoBehaviour, IAbility, IHealth {
    [SerializeField] private Sprite _icon;
    [SerializeField] private EnemySO _template;
    [SerializeField] private GameManager _gameManager;

    private int _hitpoints;

    public class AttackEvent : UnityEvent<string> { }
    public AttackEvent OnEnemyAttacks;

    // Start is called before the first frame update
    void Awake() {
        if (OnEnemyAttacks == null) { OnEnemyAttacks = new AttackEvent(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTemplate(EnemySO template) {
        _template = template;
        _hitpoints = _template.max_hitpoints;
    }

    //----- IAbility methods -----
    public Sprite GetIcon() {
        return _icon;
    }

    public void Trigger() {
        AbilitySO ab = _template.primaryAbility;
        Debug.Log($"Enemy attacks with {ab.abilityName}");

        if (ab.type == AbilityType.DamageDealing) {
            // get target
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
        } else {
            _hitpoints = Mathf.Max(0, _hitpoints - Mathf.Abs(amount));
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
        throw new System.NotImplementedException();
    }

    public IHealth.IHealthEvent GetHealthEvent() {
        return null;
    }
}
