using UnityEngine;

public class GrenadeCollision : MonoBehaviour
{
    public float speedReductionFactor = 0.7f; // 70% of the previous speed retained
    public float minSpeedThreshold = 0.5f; // Minimum speed before stopping completely

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb != null)
        {
            // Reduce speed after each collision
            rb.velocity *= speedReductionFactor;

            // Stop completely if speed is too low
            if (rb.velocity.magnitude < minSpeedThreshold)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}