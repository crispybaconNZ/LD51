using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour, IAbility, IHealth {

    private Deck drawDeck;
    private Deck discardDeck;
    private Deck hand;

    public class DeckChanged : UnityEvent<Deck> { }
    public DeckChanged OnDrawDeckChanged;
    public DeckChanged OnDiscardDeckChanged;
    public DeckChanged OnHandChanged;
    public IHealth.IHealthEvent OnHealthChanged;

    public const int STARTING_DRAW_DECK_SIZE = 20;

    [SerializeField] private CardSO[] startingDrawDeckSeed;     // cards that can be used to populate the player's hand at start
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Sprite _icon; // for timeline

    private int _hitpoints;
    private const int MAX_HITPOINTS = 20;
    public bool playedCard = false;

    private void Awake() {
        if (OnDrawDeckChanged == null) { OnDrawDeckChanged = new DeckChanged(); }
        if (OnDiscardDeckChanged == null) { OnDiscardDeckChanged = new DeckChanged(); }
        if (OnHandChanged == null) { OnHandChanged = new DeckChanged(); }
        if (OnHealthChanged == null) { OnHealthChanged = new IHealth.IHealthEvent(); }
    }

    private void Start() {
        drawDeck = new Deck();
        discardDeck = new Deck();
        hand = new Deck();
        _hitpoints = MAX_HITPOINTS;
    }

    public void DrawCards(int number=3) {
        if (hand.Count > 0) { DiscardHand(); }

        int drawn = 0;
        bool recycled = false;

        while (drawn < number) { 
            if (drawDeck.Count > 0) {
                // still a card in the draw deck
                hand.AddCard(drawDeck.TakeCard());
                drawn++;
            } else {
                RecycleDiscardToDraw();
                recycled = true;
            }
        }

        OnDrawDeckChanged?.Invoke(drawDeck);
        OnHandChanged?.Invoke(hand);
        if (recycled) { OnDiscardDeckChanged?.Invoke(discardDeck); }
    }

    public void RecycleDiscardToDraw() {
        discardDeck.Shuffle();

        while (discardDeck.Count > 0) {
            drawDeck.AddCard(discardDeck.TakeCard());
        }

        OnDiscardDeckChanged?.Invoke(discardDeck);
        OnDrawDeckChanged?.Invoke(drawDeck);
    }

    public void DiscardHand() {
        while (hand.Count > 0) {
            discardDeck.AddCard(hand.TakeCard());
        }
        OnHandChanged?.Invoke(hand);
        OnDiscardDeckChanged?.Invoke(discardDeck);
    }

    public void InitialiseDrawDeck() {
        drawDeck.Clear();
        for (int i = 0; i < STARTING_DRAW_DECK_SIZE; i++) {
            int randomCard = Random.Range(0, startingDrawDeckSeed.Length);
            drawDeck.AddCard(startingDrawDeckSeed[randomCard]);
        }
        OnDrawDeckChanged?.Invoke(drawDeck);
    }

    public void PlayCard(int index) {
        if (_gameManager.currentState != GameState.PlayPhase) { return; }

        CardSO card = hand.TakeCard(index);
        if (card != null) {
            ResolveCard(card);
            discardDeck.AddCard(card);
            OnHandChanged?.Invoke(hand);
            OnDiscardDeckChanged?.Invoke(discardDeck);

            _gameManager.order.InsertAt(_gameManager.currentTime + card.ability.cost, gameObject.GetComponent<IAbility>());
            playedCard = true;
        }
    }

    public void ResolveCard(CardSO card) {
        switch (card.ability.type) {
            case AbilityType.DamageDealing:
                ResolveDamangeDealer(card);
                break;
            case AbilityType.Healing:
                ResolveHealPlayer(card);
                break;
            case AbilityType.Summoning:
                ResolveSummoning(card);
                break;
            default:
                Debug.Log($"Unhandled ability type '{card.ability.type}' on card '{card.cardName}'");
                break;
        }
    }

    public void ResolveDamangeDealer(CardSO card) {
        // select a target
        IHealth target = _gameManager.SelectTargetEnemy();

        // apply damage to that target
        target.DoDamage(card.ability.damageDealt);
    }

    public void ResolveHealPlayer(CardSO card) {
        HealDamage(card.ability.damageHealed);
    }

    public void ResolveSummoning(CardSO card) {

    }

    //----- IAbility methods -----
    public void Trigger() {
        throw new System.NotImplementedException();
    }

    public Sprite GetIcon() {
        return _icon;
    }

    public string GetTag() {
        return tag;
    }

    //----- IHealth methods -----
    public int CurrentHealth() {
        return _hitpoints;
    }

    public int MaxHealth() {
        return MAX_HITPOINTS;
    }

    public int DoDamage(int amount = -1) {       
        if (amount == -1) {
            _hitpoints = 0;
        } else {
            _hitpoints = Mathf.Max(0, _hitpoints - Mathf.Abs(amount));
        }

        GetHealthEvent()?.Invoke(this);
        return _hitpoints;
    }

    public int HealDamage(int amount = -1) {
        if (amount == -1) {
            _hitpoints = MAX_HITPOINTS;
        } else {
            _hitpoints = Mathf.Min(MAX_HITPOINTS, _hitpoints + Mathf.Abs(amount));
        }

        GetHealthEvent()?.Invoke(this);
        return _hitpoints;
    }

    public IHealth.IHealthEvent GetHealthEvent() {
        return OnHealthChanged;
    }

    public bool IsDead() {
        return _hitpoints <= 0;
    }
}
