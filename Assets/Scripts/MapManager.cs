using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapManager : MonoBehaviour
{
    [Header("Map Display")]
    [SerializeField] private SpriteRenderer mapRenderer;
    [SerializeField] private GridManager gridManager;
    
    [Header("Map Settings")]
    [SerializeField] private bool scaleToGrid = true;
    [SerializeField] private float pixelsPerGridCell = 100f; // How many pixels = 1 grid cell
    
    private Sprite currentMap;
    private Texture2D currentTexture;

    void Start()
    {
        // Create map renderer if it doesn't exist
        if (mapRenderer == null)
        {
            CreateMapRenderer();
        }
        
        // Only load a map if one was selected
        if (GameData.Instance != null && !string.IsNullOrEmpty(GameData.Instance.selectedMapPath))
        {
            LoadMapFromPath(GameData.Instance.selectedMapPath);
        }
        // Don't load default map on quick start
        // Just leave it empty (no background)
        
        // Hide the map renderer if no map is loaded
        if (mapRenderer != null && mapRenderer.sprite == null)
        {
            mapRenderer.enabled = false;
        }
    }

    void CreateMapRenderer()
    {
        GameObject mapObject = new GameObject("Map");
        mapObject.transform.parent = transform;
        mapRenderer = mapObject.AddComponent<SpriteRenderer>();
        mapRenderer.transform.position = new Vector3(0, 0, 5f); // Much further back
        
        // Explicitly set sorting
        mapRenderer.sortingLayerName = "Map";
        mapRenderer.sortingOrder = 0;
        
        Debug.Log($"Map renderer created - Sorting Layer: {mapRenderer.sortingLayerName}, Order: {mapRenderer.sortingOrder}");
    }
    public void LoadDefaultMap()
    {
        Debug.Log("Loading default map...");
        
        // Create a textured background
        int texWidth = 512;
        int texHeight = 512;
        Texture2D texture = new Texture2D(texWidth, texHeight);
        
        // Create a checkered pattern or solid color
        Color color1 = new Color(0.25f, 0.35f, 0.45f); // Dark blue-gray
        Color color2 = new Color(0.3f, 0.4f, 0.5f);    // Slightly lighter
        
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                // Checkered pattern (optional)
                int checkSize = 64;
                bool checker = ((x / checkSize) + (y / checkSize)) % 2 == 0;
                texture.SetPixel(x, y, checker ? color1 : color2);
            }
        }
        texture.Apply();
        
        ApplyTextureAsMap(texture, "Default Map");
    }

    public void LoadMapFromPath(string path)
    {
        Debug.Log($"Attempting to load map from: {path}");
        
        // Check if file exists
        if (!File.Exists(path))
        {
            Debug.LogError($"Map file not found: {path}");
            LoadDefaultMap();
            return;
        }
        
        // Check file extension
        string ext = Path.GetExtension(path).ToLower();
        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
        {
            Debug.LogError($"Unsupported file format: {ext}");
            LoadDefaultMap();
            return;
        }

        // Load image
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        
        if (texture.LoadImage(fileData))
        {
            Debug.Log($"Map loaded successfully! Size: {texture.width}x{texture.height}");
            ApplyTextureAsMap(texture, Path.GetFileName(path));
        }
        else
        {
            Debug.LogError("Failed to load image data");
            LoadDefaultMap();
        }
    }

    void ApplyTextureAsMap(Texture2D texture, string mapName)
    {
        currentTexture = texture;
        
        Sprite mapSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerGridCell
        );

        if (mapRenderer != null)
        {
            mapRenderer.sprite = mapSprite;
            mapRenderer.enabled = true;
            
            Color c = mapRenderer.color;
            c.a = 1f;
            mapRenderer.color = c;
            
            // Make SURE it's on the Map layer
            mapRenderer.sortingLayerName = "Map";
            mapRenderer.sortingOrder = 0;
            
            if (scaleToGrid && gridManager != null)
            {
                ScaleMapToGrid();
            }
            
            // Force grid to recreate on top
            if (gridManager != null)
            {
                gridManager.RecreateGrid();
            }
            
            Debug.Log($"Map '{mapName}' applied - Layer: {mapRenderer.sortingLayerName}");
        }

        currentMap = mapSprite;
    }

    void ScaleMapToGrid()
    {
        if (mapRenderer == null || gridManager == null) return;
        
        // Get grid dimensions
        float gridWidth = gridManager.GetGridWidth();
        float gridHeight = gridManager.GetGridHeight();
        float cellSize = gridManager.GetCellSize();
        
        // Calculate map size in world units
        float mapWorldWidth = currentTexture.width / pixelsPerGridCell;
        float mapWorldHeight = currentTexture.height / pixelsPerGridCell;
        
        // Option 1: Stretch to fill entire grid
        float scaleX = (gridWidth * cellSize) / mapWorldWidth;
        float scaleY = (gridHeight * cellSize) / mapWorldHeight;
        
        mapRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        
        // Center the map on the grid
        Vector2 gridOffset = gridManager.GetGridOffset();
        float centerX = (gridWidth * cellSize / 2f) + gridOffset.x;
        float centerY = (gridHeight * cellSize / 2f) + gridOffset.y;
        mapRenderer.transform.position = new Vector3(centerX, centerY, 0);
    }

    // Public method to load a map at runtime
    public void LoadMapFromFilePicker()
    {
        // For now, you can call this with a hardcoded path for testing
        // Later you can add a file picker UI
        
        #if UNITY_EDITOR
        string testPath = UnityEditor.EditorUtility.OpenFilePanel("Select Map Image", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(testPath))
        {
            LoadMapFromPath(testPath);
        }
        #endif
    }

    
}