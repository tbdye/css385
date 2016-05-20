using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public string mSceneToLoad;
    public Button mOkayButton;

    // Use this for initialization
    void Start ()
    {
        // Create listeners
        mOkayButton.onClick.AddListener(OkayService);
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
    private void OkayService()
    {
        LoadScene(mSceneToLoad);
    }
    #endregion
}