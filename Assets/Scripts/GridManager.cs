using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 50;
    [SerializeField] private int gridHeight = 50;
    [SerializeField] private float cellSize = 1f;
    
    [Header("Visual Settings")]
    [SerializeField] private bool showGrid = true;
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private float gridLineWidth = 0.05f; // Changed from 0.02
    
    [Header("Grid Offset")]
    [SerializeField] private Vector2 gridOffset = Vector2.zero;
    
    private GameObject gridLinesParent;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (showGrid)
        {
            CreateGridLines();
        }
    }

    void CreateGridLines()
    {
        gridLinesParent = new GameObject("GridLines");
        gridLinesParent.transform.parent = transform;
        gridLinesParent.transform.position = new Vector3(gridOffset.x, gridOffset.y, 0);

        // Create vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            CreateLine(
                new Vector3(x * cellSize, 0, 0),
                new Vector3(x * cellSize, gridHeight * cellSize, 0),
                gridLinesParent.transform
            );
        }

        // Create horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            CreateLine(
                new Vector3(0, y * cellSize, 0),
                new Vector3(gridWidth * cellSize, y * cellSize, 0),
                gridLinesParent.transform
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        start.z = -1f;  // In front of map
        end.z = -1f;
        
        GameObject lineObject = new GameObject("GridLine");
        lineObject.transform.parent = parent;
        lineObject.transform.position = Vector3.zero;
        lineObject.layer = 0; // Default layer
        
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        
        // Create material with Unlit/Color shader
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = gridColor;
        line.material = lineMaterial;
        
        line.startColor = gridColor;
        line.endColor = gridColor;
        
        line.widthMultiplier = gridLineWidth;
        line.widthCurve = AnimationCurve.Linear(0, 1, 1, 1);
        
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        
        // Try BOTH sorting layer AND sorting order
        line.sortingLayerName = "Grid";
        line.sortingOrder = 100; // Very high to ensure it's on top
        line.useWorldSpace = false;
        
        // Make sure it renders on top
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.alignment = LineAlignment.View; // Changed from TransformZ
        
        // Debug log
        Debug.Log($"Created grid line - Sorting Layer: {line.sortingLayerName}, Order: {line.sortingOrder}");
    }

    // Keep all your other methods (WorldToGrid, GridToWorld, etc.)
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOffset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - gridOffset.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridPosition.x * cellSize + cellSize / 2f + gridOffset.x;
        float y = gridPosition.y * cellSize + cellSize / 2f + gridOffset.y;
        return new Vector3(x, y, 0);
    }

    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);
        return GridToWorld(gridPos);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridWidth &&
               gridPosition.y >= 0 && gridPosition.y < gridHeight;
    }

    public void ToggleGrid()
    {
        showGrid = !showGrid;
        if (gridLinesParent != null)
        {
            gridLinesParent.SetActive(showGrid);
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = gridColor;
            
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = new Vector3(x * cellSize + gridOffset.x, gridOffset.y, 0);
                Vector3 end = new Vector3(x * cellSize + gridOffset.x, gridHeight * cellSize + gridOffset.y, 0);
                Gizmos.DrawLine(start, end);
            }
            
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 start = new Vector3(gridOffset.x, y * cellSize + gridOffset.y, 0);
                Vector3 end = new Vector3(gridWidth * cellSize + gridOffset.x, y * cellSize + gridOffset.y, 0);
                Gizmos.DrawLine(start, end);
            }
        }
    }

    public void RecreateGrid()
    {
        // Destroy old grid
        if (gridLinesParent != null)
        {
            Destroy(gridLinesParent);
        }
        
        // Create new grid
        if (showGrid)
        {
            CreateGridLines();
        }
    }
    
// Helpers
    public int GetGridWidth() { return gridWidth; }
    public int GetGridHeight() { return gridHeight; }
    public float GetCellSize() { return cellSize; }
    public Vector2 GetGridOffset() { return gridOffset; }
    }