using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private TextMeshProUGUI _currentTimeText;
    [SerializeField] private TextMeshProUGUI _messageBox;

    // Start is called before the first frame update
    private void OnEnable() {
        _gameManager.OnInitiativeChanged.AddListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.AddListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.AddListener(HandleLevelChange);
    }

    private void OnDisable() {
        _gameManager.OnInitiativeChanged.RemoveListener(UpdateInitiativeOrder);
        _gameManager.OnTimelineChanged.RemoveListener(UpdateCurrentTime);
        _gameManager.OnLevelChanged.RemoveListener(HandleLevelChange);
    }

    void Start() {
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
}
