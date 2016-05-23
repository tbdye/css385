using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public string mSceneToLoad;
	public Button okayButton;
	public Button resetButton;
	public Button menuButton;

	// Use this for initialization
	void Start ()
	{
		// Create listeners
		okayButton.onClick.AddListener(OkayService);
		if(resetButton != null)
			resetButton.onClick.AddListener(ResetService);
		if (menuButton != null)
			menuButton.onClick.AddListener(MenuService);
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
	void OkayService()
	{
		LoadScene(mSceneToLoad);
	}

	void ResetService()
	{
		var scene = SceneManager.GetActiveScene();
		LoadScene(scene.name);
	}

	void MenuService()
	{
		LoadScene("MainMenu");
	}
	#endregion
}