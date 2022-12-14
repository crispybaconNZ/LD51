/*
 * Class to represent the player's cards.
 * 
 */

using UnityEngine;

[CreateAssetMenu(fileName = "New Player Card", menuName = "Scriptable Objects/New Player Card")]
public class CardSO : ScriptableObject {
    public string cardName;     // the name of the card
    public AbilitySO ability;   // the ability contained on the card
    public Sprite frontImage;

    public override string ToString() {
        return cardName;
    }
}
