using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MusicManager : MonoBehaviour {
	// This is a very basic way of doing music with linear crossfades
	// More flexible and robust examples of this would use some kind of 
	// coroutine tweeing library (DoTween or similar) and use easing curves

	public AudioClip[] musicClips;
	private AudioSource[] musicSources;
	public static MusicManager Instance;

	public float fadeTime = 1f;

	public int currentSceneIndex;

	public AudioSource currentMusicSource;
	
	
	void Awake() {
		//enforce singleton pattern
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
		
		DontDestroyOnLoad(gameObject);
	}
	
	
	// Use this for initialization
	void Start () {
		
		
		
		 
		//initialize the audio source
		musicSources = new AudioSource[musicClips.Length];

		for (int i = 0; i < musicClips.Length; i++) {
			//make a new game object, make it a child of this game object
			GameObject musicPlayer = new GameObject();
			musicPlayer.transform.parent = gameObject.transform;
			
			//add an audio source to this game object, make sure it loops,
			//then assign the corresponding music clip
			musicSources[i] = musicPlayer.AddComponent<AudioSource>();
			musicSources[i].loop = true;
			musicSources[i].clip = musicClips[i];
			
		}
		
		//for this demo, we have as many music clips
		//as scenes.  This won't work if you have fewer scenes
		//than music clip (or vice versa)!!!

		currentMusicSource = musicSources[0];
		currentMusicSource.Play();


	}
	

	void Update () {

		if (Input.GetKeyDown(KeyCode.Alpha1) && SceneManager.GetActiveScene().buildIndex != 1) {
			//go to the first level when we press 1 (if we're not there already)
			GoToFirstLevel();
		} else if (Input.GetKeyDown(KeyCode.Alpha2) && SceneManager.GetActiveScene().buildIndex != 2) {
			GoToItemShop();
		} else if (Input.GetKeyDown(KeyCode.Alpha3) && SceneManager.GetActiveScene().buildIndex != 1) {
			ExitItemShop();
		} else if (Input.GetKeyDown(KeyCode.Alpha4) && SceneManager.GetActiveScene().buildIndex != 0) {
			BackToMainMenu();
		} else if (Input.GetKeyDown(KeyCode.Alpha5) && SceneManager.GetActiveScene().buildIndex != 0) {
			//THE BAD SOUNDING WAY
			
			currentMusicSource.Stop();
			SceneManager.LoadScene(0);
			musicSources[0].Play();
			currentMusicSource = musicSources[0];
		}
		
	}


	public void FadeOut(AudioSource source, float t) {
		//just doing linear FadeOut for now - see Jack's easing curves, or DoTween, for other fading curves

		source.DOFade(0f, t);


	}

	public void FadeIn(AudioSource source, float t) {
		source.DOFade(1f, t);
	}

	public void PlaySong(int clipIndex) {
		musicSources[clipIndex].Play();
	}

	public void GoToFirstLevel() {
		//when we go to the first level, we want to fade out the currently playing music, then play the 
		//level 1 music
		FadeOut(currentMusicSource, fadeTime);
		SceneManager.LoadScene(1);
		
		//make sure the music source is stopped
		musicSources[1].Stop();
		musicSources[1].volume = 1.0f;
		musicSources[1].PlayDelayed(fadeTime);
		currentMusicSource = musicSources[1];

	}

	public void GoToItemShop() {
		//now we want to crossfade to the Item Shop Music from the main game music
		FadeOut(currentMusicSource, fadeTime);
		if (!musicSources[2].isPlaying) {
			musicSources[2].Play();
		}
		FadeIn(musicSources[2], fadeTime);
		currentMusicSource = musicSources[2];
		SceneManager.LoadScene(2);
	}

	public void ExitItemShop() {
		FadeOut(currentMusicSource, fadeTime);
		FadeIn(musicSources[1], fadeTime);
		if (!musicSources[1].isPlaying) {
			musicSources[1].Play();
		}
		currentMusicSource = musicSources[1];
		SceneManager.LoadScene(1);
	}

	public void BackToMainMenu() {
		FadeOut(currentMusicSource, fadeTime);
		SceneManager.LoadScene(0);
		musicSources[0].Stop();
		musicSources[0].volume = 1.0f;
		musicSources[0].PlayDelayed(fadeTime);

		currentMusicSource = musicSources[0];
	}
}
