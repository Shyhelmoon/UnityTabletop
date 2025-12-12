using UnityEngine;
using System.Collections.Generic;

public class TokenManager : MonoBehaviour
{
    [Header("Token Prefab")]
    [SerializeField] private GameObject tokenPrefab;
    
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    
    [Header("Spawn Settings")]
    [SerializeField] private Color defaultTokenColor = Color.blue;
    [SerializeField] private Token.TokenSize defaultTokenSize = Token.TokenSize.Medium;
    
    private List<Token> allTokens = new List<Token>();
    private Token selectedToken = null;

    void Update()
    {
        // Deselect on click empty space
        if (Input.GetMouseButtonDown(0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // Check if we clicked on nothing
                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero
                );
                
                if (hit.collider == null)
                {
                    DeselectAll();
                }
            }
        }
    }

    public void SpawnTokenAtMouse()
    {
        if (tokenPrefab == null || gridManager == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        SpawnToken(mousePos, defaultTokenColor, defaultTokenSize);
    }

    public void SpawnToken(Vector3 worldPosition, Color color, Token.TokenSize size)
    {
        if (tokenPrefab == null || gridManager == null) return;

        // Snap to grid
        Vector3 spawnPos = gridManager.SnapToGrid(worldPosition);
        Vector2Int gridPos = gridManager.WorldToGrid(spawnPos);

        // Check if valid position
        if (!gridManager.IsValidGridPosition(gridPos))
        {
            Debug.LogWarning("Cannot spawn token outside grid");
            return;
        }

        // Spawn token
        GameObject tokenObj = Instantiate(tokenPrefab, spawnPos, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        
        if (token != null)
        {
            token.SetColor(color);
            token.SetSize(size);
            token.SetName($"Token_{allTokens.Count + 1}");
        }
    }

    public void RegisterToken(Token token)
    {
        if (!allTokens.Contains(token))
        {
            allTokens.Add(token);
        }
    }

    public void UnregisterToken(Token token)
    {
        allTokens.Remove(token);
        if (selectedToken == token)
        {
            selectedToken = null;
        }
    }

    public void SelectToken(Token token)
    {
        // Deselect previous
        if (selectedToken != null && selectedToken != token)
        {
            selectedToken.SetSelected(false);
        }
        
        selectedToken = token;
    }

    public void DeselectAll()
    {
        if (selectedToken != null)
        {
            selectedToken.SetSelected(false);
            selectedToken = null;
        }
    }

    public void DeleteSelectedToken()
    {
        if (selectedToken != null)
        {
            selectedToken.DestroyToken();
            selectedToken = null;
        }
    }

    public void SetDefaultTokenColor(Color color)
    {
        defaultTokenColor = color;
    }

    public void SetDefaultTokenSize(Token.TokenSize size)
    {
        defaultTokenSize = size;
    }

    public Token GetSelectedToken() => selectedToken;
    public List<Token> GetAllTokens() => new List<Token>(allTokens);
}