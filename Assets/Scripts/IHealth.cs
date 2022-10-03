using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth {
    int CurrentHealth();
    int MaxHealth();
    int DoDamage(int amount = -1);
    int HealDamage(int amount = -1);

    bool IsDead();
}
