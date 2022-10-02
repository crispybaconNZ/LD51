using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour {
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameManager _gameManager;

    private void OnEnable() {        
        _gameManager.OnLevelChanged?.AddListener(HandleLevelChange);
    }

    private void OnDisable() {
        _gameManager.OnLevelChanged.RemoveListener(HandleLevelChange);
    }

    // Start is called before the first frame update
    void Start() {
        _gameManager.OnLevelChanged?.AddListener(HandleLevelChange);
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void HandleLevelChange(LevelSO level) {
        Debug.Log("Crystal Manager still has to handle level change");
    }
}
