using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour {

    private Queue<CardSO> drawDeck;
    private Queue<CardSO> discardDeck;
    private Queue<CardSO> hand;

    public class DeckChanged : UnityEvent<Queue<CardSO>> { }
    public DeckChanged OnDrawDeckChanged;
    public DeckChanged OnDiscardDeckChanged;
    public DeckChanged OnHandChanged;

    public const int STARTING_DRAW_DECK_SIZE = 20;

    [SerializeField] private CardSO[] startingDrawDeckSeed;     // cards that can be used to populate the player's hand at start

    private void Awake() {
        if (OnDrawDeckChanged == null) { OnDrawDeckChanged = new DeckChanged(); }
        if (OnDiscardDeckChanged == null) { OnDiscardDeckChanged = new DeckChanged(); }
        if (OnHandChanged == null) { OnHandChanged = new DeckChanged(); }
    }

    private void Start() {
        drawDeck = new Queue<CardSO>();
        discardDeck = new Queue<CardSO>();
        hand = new Queue<CardSO>();

        InitialiseDrawDeck();
    }

    private void Update() {
    }

    public void DrawCards(int number=3) {
        hand.Clear();

        if (drawDeck.Count >= number) {
            // more than enough cards to draw
            for (int i = 0; i < number; i++) {
                CardSO card = drawDeck.Dequeue();
                hand.Enqueue(card);
            }
        } else {
            int remainder = number - drawDeck.Count;
            int available = drawDeck.Count;
            for (int i = 0; i < available; i++) {
                CardSO card = drawDeck.Dequeue();
                hand.Enqueue(card);
            }

            RecycleDisposeToDraw();

            for (int i = 0; i < remainder; i++) {
                CardSO card = drawDeck.Dequeue();
                hand.Enqueue(card);
            }

            OnDiscardDeckChanged?.Invoke(discardDeck);
        }

        OnDrawDeckChanged?.Invoke(drawDeck);
        OnHandChanged?.Invoke(hand);
    }

    public void RecycleDisposeToDraw() {

    }

    public void InitialiseDrawDeck() {
        for (int i = 0; i < STARTING_DRAW_DECK_SIZE; i++) {
            int randomCard = Random.Range(0, startingDrawDeckSeed.Length);
            drawDeck.Enqueue(startingDrawDeckSeed[randomCard]);
        }
        OnDrawDeckChanged?.Invoke(drawDeck);

        foreach (CardSO card in drawDeck) {
            Debug.Log(card.cardName);
        }
    }
}
