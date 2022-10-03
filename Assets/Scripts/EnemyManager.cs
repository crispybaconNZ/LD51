using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IAbility, IHealth {
    [SerializeField] private Sprite _icon; // for timeline

    private int _hitpoints;

    //----- IHealth methods -----
    public int CurrentHealth() {
        throw new System.NotImplementedException();
    }

    public int DoDamage(int amount = -1) {
        throw new System.NotImplementedException();
    }

    public int HealDamage(int amount = -1) {
        throw new System.NotImplementedException();
    }

    public bool IsDead() {
        throw new System.NotImplementedException();
    }

    public int MaxHealth() {
        throw new System.NotImplementedException();
    }

    public IHealth.IHealthEvent GetHealthEvent() {
        return null;
    }


    //----- IAbility methods -----
    public void Trigger() {
        throw new System.NotImplementedException();
    }

    public Sprite GetIcon() {
        throw new System.NotImplementedException();
    }


}
