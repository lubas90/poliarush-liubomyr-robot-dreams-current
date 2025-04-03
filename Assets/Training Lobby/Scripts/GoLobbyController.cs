using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Lesson7
{
    public class GoLobbyController : MonoBehaviour
    {
        [SerializeField] private string lobbySceneName = "LobbyyScene";
        [SerializeField] private GameObject lobbyPanel;
        [Header("UI Buttons")]
        [SerializeField] private Button GoLobbyButton;
        [SerializeField] private Button GoBackButton;
       

        
        void Start()
        {
            GoLobbyButton.interactable = true;
            GoBackButton.interactable = true;
            
            GoLobbyButton.onClick.AddListener(LoadLobbyScene);
            GoBackButton.onClick.AddListener(GoBackToGame);
        }

        void LoadLobbyScene()
        {
            SceneManager.LoadScene(lobbySceneName);
        }
        void GoBackToGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lobbyPanel.SetActive(false);
        }
        private void OnEnable()
        {
            InputController.OnGoLobbyInput += HandleGoLobbyInput;
        }

        private void OnDisable()
        {
            InputController.OnGoLobbyInput -= HandleGoLobbyInput;
        }

        private void HandleGoLobbyInput(bool isPressed)
        {
            if (isPressed)
            {
                Cursor.lockState = lobbyPanel.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = lobbyPanel.activeSelf;
                lobbyPanel.SetActive(!lobbyPanel.activeSelf);
               
            }
        }
    }
}