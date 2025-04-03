using UnityEngine;

public class FlagHeightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform flag;       
    [SerializeField] private Transform cylinder;  
    [SerializeField] private Transform flagTop; 
    [SerializeField] private Transform flagBottom;
    [SerializeField] private float _heightDecreaseSpeed;
    [SerializeField] private Vector3 rotationAxis = Vector3.up; 
    [SerializeField] private float rotationSpeed = 1f; // degrees per second


    [Header("Height Control")]
    [SerializeField, Range(0f, 1f)]
    private float normalizedHeight = 0f;           // 0 = bottom, 1 = top

    private float minY;
    private float maxY;

    void Start()
    {
        if (cylinder == null || flag == null)
        {
            Debug.LogError("Please assign both the cylinder and flag objects in the Inspector.");
            enabled = false;
            return;
        }

        // Calculate min and max Y positions based on the cylinder's position and height
        float cylinderHeight = cylinder.localScale.y;
        minY = flagBottom.position.y;
        maxY = flagTop.position.y;
    }
    public void DecreaseFlagHeightOverTime()
    {
        normalizedHeight -= _heightDecreaseSpeed * Time.deltaTime;
        normalizedHeight = Mathf.Max(0f, normalizedHeight);

        float newY = Mathf.Lerp(minY, maxY, normalizedHeight);
        flag.position = new Vector3(flag.position.x, newY, flag.position.z);
        
        RotateCylinder();
    }

    private void RotateCylinder()
    {
        if (cylinder != null)
        {
            cylinder.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
    
    void Update()
    {
        normalizedHeight = Mathf.Clamp01(normalizedHeight);
        float newY = Mathf.Lerp(minY, maxY, normalizedHeight);
        flag.position = new Vector3(flag.position.x, newY, flag.position.z);
    }

    // Allows other scripts to control the flag height
    public void SetNormalizedHeight(float value)
    {
        normalizedHeight = Mathf.Clamp01(value);
    }

    public float GetNormalizedHeight()
    {
        return normalizedHeight;
    }
}