using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	#region Public Fields

	public AudioClip ATrack, BTrack, Victory, Failure;

	#endregion

	#region Private Fields

	static MusicManager instance;
	bool ab;
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
		sources[1].time = sources[0].time;
		sources[0].volume = AVolume;
		sources[1].volume = BVolume;

		if(!sources[2].isPlaying)
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

	#endregion
}