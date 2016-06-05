using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    AudioSource music;

    // Initializes before Start()
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        music = GetComponent<AudioSource>();

        if (!music.isPlaying)
        {
            music.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            Destroy(gameObject);
        }
    }
}