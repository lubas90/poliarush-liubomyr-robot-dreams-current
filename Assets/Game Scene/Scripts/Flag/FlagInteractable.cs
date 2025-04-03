using UnityEngine;
using Lesson7;

public class FlagInteractable : MonoBehaviour
{
    [SerializeField] private FlagHeightController flagController;
    private bool playerInRange = false;

    private FlagManager flagManager;

    protected void OnEnable()
    {
        InputController.OnInteractInput += PrimaryInputHandler;
        flagManager = FindObjectOfType<FlagManager>();
    }

    protected void OnDisable()
    {
        InputController.OnInteractInput -= PrimaryInputHandler;
    }

    public void PrimaryInputHandler(bool pressed)
    {
        if (!playerInRange || !pressed) return;

        // Example interaction: Lower flag
        flagController.SetNormalizedHeight(0f);

        // Remove this flag from the dictionary
        flagManager?.RemoveFlag(this);

        // Optional: disable the flag or play a disappear animation
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}