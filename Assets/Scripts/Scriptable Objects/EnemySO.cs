using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Objects/New Enemy")]
public class EnemySO : ScriptableObject {
    public string enemyName;
    public int health;
    public AbilitySO primaryAbility;
}
