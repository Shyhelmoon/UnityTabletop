using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private TokenManager tokenManager;
    
    [Header("Buttons")]
    [SerializeField] private Button gridToggleButton;
    [SerializeField] private TextMeshProUGUI gridToggleText;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button loadMapButton;
    [SerializeField] private Button addTokenButton;
    [SerializeField] private Button deleteTokenButton;
    
    [Header("Settings")]
    private bool gridVisible = true;
    private bool tokenSpawnMode = false;

    void Start()
    {
        if (gridToggleButton != null)
            gridToggleButton.onClick.AddListener(ToggleGrid);
        
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        if (loadMapButton != null)
            loadMapButton.onClick.AddListener(LoadMap);
        
        if (addTokenButton != null)
            addTokenButton.onClick.AddListener(ToggleTokenSpawnMode);
        
        if (deleteTokenButton != null)
            deleteTokenButton.onClick.AddListener(DeleteSelectedToken);
        
        UpdateGridButtonText();
    }

    void Update()
    {
        // Spawn token on click when in spawn mode
        if (tokenSpawnMode && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                tokenManager.SpawnTokenAtMouse();
            }
        }
        
        // Escape to return to menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    public void ToggleGrid()
    {
        if (gridManager != null)
        {
            gridVisible = !gridVisible;
            gridManager.ToggleGrid();
            UpdateGridButtonText();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    void UpdateGridButtonText()
    {
        if (gridToggleText != null)
        {
            gridToggleText.text = gridVisible ? "Hide Grid" : "Show Grid";
        }
    }

    public void ReturnToMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMap()
    {
        if (mapManager != null)
        {
            mapManager.LoadMapFromFilePicker();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ToggleTokenSpawnMode()
    {
        tokenSpawnMode = !tokenSpawnMode;
        
        // Update button visual
        if (addTokenButton != null)
        {
            ColorBlock colors = addTokenButton.colors;
            colors.normalColor = tokenSpawnMode ? new Color(0f, 0.6f, 0f) : new Color(0f, 0.53f, 1f);
            addTokenButton.colors = colors;
        }
        
        Debug.Log(tokenSpawnMode ? "Token spawn mode ON - Click to place tokens" : "Token spawn mode OFF");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void DeleteSelectedToken()
    {
        if (tokenManager != null)
        {
            tokenManager.DeleteSelectedToken();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
}