using UnityEngine;
using System.Collections;
using System.Text;
using Lesson7;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Transform grenadeSpawnLocation;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform crosshair;
    [SerializeField] private float grenadeThrowForce = 10f;
    [SerializeField] private float grenadeRotationForce = 5f;
    [SerializeField] private float explosionDelay = 2f;
    
    private bool _initialized = false;

    private void Start()
    {
        InputController.OnGrenadeInput += HandleRaycastTrigger;
        _initialized = true;
    }

    private void OnDestroy()
    {
        InputController.OnGrenadeInput -= HandleRaycastTrigger;
    }

    private void HandleRaycastTrigger(bool isPressed)
    {
        if (!_initialized) return;
        if (isPressed) ThrowGrenade();
    }

    private void ThrowGrenade()
    {
        if (grenadeSpawnLocation == null || grenadePrefab == null || crosshair == null)
        {
            Debug.LogError("Missing necessary references!");
            return;
        }

        Vector3 spawnPosition = grenadeSpawnLocation.position + Vector3.up * 1.5f; // Adjust spawn height
        GameObject grenade = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            Vector3 throwDirection = crosshair.forward;
            rb.AddForce(throwDirection * grenadeThrowForce, ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * grenadeRotationForce, ForceMode.VelocityChange);
        }

        StartCoroutine(HandleExplosion(grenade));
    }

    private IEnumerator HandleExplosion(GameObject grenade)
    {
        yield return new WaitForSeconds(explosionDelay);
        
        Vector3 position = grenade.transform.position;
        Instantiate(explosionPrefab, position, Quaternion.identity);
        
        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        StringBuilder affectedObjectsLog = new StringBuilder("Explosion occurred. Objects affected: ");
        
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject == grenade) continue; // Skip the grenade itself
            
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                affectedObjectsLog.Append(hit.gameObject.name + ", ");
                float distance = Vector3.Distance(position, hit.transform.position);
                float falloffFactor = Mathf.Clamp01(1 - (distance / explosionRadius));
                rb.AddExplosionForce(explosionForce * falloffFactor, position, explosionRadius);
            }
        }
        
        Debug.Log(affectedObjectsLog.ToString().TrimEnd(',', ' '));
        Destroy(grenade);
    }
}