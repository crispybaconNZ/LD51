using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Objects/New Enemy")]
public class EnemySO : ScriptableObject {
    public string enemyName;
    public int max_hitpoints;
    public AbilitySO primaryAbility;
    public Sprite sprite;
}
