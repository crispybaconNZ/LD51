/*
 * Class that represents an ability for either a player's card or an enemy.
 */

using UnityEngine;


public enum AbilityType {
    DamageDealing,
    Healing,
    Summoning
}

public enum DamageType {
    NormalDamage,
    ArmourPiercingDamage,
    FireDamage
}

[CreateAssetMenu(fileName = "New Ability", menuName = "Scriptable Objects/New Ability")]
public class AbilitySO : ScriptableObject {
    public const int MELEE_RANGE = 0;

    public string abilityName;          // name of the ability
    public int cost;                    // amount of time (in seconds) that the ability uses
    public AbilityType type;            // ability's type
    public DamageType damageType;       // type of damage dealt (damage-dealing abilities)
    public int damageDealt = 0;         // amount of damage done (damage-dealing abilities)
    public int damageHealed = 0;        // amount of damage healed (healing abilities)
    public int range = MELEE_RANGE;     // range of ability (damage-dealing abilities)
    public GameObject target = null;    // target to apply/heal damage to (damage-dealing and healing abilities)
}
