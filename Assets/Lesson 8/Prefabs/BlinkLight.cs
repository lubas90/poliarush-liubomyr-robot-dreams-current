using UnityEngine;

public class RandomBlinkingLight : MonoBehaviour
{
    [SerializeField] Light pointLight; // Assign the Point Light in the Inspector
    [SerializeField] public float minIntensity = 0.5f; // Minimum brightness
    [SerializeField] public float maxIntensity = 3.0f; // Maximum brightness
    [SerializeField] public float blinkSpeed = 0.5f; // Time to transition between brightness levels

    private float targetIntensity; // The next target intensity
    private float currentLerpTime; // Timer for lerping

    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>(); // Get the Light component if not assigned
        }
        targetIntensity = Random.Range(minIntensity, maxIntensity); // Set initial target intensity
    }

    void Update()
    {
        // Gradually adjust intensity
        currentLerpTime += Time.deltaTime / blinkSpeed;
        pointLight.intensity = Mathf.Lerp(pointLight.intensity, targetIntensity, currentLerpTime);

        // If close to target, pick a new random brightness
        if (Mathf.Abs(pointLight.intensity - targetIntensity) < 0.1f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            currentLerpTime = 0f; // Reset lerp timer
        }
    }
}