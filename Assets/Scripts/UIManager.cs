using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private CrystalManager _crystalManager;
    [SerializeField] private EnemyManager _enemyManager;  

    [SerializeField] private TextMeshProUGUI _currentTimeText;
    [SerializeField] private TextMeshProUGUI _messageBox;
    [SerializeField] private TextMeshProUGUI _discardDeckCounterText;
    [SerializeField] private Image _discardDeckTopCard;
    [SerializeField] private Image _drawDeckTopCard;
    [SerializeField] private TextMeshProUGUI _drawDeckCounterText;

    [SerializeField] private Button[] _choices;

    [SerializeField] private Sprite _emptyDeck;
    [SerializeField] private Sprite _cardBack;
    [SerializeField] private Sprite _emptyDiscardDeck;
    [SerializeField] private Sprite _emptyDrawDeck;


    private void SubscribeToEvents() {
        _gameManager.OnInitiativeChanged.AddListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.AddListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.AddListener(HandleLevelChange);

        _playerManager.OnDiscardDeckChanged.AddListener(UpdateDiscardDeck);
        _playerManager.OnDrawDeckChanged.AddListener(UpdateDrawDeck);
        _playerManager.OnHandChanged.AddListener(UpdateHand);
        _playerManager.GetHealthEvent().AddListener(HandlePlayerHealthChange);

        _crystalManager.OnEnemySummoned.AddListener(HandleEnemySummoned);
    }

    private void OnEnable() {
        SubscribeToEvents();
    }

    private void OnDisable() {
        _gameManager.OnInitiativeChanged.RemoveListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.RemoveListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.RemoveListener(HandleLevelChange);

        _playerManager.OnDiscardDeckChanged.RemoveListener(UpdateDiscardDeck);
        _playerManager.OnDrawDeckChanged.RemoveListener(UpdateDrawDeck);
        _playerManager.OnHandChanged.RemoveListener(UpdateHand);
        _playerManager.GetHealthEvent().RemoveListener(HandlePlayerHealthChange);

        _crystalManager.OnEnemySummoned.RemoveListener(HandleEnemySummoned);
    }

    void Start() {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update() {
        
    }

    //----- InitiativeOrder/Timeline listeners -----
    public void UpdateInitiativeOrder(SortedList<int, IAbility> obj) {
    }

    public void UpdateCurrentTime(int newTime) {
        _currentTimeText.text = newTime.ToString();
    }

    private void HandleLevelChange(LevelSO level) {
        _messageBox.text = $"Level {level.level} : {level.levelName}";
    }

    //----- Deck listeners -----
    private void UpdateDiscardDeck(Deck deck) {
        // pick the correct image 
        if (deck.Count > 0) {
            _discardDeckTopCard.sprite = deck.PeekCard(deck.Count - 1).frontImage;
        } else {
            _discardDeckTopCard.sprite = _emptyDiscardDeck;
        }
        _discardDeckCounterText.text = deck.Count.ToString();
    }

    private void UpdateDrawDeck(Deck deck) {
        // pick the correct image 
        if (deck.Count > 0) {
            _drawDeckTopCard.sprite = _cardBack;
        } else {
            _drawDeckTopCard.sprite = _emptyDrawDeck;
        }
        _drawDeckCounterText.text = deck.Count.ToString();
    }

    private void UpdateHand(Deck deck) {
        if (deck.Count == 0) {
            for (int index = 0; index < _choices.Length; index++) {
                _choices[index].GetComponent<Image>().sprite = _emptyDeck;
            }           
            return;
        }

        int limit = Mathf.Min(deck.Count, _choices.Length);
        for (int index = 0; index < limit; index++) {
            _choices[index].GetComponent<Image>().sprite = deck.PeekCard(index).frontImage;
        }
    }

    //----- Crystal listeners -----
    private void HandleEnemySummoned(EnemySO enemy) {
        _messageBox.text = $"The Crystal summons a {enemy.enemyName}!";
    }

    //----- Player listeners -----
    private void HandlePlayerHealthChange(IHealth player) {

    }
}
