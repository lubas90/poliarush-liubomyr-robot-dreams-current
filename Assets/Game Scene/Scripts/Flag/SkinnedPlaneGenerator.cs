using UnityEngine;

public class SkinnedPlaneGenerator : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [ContextMenu("Generate Skinned Plane")]
    public void GenerateSkinnedPlane()
    {
        if (target == null)
        {
            Debug.LogWarning("Please assign a target GameObject.");
            return;
        }

        var mesh = new Mesh
        {
            name = "SkinnedPlaneMesh"
        };

        int width = 10;
        int height = 6;
        float scale = 0.1f;

        int vertCount = (width + 1) * (height + 1);
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        BoneWeight[] boneWeights = new BoneWeight[vertCount];
        int[] triangles = new int[width * height * 6];

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                int i = x + y * (width + 1);
                vertices[i] = new Vector3(x * scale, -y * scale, 0);
                uv[i] = new Vector2((float)x / width, (float)y / height);

                boneWeights[i] = new BoneWeight
                {
                    boneIndex0 = 0,
                    weight0 = 1f
                };
            }
        }

        int t = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = x + y * (width + 1);
                triangles[t++] = i;
                triangles[t++] = i + width + 1;
                triangles[t++] = i + 1;

                triangles[t++] = i + 1;
                triangles[t++] = i + width + 1;
                triangles[t++] = i + width + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.boneWeights = boneWeights;
        mesh.RecalculateNormals();

        Matrix4x4[] bindPoses = new Matrix4x4[1];
        bindPoses[0] = target.transform.worldToLocalMatrix;
        mesh.bindposes = bindPoses;

        GameObject boneObj = new GameObject("FlagBone");
        boneObj.transform.SetParent(target.transform);
        boneObj.transform.localPosition = Vector3.zero;
        boneObj.transform.localRotation = Quaternion.identity;

        var smr = target.GetComponent<SkinnedMeshRenderer>();
        if (!smr) smr = target.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
        smr.bones = new Transform[] { boneObj.transform };
        smr.rootBone = boneObj.transform;

        Debug.Log("✅ Skinned plane mesh generated on: " + target.name);
    }
}
