using UnityEngine;
using UnityEditor;

public class AutoGenerateLightProbes : MonoBehaviour
{
    [SerializeField] private Terrain[] terrain;
    [SerializeField] private float spacing = 10f; // Distance between probes
    [SerializeField] private float heightOffset = 2f; // Height above the terrain
    [SerializeField] private float extraHeight = 3f; // Additional higher probe for better lighting interpolation

    [ContextMenu("Generate Light Probes")]
    private void GenerateLightProbes()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned! Assign a Terrain object.");
            return;
        }

        // Create or find a Light Probe Group
        LightProbeGroup lightProbeGroup = GetComponent<LightProbeGroup>();
        if (lightProbeGroup == null)
        {
            lightProbeGroup = gameObject.AddComponent<LightProbeGroup>();
        }
        
        // Generate probe positions
        var probeList = new System.Collections.Generic.List<Vector3>();
        
        for (int i = 0; i < terrain.Length; i++)
        {
            // Get terrain size
            Vector3 terrainSize = terrain[i].terrainData.size;
            Vector3 terrainPosition = terrain[i].transform.position;



            for (float x = 0; x <= terrainSize.x; x += spacing)
            {
                for (float z = 0; z <= terrainSize.z; z += spacing)
                {
                    // Get the terrain height at this position
                    float terrainHeight =
                        terrain[i].SampleHeight(new Vector3(terrainPosition.x + x, 0, terrainPosition.z + z));

                    // Primary probe above the terrain
                    Vector3 probePos = new Vector3(terrainPosition.x + x, terrainHeight + heightOffset,
                        terrainPosition.z + z);
                    probeList.Add(probePos);

                    // Additional higher probe for better interpolation (optional)
                    probeList.Add(probePos + Vector3.up * extraHeight);
                }
            }

            // Assign probes to Light Probe Group
            lightProbeGroup.probePositions = probeList.ToArray();
            Debug.Log($"Generated {probeList.Count} light probes over the terrain.");
        }
    }
}