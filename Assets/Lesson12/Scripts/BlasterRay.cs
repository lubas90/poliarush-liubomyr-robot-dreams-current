using UnityEngine;
using System.Collections;
using Lesson7;

public class ShootRaycast : MonoBehaviour
{
    public Transform crosshair; // Assign the Crosshair Transform (3D Object, not UI)
    public Transform weapon; // Assign the Weapon Transform (barrel exit)
    public float sphereCastRadius = 0.1f; // Radius of the SphereCast
    public Color cylinderColor = Color.red; // Set the color of the cylinder

    private bool _initialized = false;

    private void Start()
    {
        InputController.OnPrimaryInput += HandleRaycastTrigger; // Subscribe to event
        _initialized = true;
    }

    private void OnDestroy()
    {
        if (_initialized)
        {
            InputController.OnPrimaryInput -= HandleRaycastTrigger; // Unsubscribe to prevent memory leaks
        }
    }

    private void HandleRaycastTrigger(bool isPressed)
    {
        if (isPressed) // Only trigger if input is active
        {
            CreateShotEffect();
        }
    }

    private void CreateShotEffect()
    {
        if (crosshair == null || weapon == null) return;

        // Cast a sphere from the weapon's position in the forward direction
        Vector3 weaponPos = weapon.position;
        Vector3 direction = crosshair.transform.forward;
        RaycastHit hit;

        if (Physics.SphereCast(weaponPos, sphereCastRadius, direction, out hit, Mathf.Infinity))
        {
            Vector3 hitPoint = hit.point;

            // Calculate the distance between the crosshair and the hit point
            float distance = Vector3.Distance(crosshair.position, hitPoint);

            // Generate random offsets in X and Y (1% of the distance)
            float randomOffsetX = Random.Range(-0.01f, 0.01f) * distance;
            float randomOffsetY = Random.Range(-0.01f, 0.01f) * distance;

            // Create an empty parent object for the cylinder
            GameObject shotEffectParent = new GameObject("ShotEffect");
            shotEffectParent.transform.position = hitPoint;

            // Create a cylinder from the weapon to the hit point
            CreateCylinder(weaponPos, hitPoint, shotEffectParent);

            // Destroy the parent object after 1 second
            Destroy(shotEffectParent, 1f);
        }
    }

    private void CreateCylinder(Vector3 start, Vector3 end, GameObject parent)
    {
        // Calculate position, rotation, and scale of the cylinder
        Vector3 midPoint = (start + end) / 2;
        float distance = Vector3.Distance(start, end);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        // Set cylinder properties
        cylinder.transform.position = midPoint;
        cylinder.transform.up = (end - start).normalized; // Align with the direction
        cylinder.transform.localScale = new Vector3(0.05f, distance / 2, 0.05f); // Adjust size

        // Apply color
        Renderer renderer = cylinder.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = cylinderColor;
        }

        // Set cylinder as a child of the shotEffectParent
        cylinder.transform.SetParent(parent.transform);
    }
}