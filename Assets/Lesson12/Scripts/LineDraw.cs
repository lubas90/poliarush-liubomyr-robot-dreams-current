using UnityEngine;
using System.Collections;
using Lesson7;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private Material sphereMaterial;

    private bool _initialized = false;
    private GameObject explosionSphere;

    private void Start()
    {
        InputController.OnPrimaryInput += HandleRaycastTrigger;
        _initialized = true;
    }

    private void OnDestroy()
    {
        InputController.OnPrimaryInput -= HandleRaycastTrigger;
    }

    private void HandleRaycastTrigger(bool isPressed)
    {
        if (!_initialized) return;
        if (isPressed) CastRayFromPlayer();
    }

    void CastRayFromPlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
            return;
        }

        Vector3 rayOrigin = player.position + Vector3.up * player.localScale.y;
        Vector3 rayDirection = player.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, targetLayers))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name} at {hit.point}");
            StartCoroutine(HandleExplosion(hit.point));
        }
        else
        {
            Debug.Log("No object hit.");
        }

        StartCoroutine(DrawRayForDuration(rayOrigin, rayDirection, maxDistance, Color.green, 0.1f));
    }

    private IEnumerator DrawRayForDuration(Vector3 origin, Vector3 direction, float distance, Color color, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            Debug.DrawRay(origin, direction * distance, color);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator HandleExplosion(Vector3 position)
    {
        if (explosionSphere != null)
        {
            yield break;
        }

        explosionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        explosionSphere.transform.position = position;
        explosionSphere.transform.localScale = Vector3.one * explosionRadius * 2;
        if (sphereMaterial != null)
        {
            explosionSphere.GetComponent<Renderer>().material = sphereMaterial;
        }
        Destroy(explosionSphere, 0.5f);
        yield return new WaitForSeconds(0.5f);
        explosionSphere = null;

        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }
    }
}
