using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    // NO quickStartPanel!
    [SerializeField] private GameObject importMapPanel;
    [SerializeField] private GameObject joinSessionPanel;
    [SerializeField] private GameObject campaignsPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Loading Screen (Optional)")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private Text loadingText;
    
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && buttonClickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        ShowMainMenu();
        
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    #region Menu Navigation

    public void ShowMainMenu()
    {
        PlayButtonSound();
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowImportMap()
    {
        PlayButtonSound();
        HideAllPanels();
        importMapPanel.SetActive(true);
    }

    public void ShowJoinSession()
    {
        PlayButtonSound();
        HideAllPanels();
        joinSessionPanel.SetActive(true);
    }

    public void ShowCampaigns()
    {
        PlayButtonSound();
        HideAllPanels();
        campaignsPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        PlayButtonSound();
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        // No quickStartPanel to hide!
        importMapPanel.SetActive(false);
        joinSessionPanel.SetActive(false);
        campaignsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    #endregion

    #region Scene Loading

    /// <summary>
    /// Quick start - goes directly to game scene
    /// Called by QuickStart button on MainMenu
    /// </summary>
    public void StartQuickGame()
    {
        Debug.Log("=== StartQuickGame called ===");
        PlayButtonSound();
        
        if (GameData.Instance != null)
        {
            GameData.Instance.ResetToDefaults();
            GameData.Instance.isQuickStart = true;
            Debug.Log("GameData set");
        }
        
        Debug.Log("About to load scene: " + gameSceneName);
        
        // Simple direct load - no async, no loading panel
        SceneManager.LoadScene(gameSceneName);
        
        Debug.Log("LoadScene called");
    }
    public void StartWithMap(string mapPath)
    {
        PlayButtonSound();
        
        GameData.Instance.selectedMapPath = mapPath;
        GameData.Instance.isQuickStart = false;
        
        LoadGameScene();
    }

    public void JoinSession(string sessionId)
    {
        PlayButtonSound();
        
        GameData.Instance.sessionId = sessionId;
        GameData.Instance.isHost = false;
        
        LoadGameScene();
    }

    public void LoadCampaign(string campaignId)
    {
        PlayButtonSound();
        
        GameData.Instance.campaignId = campaignId;
        
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        if (loadingPanel != null)
        {
            StartCoroutine(LoadSceneAsync(gameSceneName));
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        HideAllPanels();
        loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            if (loadingBar != null)
                loadingBar.value = progress;
                
            if (loadingText != null)
                loadingText.text = $"Loading... {progress * 100:0}%";

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    #endregion

    #region Utility

    public void QuitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    #endregion
}