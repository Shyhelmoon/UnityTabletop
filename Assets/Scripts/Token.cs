using UnityEngine;

public class Token : MonoBehaviour
{
    [Header("Token Properties")]
    [SerializeField] private string tokenName = "Token";
    [SerializeField] private Color tokenColor = Color.blue;
    [SerializeField] private TokenSize tokenSize = TokenSize.Medium;
    
    [Header("References")]
    private GridManager gridManager;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    
    [Header("Selection")]
    private bool isSelected = false;
    private GameObject selectionRing;
    
    [Header("Movement")]
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    private Color normalColor;

    public enum TokenSize
    {
        Small,   // 0.5 cells
        Medium,  // 1 cell
        Large    // 2 cells
    }

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = tokenColor;
            normalColor = tokenColor;
            spriteRenderer.sortingLayerName = "Tokens";
            spriteRenderer.sortingOrder = 0;
        }
        
        UpdateTokenSize();
        CreateSelectionRing();
        
        // Snap to grid on start
        if (gridManager != null)
        {
            transform.position = gridManager.SnapToGrid(transform.position);
        }
        
        // Register with TokenManager
        TokenManager manager = FindObjectOfType<TokenManager>();
        if (manager != null)
        {
            manager.RegisterToken(this);
        }
    }

    void Update()
    {
        // Delete key to remove token
        if (isSelected && Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyToken();
        }
    }

    void UpdateTokenSize()
    {
        float scale = 0.8f; // Base scale
        
        switch (tokenSize)
        {
            case TokenSize.Small:
                scale = 0.5f;  // Changed from 0.4f
                break;
            case TokenSize.Medium:
                scale = 0.9f;  // Changed from 0.8f - fills ~90% of cell
                break;
            case TokenSize.Large:
                scale = 1.8f;  // Changed from 1.6f
                break;
        }
        
        transform.localScale = new Vector3(scale, scale, 1f);
        
        // Update collider size
        if (circleCollider != null)
        {
            circleCollider.radius = 0.5f;
        }
    }

    void CreateSelectionRing()
    {
        // Create a ring sprite to show selection
        selectionRing = new GameObject("SelectionRing");
        selectionRing.transform.parent = transform;
        selectionRing.transform.localPosition = Vector3.zero;
        
        SpriteRenderer ringRenderer = selectionRing.AddComponent<SpriteRenderer>();
        
        // Create a circle sprite for the ring
        Texture2D ringTexture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                float dx = x - 64f;
                float dy = y - 64f;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                // Create ring (outer circle - inner circle)
                if (distance > 58f && distance < 64f)
                {
                    pixels[y * 128 + x] = selectedColor;
                }
                else
                {
                    pixels[y * 128 + x] = Color.clear;
                }
            }
        }
        
        ringTexture.SetPixels(pixels);
        ringTexture.Apply();
        
        Sprite ringSprite = Sprite.Create(
            ringTexture,
            new Rect(0, 0, 128, 128),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        ringRenderer.sprite = ringSprite;
        ringRenderer.sortingLayerName = "Tokens";
        ringRenderer.sortingOrder = -1; // Behind token
        
        selectionRing.SetActive(false);
    }

    void OnMouseDown()
    {
        // Check if clicking on UI
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        
        isDragging = true;
        originalPosition = transform.position;
        
        Vector3 mousePos = GetMouseWorldPosition();
        dragOffset = transform.position - mousePos;
        
        SetSelected(true);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = mousePos + dragOffset;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            
            // Snap to grid
            if (gridManager != null)
            {
                Vector3 snappedPos = gridManager.SnapToGrid(transform.position);
                Vector2Int gridPos = gridManager.WorldToGrid(snappedPos);
                
                if (gridManager.IsValidGridPosition(gridPos))
                {
                    transform.position = snappedPos;
                }
                else
                {
                    transform.position = originalPosition;
                }
            }
            
            // Reset color based on selection
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isSelected ? normalColor : normalColor;
            }
        }
    }

    void OnMouseEnter()
    {
        if (!isDragging && spriteRenderer != null && !isSelected)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        if (!isDragging && spriteRenderer != null && !isSelected)
        {
            spriteRenderer.color = normalColor;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectionRing != null)
        {
            selectionRing.SetActive(selected);
        }
        
        if (spriteRenderer != null && !isDragging)
        {
            spriteRenderer.color = selected ? normalColor : normalColor;
        }
        
        // Notify TokenManager
        if (selected)
        {
            TokenManager manager = FindObjectOfType<TokenManager>();
            if (manager != null)
            {
                manager.SelectToken(this);
            }
        }
    }

    public void DestroyToken()
    {
        // Unregister from TokenManager
        TokenManager manager = FindObjectOfType<TokenManager>();
        if (manager != null)
        {
            manager.UnregisterToken(this);
        }
        
        Destroy(gameObject);
    }

    // Public getters/setters
    public void SetColor(Color color)
    {
        tokenColor = color;
        normalColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public void SetSize(TokenSize size)
    {
        tokenSize = size;
        UpdateTokenSize();
    }

    public void SetName(string name)
    {
        tokenName = name;
        gameObject.name = name;
    }

    public string GetTokenName() => tokenName;
    public Color GetTokenColor() => tokenColor;
    public bool IsSelected() => isSelected;
}