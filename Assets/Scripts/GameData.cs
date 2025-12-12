using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Session Info")]
    public bool isQuickStart = true;
    public bool isHost = true;
    public string sessionId = "";
    
    [Header("Map/Campaign")]
    public string selectedMapPath = "";
    public string campaignId = "";
    
    [Header("Player Info")]
    public string playerName = "Player";
    public Color playerColor = Color.blue;

    void Awake()
    {
        // Singleton pattern - only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetToDefaults()
    {
        isQuickStart = true;
        isHost = true;
        sessionId = "";
        selectedMapPath = "";
        campaignId = "";
    }
}