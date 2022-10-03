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
    private GameObject _selectedTarget;
 
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
        _selectedTarget = null;

    }

    void Update() {
        if (order.Count > 1) { AdvanceTime(); }
        if (sr.sprite == null) { sr.sprite = currentLevel.backgroundImage; }

        HandleState();
    }

    public void HandleState() {
        Debug.Log($"Current state is {currentState}");

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
                    currentState = GameState.GameLoss;
                } else {
                    currentState = GameState.SelectNextPlayer;
                }
                break;

            case GameState.SelectNextPlayer:
                currentState = SelectNextPlayer();
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
                while (!_playerManager.playedCard) { }
                currentState = GameState.DiscardPhase;
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
                currentState = GameState.SelectTarget;
                break;

            case GameState.SelectTarget:
                currentState = GameState.AttackTarget;
                break;

            case GameState.AttackTarget:
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

    public void AdvanceTime() {
        // move time to the next GameObject in the initiative order
        currentTime++;  // advance by one to go past the last actor
        // order.Purge(currentTime);   // remove everything before this new time
        currentTime = order.MinimumIndex;   // update currentTime to the first item in the initiative order
        OnTimelineChanged?.Invoke(currentTime);     // tell anyone interested that the time has changed
    }
    
    public List<IHealth> GetTargetsForEnemies() {
        List<IHealth> targets = new List<IHealth>();

        // Add the player
        targets.Add(_playerManager);

        // Add any player allies

        // return list
        return targets;
    }

    public GameState SelectNextPlayer() {
        // advance time so the next player is available
        AdvanceTime();
        Debug.Log($"Current time updated to {currentTime}");

        // get next player, should be at the next index
        IAbility nextPlayer = order.GetNext(currentTime);
        if (nextPlayer == null) { return currentState; }

        // use the tag to determine whose turn it is next
        if (nextPlayer.GetTag() == "Player") {
            return GameState.PlayerTurn;
        } else if (nextPlayer.GetTag() == "Crystal") {
            return GameState.CrystalTurn;
        } else if (nextPlayer.GetTag() == "Enemy") {
            return GameState.EnemyTurn;
        } else {
            Debug.Log($"Unknown tag '{nextPlayer.GetTag()}'");
            return GameState.SelectNextPlayer;
        }
    }

    public IHealth SelectTargetEnemy() {
        while (_selectedTarget == null) {
            // wait for a mouse click and
            // see if it's on an enemy gameobject (has tag "Enemy")
            if (_selectedTarget.tag == "Enemy") {
                IHealth target = _selectedTarget.GetComponent<IHealth>();
                // if yes, convert to IHealth and return
                return target;
            } else {
                _selectedTarget = null;
            }
            // otherwise wait for mouse click
        }
        return null;
    }

    public void HandleMouseClick(InputAction.CallbackContext context) {
        if (context.performed) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null) {
                Debug.Log($"Clicked on {hit.collider.gameObject.name}");
                _selectedTarget = hit.collider.gameObject;
                _playerManager.playedCard = true;
            }
        }
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
