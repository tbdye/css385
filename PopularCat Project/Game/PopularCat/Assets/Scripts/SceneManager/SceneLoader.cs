using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
	public Blackout blackout;
	public string mSceneToLoad;
	public Button okayButton;
	public Button resetButton;
	public Button menuButton;

	AsyncOperation loadingOperation;
	string nextLevel;

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
		loadingOperation = SceneManager.LoadSceneAsync(level);
		nextLevel = level;
		Blackout.Activate();
		blackout.activeAlpha = 1;
		GameState.LoadingScene = true;
		foreach(var sr in FindObjectsOfType<SpriteRenderer>())
		{
			sr.sortingLayerName = "Default";
		}
		FindObjectOfType<Player>().GetComponent<SpriteRenderer>().sortingLayerName = "Blackout";
		FindObjectOfType<Blackout>().GetComponent<SpriteRenderer>().sortingLayerName = "Blackout";
		foreach (var ui in FindObjectsOfType<MaskableGraphic>())
		{
			ui.color = Color.clear;
		}
		SequenceManager.StopSequences();
		SequenceDebugger.End();
	}

	void Update()
	{
		if(loadingOperation != null && loadingOperation.isDone)
		{
			FirstGameManager.GameState.SetCurrentLevel(nextLevel);
		}

	}

	#region Button Service Functions
	/// <summary>
	/// NewGameService
	/// </summary>
	public void OkayService()
	{
		LoadScene(mSceneToLoad);
	}

	public void ResetService()
	{
		var scene = SceneManager.GetActiveScene();
		LoadScene(scene.name);
	}

	public void MenuService()
	{
		LoadScene("Level0");
	}
	public void Quit()
	{
		Application.Quit();
	}
	#endregion
}