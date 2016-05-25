using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RulesScreen : MonoBehaviour
{
    public Button mContinue;
    public Button mBack;

    // Use this for initialization
    void Start()
    {
        // Buttons are bound to UI through Unity3d MenuLevelManager object.

        // Create listeners
        mContinue.onClick.AddListener(ContinueService);
        mBack.onClick.AddListener(BackService);
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
    /// ContinueService
    /// </summary>
    private void ContinueService()
    {
        LoadScene("Level1");
    }

    /// <summary>
    /// BackService
    /// </summary>
    private void BackService()
    {
        LoadScene("MainMenu");
    }
    #endregion
}