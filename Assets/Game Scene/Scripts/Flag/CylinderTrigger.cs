using UnityEngine;

public class CylinderTrigger : MonoBehaviour
{
    public FlagHeightController flagController;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            flagController.DecreaseFlagHeightOverTime();
        }
    }
}