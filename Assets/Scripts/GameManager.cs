using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GameState {
    InitGame,
    InitLevel,

    PlayerTurn,     // Start of player turn
    DrawPhase,      // Draw cards into hand
    PlayPhase,      // Play a card from hand
    SelectEnemy,    // Select an enemy target
    DiscardPhase,   // Discard remaining cards

    EnemyTurn,      // Start of enemy turn
    SelectTarget,   // Select target for enemy to attack
    AttackTarget,   // Perform attack on player

    CrystalTurn,    // Start of crystal's turn
    SpawnEnemy,     // Spawn an enemy onto the battlefield

    GameOver        // game is over
}

public class GameManager : MonoBehaviour {
    public GameState currentState = GameState.InitGame;
    public int currentTime = 0;

    private InitiativeOrder order;

    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private LevelSO currentLevel;

    private SpriteRenderer sr;
 
    // ----- EVENTS -----
    public class InitiativeEvent : UnityEvent<SortedList<int, GameObject>> { }
    public InitiativeEvent OnInitiativeChanged;

    public class TimelineEvent: UnityEvent<int> { }
    public TimelineEvent OnTimelineChanged;

    public class LevelEvent: UnityEvent<LevelSO> { }
    public LevelEvent OnLevelChanged;

    // ----- METHODS -----
    void Awake() {
        if (OnInitiativeChanged == null) { OnInitiativeChanged = new InitiativeEvent(); }
        if (OnTimelineChanged == null) { OnTimelineChanged = new TimelineEvent(); }
        if (OnLevelChanged == null) { OnLevelChanged = new LevelEvent(); }
    }

    private void OnEnable() {
        order?.OnInitiativeChanged.AddListener(HandleInitiativeChange);
        currentState = GameState.InitLevel;
    }

    private void OnDisable() {
        order?.OnInitiativeChanged.RemoveListener(HandleInitiativeChange);
    }

    void Start() {
        order = new InitiativeOrder();
        sr = GetComponent<SpriteRenderer>();
        currentState = GameState.InitLevel;
    }

    void Update() {
        if (order.Count > 1) { AdvanceTime(); }
        if (sr.sprite == null) { sr.sprite = currentLevel.backgroundImage; }

        HandleState();
    }

    public void HandleState() {
        switch (currentState) {
            case GameState.InitLevel:
                OnLevelChanged.Invoke(currentLevel);
                currentState = GameState.PlayerTurn;
                break;
            default:
                Debug.Log("Unhandled game state '" + currentState + "'");
                break;
        }
    }

    public void HandleInitiativeChange(SortedList<int, GameObject> order) {
        // propagates initiative order changes to preserve privacy of InitiativeOrder object
        OnInitiativeChanged?.Invoke(order);
    }

    public void AdvanceTime() {
        // move time to the next GameObject in the initiative order
        currentTime++;  // advance by one to go past the last actor
        order.Purge(currentTime);   // remove everything before this new time
        currentTime = order.MinimumIndex;   // update currentTime to the first item in the initiative order
        OnTimelineChanged?.Invoke(currentTime);     // tell anyone interested that the time has changed
    }

    public void NextLevel(InputAction.CallbackContext context) {
        if (context.performed) {
            if (currentLevel == null) {
                currentState = GameState.GameOver;
            }

            currentLevel = currentLevel.nextLevel;
            if (currentLevel != null) {
                currentState = GameState.InitLevel;
            } else {
                currentState = GameState.GameOver;
            }
        }
    }
}
