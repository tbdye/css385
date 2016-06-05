using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlobalGameManager : MonoBehaviour
{
    private string mCurrentLevel = "Level0";

    /// <summary>
    /// Start
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// SetCurrentLevel
    /// </summary>
    /// <param name="level"></param>
    public void SetCurrentLevel(string level)
    {
        mCurrentLevel = level;
    }
}