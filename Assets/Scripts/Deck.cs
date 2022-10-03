using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Deck {
    private List<CardSO> _cards;

    public int Count { get { return _cards.Count; } }

    public Deck() {
        _cards = new List<CardSO>();
    }

    ~Deck() {
        _cards.Clear();
    }

    public void Shuffle() {
        List<CardSO> shuffled = new List<CardSO>();
        while (_cards.Count > 0) {
            int index = Random.Range(0, _cards.Count);
            CardSO card = _cards[index];
            _cards.Remove(card);
            shuffled.Add(card);
        }
        _cards = shuffled;
    }

    public CardSO PeekCard(int index=0) {
        return _cards[index];
    }

    public CardSO TakeCard() {
        CardSO card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void AddCard(CardSO card, bool to_top = true) {
        if (to_top) {
            _cards.Insert(0, card);
        } else {
            _cards.Add(card);
        }
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();

        sb.Append($"{Count} cards: ");
        foreach (CardSO card in _cards) {
            sb.Append($"{card} / ");
        }

        return sb.ToString();
    }
}
