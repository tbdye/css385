using UnityEngine;
using System.Collections;

public class FirstGameManager : MonoBehaviour
{
    #region Create GlobalGameManager
    // Will create the GlobalGameManager only once to persist over all scenes.
    private static GlobalGameManager sGameState = null;

    /// <summary>
    /// CreateGlobalManager
    /// </summary>
    private static void CreateGlobalManager()
    {
        GameObject newGameState = new GameObject();
        newGameState.name = "GlobalStateManager";
        newGameState.AddComponent<GlobalGameManager>();
        sGameState = newGameState.GetComponent<GlobalGameManager>();
    }

    /// <summary>
    /// GameState
    /// </summary>
    public static GlobalGameManager GameState
    {
        get
        {
            return sGameState;
        }
    }
    #endregion

    /// <summary>
    /// Awake
    /// Called regardless even if script components are not enabled.
    /// </summary>
    void Awake()
    {
        if (sGameState == null)
        {
            CreateGlobalManager();
        }
    }

    /// <summary>
    /// Start
    /// All objects from all levels should be able to access FirstGameManager.GameState
    /// </summary>
    void Start()
    {

    }
}