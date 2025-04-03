using UnityEngine;

public class SyncCylinderWithFlag : MonoBehaviour
{
    [SerializeField] private GameObject flagObject;     // Assign your flag here
    [SerializeField] private GameObject cylinderObject; // Assign your cylinder here

    void Update()
    {
        if (flagObject != null && cylinderObject != null)
        {
            cylinderObject.SetActive(flagObject.activeSelf);
        }
    }
}