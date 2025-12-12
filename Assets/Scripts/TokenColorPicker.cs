using UnityEngine;
using UnityEngine.UI;

public class TokenColorPicker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TokenManager tokenManager;
    [SerializeField] private GameObject colorPickerPanel;
    
    [Header("Preset Colors")]
    [SerializeField] private Button[] colorButtons;
    
    private Color[] presetColors = new Color[]
    {
        new Color(0.2f, 0.4f, 1f),    // Blue
        new Color(1f, 0.3f, 0.3f),    // Red
        new Color(0.3f, 1f, 0.3f),    // Green
        new Color(1f, 1f, 0.3f),      // Yellow
        new Color(1f, 0.5f, 0f),      // Orange
        new Color(0.6f, 0.3f, 1f),    // Purple
        Color.white,                   // White
        new Color(0.2f, 0.2f, 0.2f)   // Dark Gray
    };

    void Start()
    {
        // Hide panel on start
        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(false);
        }
        
        // Setup color buttons
        for (int i = 0; i < colorButtons.Length && i < presetColors.Length; i++)
        {
            int index = i; // Capture for closure
            Color color = presetColors[i];
            
            // Set button color
            Image buttonImage = colorButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = color;
            }
            
            // Add click listener
            colorButtons[i].onClick.AddListener(() => SelectColor(color));
        }
    }

    public void ToggleColorPicker()
    {
        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(!colorPickerPanel.activeSelf);
        }
    }

    public void SelectColor(Color color)
    {
        if (tokenManager != null)
        {
            // Apply to selected token
            Token selectedToken = tokenManager.GetSelectedToken();
            if (selectedToken != null)
            {
                selectedToken.SetColor(color);
                Debug.Log($"Changed token color to {color}");
            }
            else
            {
                // Set as default for new tokens
                tokenManager.SetDefaultTokenColor(color);
                Debug.Log($"Set default token color to {color}");
            }
        }
        
        // Close panel after selection
        if (colorPickerPanel != null)
        {
            colorPickerPanel.SetActive(false);
        }
    }
}