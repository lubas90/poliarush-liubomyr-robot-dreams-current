using UnityEngine;

public class RectangleWallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab; // Assign your brick prefab in the Inspector
    [SerializeField] private GameObject planeObject; // Assign your plane object in the Inspector
    [SerializeField] private int length = 10; // Number of bricks per long side
    [SerializeField] private int width = 6; // Number of bricks per short side
    [SerializeField] private int height = 5; // Number of rows
    [SerializeField] private float brickWidth = 1.0f; // Width of a single brick
    [SerializeField] private float brickHeight = 0.5f; // Height of a single brick
    [SerializeField] private float horizontalSpacing = 0.05f; // Space between bricks horizontally
    [SerializeField] private float verticalSpacing = 0.05f; // Space between bricks vertically
    [SerializeField] private float colorVariation = 0.1f; // Maximum variation in color

    [ContextMenu("Generate Walls")]
    void GenerateWalls()
    {
        if (brickPrefab == null || planeObject == null)
        {
            Debug.LogError("Brick prefab or plane object is not assigned!");
            return;
        }

        Vector3 planePosition = planeObject.transform.position;
        float planeHeight = planeObject.transform.localScale.y;
        float wallBaseY = planePosition.y + (planeHeight / 2.0f);
        float halfBrickWidth = brickWidth / 2;
        float halfBrickDepth = brickWidth / 2;

        // Generate four walls
        GenerateWall(new Vector3(planePosition.x, wallBaseY, planePosition.z + (width * brickWidth) / 2 - halfBrickDepth), length, true, false); // Front Wall
        GenerateWall(new Vector3(planePosition.x, wallBaseY, planePosition.z - (width * brickWidth) / 2 + halfBrickDepth), length, true, false); // Back Wall
        GenerateWall(new Vector3(planePosition.x - (length * brickWidth) / 2 + halfBrickWidth, wallBaseY, planePosition.z), width, false, true); // Left Wall (rotated bricks)
        GenerateWall(new Vector3(planePosition.x + (length * brickWidth) / 2 - halfBrickWidth, wallBaseY, planePosition.z), width, false, true); // Right Wall (rotated bricks)
    }

    void GenerateWall(Vector3 startPosition, int brickCount, bool isHorizontal, bool rotateBricks)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < brickCount; col++)
            {
                float offset = (row % 2 == 0) ? 0 : (brickWidth / 2);
                float xOffset = isHorizontal ? col * (brickWidth + horizontalSpacing) - (brickCount * brickWidth / 2) + offset : 0;
                float zOffset = isHorizontal ? 0 : col * (brickWidth + horizontalSpacing) - (brickCount * brickWidth / 2) + offset;
                float yPos = startPosition.y + row * (brickHeight + verticalSpacing);
                
                Vector3 position = new Vector3(startPosition.x + xOffset, yPos, startPosition.z + zOffset);
                Quaternion rotation = rotateBricks ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
                GameObject brick = Instantiate(brickPrefab, position, rotation);
                brick.transform.parent = transform; // Keep hierarchy clean

                // Adjust the brick color slightly
                Renderer brickRenderer = brick.GetComponent<Renderer>();
                if (brickRenderer != null)
                {
                    Color baseColor = brickRenderer.material.color;
                    float variation = Random.Range(-colorVariation, colorVariation);
                    Color newColor = new Color(
                        Mathf.Clamp01(baseColor.r + variation),
                        Mathf.Clamp01(baseColor.g + variation),
                        Mathf.Clamp01(baseColor.b + variation)
                    );
                    brickRenderer.material.color = newColor;
                }
            }
        }
    }
}
