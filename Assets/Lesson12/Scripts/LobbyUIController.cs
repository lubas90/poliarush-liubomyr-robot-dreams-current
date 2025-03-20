using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button trainingSpaceButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitGameButton;

    [Header("Scene Management")]
    [SerializeField] private string trainingSpaceSceneName;

    void Start()
    {
        // Disable "Start" and "Settings" buttons
        startButton.interactable = false;
        settingsButton.interactable = false;

        // Setup listeners
        trainingSpaceButton.onClick.AddListener(LoadTrainingSpace);
        quitGameButton.onClick.AddListener(QuitGame);
    }

    void LoadTrainingSpace()
    {
        SceneManager.LoadScene(trainingSpaceSceneName);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits Play mode in Unity Editor
#else
        Application.Quit(); // Quits the built application
#endif
    }
}