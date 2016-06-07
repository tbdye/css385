using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
	public AudioSource music { get; set; }

	void Awake()
	{
		DontDestroyOnLoad(gameObject);

		music = GetComponent<AudioSource>();

	}

	void Update()
	{
		if (FindObjectOfType<MusicManager>() != null)
		{
			Destroy(gameObject);
			return;
		}
	}
}