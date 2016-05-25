using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadMenu : MonoBehaviour
{
    public string LevelName = null;

    public Button mPlayButton;
    public Button mQuitButton;
    public Button mCreditsButton;

    /// <summary>
    /// Start
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        // Buttons are bound to UI through Unity3d MenuLevelManager object.

        // Create listeners
        mPlayButton.onClick.AddListener(PlayService);
        mQuitButton.onClick.AddListener(QuitService);
        mCreditsButton.onClick.AddListener(CreditsService);
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
    /// NewGameService
    /// </summary>
    private void PlayService()
    {
        LoadScene("Rules");
    }

    /// <summary>
    /// QuitService
    /// </summary>
    private void QuitService()
    {
        Application.Quit();
    }

    /// <summary>
    /// CreditsService
    /// </summary>
    private void CreditsService()
    {
        LoadScene("Credits");
    }
    #endregion
}