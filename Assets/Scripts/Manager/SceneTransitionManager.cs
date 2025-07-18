using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Lưu trạng thái khi load scene mới (trừ khi là New Game)
        if (scene.name != "MainMenu") // Tránh lưu khi quay lại menu từ New Game
        {
            Pref.SaveGameState(scene.name);
        }
    }
}