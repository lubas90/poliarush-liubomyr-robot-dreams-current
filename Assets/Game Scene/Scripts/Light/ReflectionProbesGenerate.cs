using UnityEngine;
using UnityEngine.Rendering; // Required for ReflectionProbeMode
using UnityEditor;
using System.Collections.Generic;


public class LocateReflectiveTexturesAndGenerateProbes : MonoBehaviour
{
    [SerializeField] private float probeSpacing = 10f;
    [SerializeField] private float probeHeightOffset = 3f;
    [SerializeField] private Vector3 probeSize = new Vector3(20, 10, 20);
    [SerializeField] private LayerMask reflectionLayerMask;
    [SerializeField] private bool useBakedProbes = true;

    [ContextMenu("Find Reflective Textures & Generate Reflection Probes")]
    private void FindReflectiveTexturesAndGenerateProbes()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        HashSet<Vector3> probePositions = new HashSet<Vector3>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            foreach (Material mat in materials)
            {
                if (mat == null) continue;

                bool isReflective = false;
                Texture metallicMap = null;
                Texture reflectionTexture = null;

                if (mat.HasProperty("_MetallicGlossMap"))
                {
                    metallicMap = mat.GetTexture("_MetallicGlossMap");
                    isReflective = metallicMap != null || mat.GetFloat("_Metallic") > 0.2f;
                }

                if (mat.HasProperty("_Glossiness") || mat.HasProperty("_Smoothness"))
                {
                    float smoothness = mat.HasProperty("_Glossiness") ? mat.GetFloat("_Glossiness") : mat.GetFloat("_Smoothness");
                    if (smoothness > 0.3f) isReflective = true;
                }

                if (mat.HasProperty("_Cube"))
                {
                    reflectionTexture = mat.GetTexture("_Cube");
                    isReflective = reflectionTexture != null;
                }

                if (mat.HasProperty("_ReflectionTex"))
                {
                    reflectionTexture = mat.GetTexture("_ReflectionTex");
                    isReflective = reflectionTexture != null;
                }

                if (isReflective)
                {
                    Vector3 probePosition = renderer.bounds.center + Vector3.up * probeHeightOffset;
                    bool shouldPlaceProbe = true;

                    foreach (Vector3 existingPos in probePositions)
                    {
                        if (Vector3.Distance(existingPos, probePosition) < probeSpacing)
                        {
                            shouldPlaceProbe = false;
                            break;
                        }
                    }

                    if (shouldPlaceProbe)
                    {
                        probePositions.Add(probePosition);
                        GenerateReflectionProbe(probePosition);
                    }
                }
            }
        }

        Debug.Log($"Total Reflection Probes Placed: {probePositions.Count}");
    }

    private void GenerateReflectionProbe(Vector3 position)
    {
        GameObject probeObject = new GameObject("Reflection Probe");
        probeObject.transform.position = position;
        ReflectionProbe probe = probeObject.AddComponent<ReflectionProbe>();

        probe.size = probeSize;
        probe.mode = useBakedProbes ? ReflectionProbeMode.Baked : ReflectionProbeMode.Realtime;
        probe.refreshMode = useBakedProbes ? ReflectionProbeRefreshMode.OnAwake : ReflectionProbeRefreshMode.EveryFrame;
        probe.intensity = 1f;
        probe.boxProjection = true;
        probe.cullingMask = reflectionLayerMask;

        probeObject.transform.parent = this.transform;
    }
}
