using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHealth {
    int CurrentHealth();
    int MaxHealth();
    int DoDamage(int amount = -1);
    int HealDamage(int amount = -1);
    bool IsDead();

    public class IHealthEvent : UnityEvent<IHealth> { }
    public IHealthEvent GetHealthEvent();
}
