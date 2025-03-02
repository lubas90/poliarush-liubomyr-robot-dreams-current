using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoGenerateLightProbes : MonoBehaviour
{
    [SerializeField] private float baseSpacing = 8f; // Default spacing in open areas
    [SerializeField] private float minSpacingNearLights = 3f; // Closer probes near lights
    [SerializeField] private float detectionRadius = 12f; // Light detection range
    [SerializeField] private float lightProbeSpread = 2.5f; // Spread distance for extra probes around lights
    [SerializeField] private float minProbeHeightAboveTerrain = 1.5f; // Minimum height for bottom probe
    [SerializeField] private float maxProbeHeightAboveTerrain = 3.0f; // Max height offset in steep areas
    [SerializeField] private float dynamicObjectHeightMultiplier = 0.5f; // Adjusts probe height to match dynamic object height
    [SerializeField] private GameObject dynamicObjectReference; // Reference to a dynamic object (e.g., player or NPC)
    [SerializeField] private LayerMask lightLayerMask; // Layer mask to detect lights

    private float dynamicObjectHeight = 2.0f; // Default player/NPC height

    [ContextMenu("Generate Light Probes")]
    private void GenerateLightProbes()
    {
        if (dynamicObjectReference != null)
        {
            CharacterController characterController = dynamicObjectReference.GetComponent<CharacterController>();
            if (characterController != null)
            {
                dynamicObjectHeight = characterController.height;
            }
            else
            {
                Collider objectCollider = dynamicObjectReference.GetComponent<Collider>();
                if (objectCollider != null)
                {
                    dynamicObjectHeight = objectCollider.bounds.size.y;
                }
            }
        }

        Terrain[] terrains = FindObjectsOfType<Terrain>(); // Get all terrains
        if (terrains.Length == 0)
        {
            Debug.LogError("No terrains found in the scene!");
            return;
        }

        LightProbeGroup lightProbeGroup = GetComponent<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            lightProbeGroup = gameObject.AddComponent<LightProbeGroup>();
        }

        List<Vector3> probeList = new List<Vector3>();
        Light[] allLights = FindObjectsOfType<Light>(); // Get all light sources

        foreach (Terrain terrain in terrains)
        {
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainSize = terrainData.size;
            Vector3 terrainPosition = terrain.transform.position;

            for (float x = 0; x <= terrainSize.x; x += baseSpacing)
            {
                for (float z = 0; z <= terrainSize.z; z += baseSpacing)
                {
                    Vector3 terrainPos = new Vector3(terrainPosition.x + x, 0, terrainPosition.z + z);
                    float terrainHeight = terrain.SampleHeight(terrainPos);

                    // Determine terrain slope (steeper areas get higher bottom probes)
                    float normalizedX = x / terrainSize.x;
                    float normalizedZ = z / terrainSize.z;
                    float terrainSlope = terrainData.GetSteepness(normalizedX, normalizedZ) / 90f;

                    float bottomHeightAboveTerrain = Mathf.Lerp(minProbeHeightAboveTerrain, maxProbeHeightAboveTerrain, terrainSlope);

                    // Determine closest light and its height
                    float nearestLightHeight = terrainHeight + 3f; // Default min probe height
                    float currentSpacing = baseSpacing;

                    foreach (Light light in allLights)
                    {
                        float lightDistance = Vector3.Distance(light.transform.position, terrainPos);
                        if (lightDistance < detectionRadius)
                        {
                            currentSpacing = minSpacingNearLights; // Denser probes near lights
                            float lightHeight = light.transform.position.y;
                            nearestLightHeight = Mathf.Max(nearestLightHeight, lightHeight);
                        }
                    }

                    // Ensure bottom probe is slightly above terrain and below dynamic object head level
                    float bottomHeight = terrainHeight + Mathf.Min(bottomHeightAboveTerrain, dynamicObjectHeight * 0.3f);
                    float midHeight = terrainHeight + dynamicObjectHeight * dynamicObjectHeightMultiplier;
                    float topHeight = Mathf.Min(nearestLightHeight, terrainHeight + dynamicObjectHeight * 1.2f);

                    // Add bottom, middle, and top probes to properly influence dynamic objects
                    probeList.Add(new Vector3(terrainPos.x, bottomHeight, terrainPos.z));
                    probeList.Add(new Vector3(terrainPos.x, midHeight, terrainPos.z));
                    probeList.Add(new Vector3(terrainPos.x, topHeight, terrainPos.z));

                    // Add extra probes around light sources for better indirect lighting interpolation
                    foreach (Light light in allLights)
                    {
                        if (Vector3.Distance(light.transform.position, terrainPos) < detectionRadius)
                        {
                            Vector3 lightDirection = (terrainPos - light.transform.position).normalized;

                            // Place additional probes in front and behind the light source
                            Vector3 probeFront = light.transform.position + lightDirection * lightProbeSpread;
                            Vector3 probeBack = light.transform.position - lightDirection * lightProbeSpread;

                            // Adjust probes based on terrain and dynamic object height
                            probeFront.y = Mathf.Max(terrain.SampleHeight(probeFront) + bottomHeightAboveTerrain, bottomHeight);
                            probeBack.y = Mathf.Max(terrain.SampleHeight(probeBack) + bottomHeightAboveTerrain, bottomHeight);

                            probeList.Add(probeFront);
                            probeList.Add(probeBack);
                            probeList.Add(probeFront + Vector3.up * (midHeight - bottomHeight));
                            probeList.Add(probeBack + Vector3.up * (midHeight - bottomHeight));
                            probeList.Add(probeFront + Vector3.up * (topHeight - bottomHeight));
                            probeList.Add(probeBack + Vector3.up * (topHeight - bottomHeight));
                        }
                    }

                    // Adjust spacing dynamically
                    z += currentSpacing - baseSpacing;
                }

                x += baseSpacing - 2; // Adaptive spacing for performance optimization
            }
        }

        // Assign generated probes to the Light Probe Group
        lightProbeGroup.probePositions = probeList.ToArray();
        Debug.Log($"Generated {probeList.Count} Light Probes dynamically adjusted to terrain slope, light heights, and dynamic object heights.");
    }
}
