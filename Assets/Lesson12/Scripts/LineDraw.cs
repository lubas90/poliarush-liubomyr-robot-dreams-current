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
    [SerializeField] private GameObject explosionPrefab;

    private bool _initialized = false;
    private GameObject explosionInstance;

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
        if (explosionInstance != null)
        {
            yield break;
        }
        
        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }
        explosionInstance = Instantiate(explosionPrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Destroy(explosionInstance);
        explosionInstance = null;
    }
}
