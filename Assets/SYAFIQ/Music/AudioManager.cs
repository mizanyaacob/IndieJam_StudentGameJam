using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------Audio Source-------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    [Header("---------Audio Clip-------")]
    public AudioClip GameEnd;
    public AudioClip UpcomingEvent;
    public AudioClip Explosion;
    public AudioClip AngryAunty;
    public AudioClip Stun;
    public AudioClip Punch;
    public AudioClip GetHit;
    public AudioClip Eat;
    public AudioClip Countdown;
    public AudioClip CameraCapture;
}
