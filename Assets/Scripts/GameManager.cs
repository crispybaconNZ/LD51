using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GameState {
    InitGame,
    InitLevel,

    CheckForEndgame,    // check whether the player has won or lost
    SelectNextPlayer,   // choose who gets next turn

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

    GameOver,        // game is over
    GameWin,         // player has won
    GameLoss,        // player has lost
}

public class GameManager : MonoBehaviour {
    public GameState currentState = GameState.InitGame;
    public int currentTime = 0;

    public InitiativeOrder order;

    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private CrystalManager _crystalManager;
    [SerializeField] private LevelSO currentLevel;

    [SerializeField] private bool _debugMode = false;

    private SpriteRenderer sr;
    // private GameObject _selectedTarget;
    private IAbility nextActor = null;

    // ----- EVENTS -----
    public class InitiativeEvent : UnityEvent<SortedList<int, IAbility>> { }
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
        currentState = GameState.InitGame;
    }

    private void OnDisable() {
        order?.OnInitiativeChanged.RemoveListener(HandleInitiativeChange);
    }

    void Start() {
        order = new InitiativeOrder();
        sr = GetComponent<SpriteRenderer>();
        currentState = GameState.InitGame;
        currentTime = 0;
    }

    void Update() {
        if (sr.sprite == null) { sr.sprite = currentLevel.backgroundImage; }

        HandleState();
    }

    public void HandleState() {
        switch (currentState) {
            case GameState.InitGame:
                _playerManager.InitialiseDrawDeck();
                currentState = GameState.InitLevel;
                break;

            case GameState.InitLevel:
                OnLevelChanged?.Invoke(currentLevel);
                _playerManager.InitialiseDrawDeck();
                currentState = GameState.SelectNextPlayer;
                sr.sprite = currentLevel.backgroundImage;
                order.Reset();
                order.InsertAt(1, _playerManager.gameObject.GetComponent<IAbility>());
                order.InsertAt(10, _crystalManager.gameObject.GetComponent<IAbility>());
                currentTime = 0;
                OnTimelineChanged?.Invoke(currentTime);
                break;

            case GameState.CheckForEndgame:
                // do check here for end of game
                if (_playerManager.IsDead()) {
                    Debug.Log("Player has died");
                    currentState = GameState.GameLoss;
                } else if (_crystalManager.IsDead()) {
                    Debug.Log("Crystal has been destroyed, next level");
                    if (currentLevel.nextLevel == null) {
                        currentState = GameState.GameWin;
                    } else {
                        currentLevel = currentLevel.nextLevel;
                        currentState = GameState.InitLevel;
                    }
                } else {
                    currentState = GameState.SelectNextPlayer;
                }
                break;

            case GameState.SelectNextPlayer:
                nextActor = SelectNextActor();
                break;

            // --- player's turn ---
            case GameState.PlayerTurn:
                currentState = GameState.DrawPhase;
                break;
          
            case GameState.DrawPhase:
                _playerManager.DrawCards();
                currentState = GameState.PlayPhase;
                break;

            case GameState.PlayPhase:
                // wait for player to select a card
                if (_playerManager.playedCard) {
                    currentState = GameState.DiscardPhase;
                    _playerManager.playedCard = false;
                }
                break;

            case GameState.SelectEnemy:
                currentState = GameState.DiscardPhase;
                break;

            case GameState.DiscardPhase:
                _playerManager.DiscardHand();
                currentState = GameState.SelectNextPlayer;
                break;

            // ---  enemy's turn ---
            case GameState.EnemyTurn:
                Debug.Log("It's the enemy's turn");
                currentState = GameState.SelectTarget;
                break;

            case GameState.SelectTarget:
                currentState = GameState.AttackTarget;
                break;

            case GameState.AttackTarget:
                Debug.Log($"It's the enemy's attack, nextActor={nextActor}");
                nextActor?.Trigger();
                currentState = GameState.CheckForEndgame;
                break;

            // --- crystal's turn ---
            case GameState.CrystalTurn:
                currentState = GameState.SpawnEnemy;
                break;

            case GameState.SpawnEnemy:
                _crystalManager.Trigger();
                currentState = GameState.CheckForEndgame;
                order.InsertAt(currentTime + 10, _crystalManager.gameObject.GetComponent<IAbility>());
                break;

            // --- end of game ---
            case GameState.GameLoss:
            case GameState.GameWin:
                break;

            default:
                Debug.Log("Unhandled game state '" + currentState + "'");
                break;
        }
    }

    public void HandleInitiativeChange(SortedList<int, IAbility> order) {
        // propagates initiative order changes to preserve privacy of InitiativeOrder object
        OnInitiativeChanged?.Invoke(order);
    }
    
    public List<IHealth> GetTargetsForEnemies() {
        List<IHealth> targets = new List<IHealth>();

        // Add the player
        targets.Add(_playerManager);

        // Add any player allies to the list of targets
        // (TODO: let the player summon allies)

        // return list
        return targets;
    }

    /// <summary>
    /// Find the next actor (player, enemy or the crystal) to make a move
    /// </summary>
    /// <returns>The actor whose turn it is next.</returns>
    public IAbility SelectNextActor() {
        // get next actor, should be at the next index
        int nextIndex = order.GetNextIndex(currentTime);
        IAbility nextActor = order.RemoveAt(nextIndex);
        currentTime = nextIndex;
        OnTimelineChanged?.Invoke(currentTime);

        // use the tag to determine whose turn it is next
        if (nextActor.GetTag() == "Player") {
            currentState = GameState.PlayerTurn;            
        } else if (nextActor.GetTag() == "Crystal") {
            currentState = GameState.CrystalTurn;
        } else if (nextActor.GetTag() == "Enemy") {
            currentState = GameState.EnemyTurn;
        } else {
            Debug.Log($"Unknown tag '{nextActor.GetTag()}'");
            currentState = GameState.SelectNextPlayer;
        }
        return nextActor;
    }

    public IHealth SelectTargetEnemy() {
        return _crystalManager.GetTargetForPlayer();
    }

    // ----- TEST METHODS -----
    public void NextLevel(InputAction.CallbackContext context) {
        if (context.performed && _debugMode) {
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

    public void TestDrawCards(InputAction.CallbackContext context) {
        if (context.performed && _debugMode) {
            currentState = GameState.DrawPhase;
        }
    }

    public void TestCrystalSpawning(InputAction.CallbackContext context) {
        if (context.performed && _debugMode) {
            currentState = GameState.SpawnEnemy;
        }
    }
}
