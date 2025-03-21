using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lesson7
{
    public class GoLobbyController : MonoBehaviour
    {
        [SerializeField] private string lobbySceneName = "LobbyyScene";

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
                SceneManager.LoadScene(lobbySceneName);
            }
        }
    }
}