using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerManager _playerManager;

    [SerializeField] private TextMeshProUGUI _currentTimeText;
    [SerializeField] private TextMeshProUGUI _messageBox;
    [SerializeField] private TextMeshProUGUI _discardDeckCounterText;
    [SerializeField] private TextMeshProUGUI _drawDeckCounterText;

    // Start is called before the first frame update
    private void OnEnable() {
        _gameManager.OnInitiativeChanged.AddListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.AddListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.AddListener(HandleLevelChange);

        _playerManager.OnDiscardDeckChanged.AddListener(UpdateDiscardDeck);
        _playerManager.OnDrawDeckChanged.AddListener(UpdateDrawDeck);
        _playerManager.OnHandChanged.AddListener(UpdateHand);
    }

    private void OnDisable() {
        _gameManager.OnInitiativeChanged.RemoveListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.RemoveListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.RemoveListener(HandleLevelChange);

        _playerManager.OnDiscardDeckChanged.RemoveListener(UpdateDiscardDeck);
        _playerManager.OnDrawDeckChanged.RemoveListener(UpdateDrawDeck);
        _playerManager.OnHandChanged.RemoveListener(UpdateHand);
    }

    void Start() {
        _gameManager.OnInitiativeChanged.AddListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.AddListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.AddListener(HandleLevelChange);

        _playerManager?.OnDiscardDeckChanged.AddListener(UpdateDiscardDeck);
        _playerManager?.OnDrawDeckChanged.AddListener(UpdateDrawDeck);
        _playerManager?.OnHandChanged.AddListener(UpdateHand);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void UpdateInitiativeOrder(SortedList<int, GameObject> obj) {
    }

    public void UpdateCurrentTime(int newTime) {
        _currentTimeText.text = newTime.ToString();
    }

    private void HandleLevelChange(LevelSO level) {
        _messageBox.text = $"Level {level.level} : {level.levelName}";
    }

    private void UpdateDiscardDeck(Queue<CardSO> deck) {
        _discardDeckCounterText.text = deck.Count.ToString();
    }

    private void UpdateDrawDeck(Queue<CardSO> deck) {
        _drawDeckCounterText.text = deck.Count.ToString();
    }

    private void UpdateHand(Queue<CardSO> deck) {
    }

}
