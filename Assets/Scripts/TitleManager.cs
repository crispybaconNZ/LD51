using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    public void NextScene() {
        SceneManager.LoadScene("Game");
    }
}
