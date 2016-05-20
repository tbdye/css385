using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RulesScreen : MonoBehaviour
{
    public Button mButton;

    // Use this for initialization
    void Start()
    {
        // Buttons are bound to UI through Unity3d MenuLevelManager object.

        // Create listeners
        mButton.onClick.AddListener(GameService);
    }

    /// <summary>
    /// LoadScene
    /// </summary>
    /// <param name="level"></param>
    void LoadScene(string level)
    {
        SceneManager.LoadScene(level);
        FirstGameManager.GameState.SetCurrentLevel(level);
    }

    #region Button Service Functions
    /// <summary>
    /// GameService
    /// </summary>
    private void GameService()
    {
        LoadScene("Level1");
    }
    #endregion
}