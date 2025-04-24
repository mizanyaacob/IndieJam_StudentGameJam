using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---------Audio Source-------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------Scene and BGM-------")]
    [SerializeField] private List<SceneMusicData> sceneMusicList = new List<SceneMusicData>();


    [Header("---------SFX-------")]
   
    
    public AudioClip GameEnd;
    public AudioClip UpcomingEvent;
    public AudioClip Explosion;
    public AudioClip AngryAunty;
    public AudioClip Stun;
    public AudioClip Punch;
    public AudioClip Dash;
    public AudioClip GetHit;
    public AudioClip Eat;
    public AudioClip Eat3sec;
    public AudioClip Countdown;
    public AudioClip CameraCapture;

    [Serializable]
    public class SceneMusicData
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        AudioClip sceneClip = GetClipForScene(currentScene);
        if (sceneClip != null)
        {
            PlayMusic(sceneClip);
        }
    }

    private AudioClip GetClipForScene(string sceneName)
    {
        foreach (var data in sceneMusicList)
        {
            if (data.sceneName == sceneName)
                return data.musicClip;
        }
        return null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
