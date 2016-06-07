using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	#region Public Fields
	public GameObject carryOverPlayer;
	public AudioClip ATrack, BTrack, Victory, Failure;
	[Range(0,1)]
	public float musicMasterVolume = 1;
	public bool useLastLevel;
	public bool carryMusicIntoMenu;

	#endregion

	#region Private Fields

	bool ab, eol;
	SlowFloat AVolume, BVolume;
	AudioSource[] sources;

	#endregion

	#region Public Properties

	public AudioSource SpareAudioSource { get { return sources[3]; } }

	#endregion

	#region Private Methods

	void Awake()
	{
		sources = GetComponents<AudioSource>();
	}

	void Start()
	{

		AVolume = new SlowFloat();
		BVolume = new SlowFloat();
		AVolume.Value = 1;
		sources[0].clip = ATrack;
		sources[1].clip = BTrack;
		sources[0].Play();
		sources[1].Play();

	}

	void Update()
	{
		if (sources[0].isPlaying)
			sources[1].time = sources[0].time;

		sources[0].volume = AVolume * musicMasterVolume;
		sources[1].volume = BVolume * musicMasterVolume;
		sources[2].volume = musicMasterVolume;

		if (!eol)
			if (!sources[2].isPlaying)
			{
				if (GameState.EndOfLevel)
				{
					AVolume.Value = 0;
					BVolume.Value = 0;
					sources[2].clip = GameState.EndOfLevelPassed ?
						Victory :
						Failure;
					sources[2].Play();
				}
				else if (!ab && GameState.InEncounter)
				{
					AVolume.Value = 0;
					BVolume.Value = 1;
					ab = true;
				}
				else if (ab && !GameState.InEncounter)
				{
					AVolume.Value = 1;
					BVolume.Value = 0;
					ab = false;
				}
			}
			else
			{
				eol = true;
			}
	}

	void OnDestroy()
	{
		var newb = Instantiate(carryOverPlayer);
		DontDestroyOnLoad(newb);
		if (carryMusicIntoMenu)
			newb.GetComponent<AudioSource>().clip = ATrack;
		newb.GetComponent<AudioSource>().Play();
	}


	#endregion

}