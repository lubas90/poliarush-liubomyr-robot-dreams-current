using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab; // Assign your brick prefab in the Inspector
    [SerializeField] private GameObject planeObject; // Assign your plane object in the Inspector
    [SerializeField] private int width = 10; // Number of bricks per row
    [SerializeField] private int height = 5; // Number of rows
    [SerializeField] private float brickWidth = 1.0f; // Width of a single brick
    [SerializeField] private float brickHeight = 0.5f; // Height of a single brick
    [SerializeField] private float horizontalSpacing = 0.05f; // Space between bricks horizontally
    [SerializeField] private float verticalSpacing = 0.05f; // Space between bricks vertically

    [ContextMenu("Generate Wall")]
    void GenerateWall()
    {
        if (brickPrefab == null || planeObject == null)
        {
            Debug.LogError("Brick prefab or plane object is not assigned!");
            return;
        }

        Vector3 planePosition = planeObject.transform.position;
        float planeHeight = planeObject.transform.localScale.y;
        float wallBaseY = planePosition.y + (planeHeight / 2.0f);

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // Stagger every other row to mimic real bricks
                float xOffset = (row % 2 == 0) ? 0 : (brickWidth / 2);
                float xPos = col * (brickWidth + horizontalSpacing) + xOffset;
                float yPos = wallBaseY + (row * (brickHeight + verticalSpacing));
                
                Vector3 position = new Vector3(xPos, yPos, planePosition.z);
                GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);
                brick.transform.parent = transform; // Keep hierarchy clean
            }
        }
    }
}