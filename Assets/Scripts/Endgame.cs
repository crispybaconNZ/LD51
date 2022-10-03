using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour {
    public void NewGame(InputAction.CallbackContext context) {
        if (context.performed) {
            SceneManager.LoadScene("Fame");
        }
    }

    public void Exit(InputAction.CallbackContext context) {
        if (context.performed) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
