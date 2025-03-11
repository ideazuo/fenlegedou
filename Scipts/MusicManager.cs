using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    //“Ù–ß
    public AudioClip clickMusicClip;
    public AudioClip destroyMusicClip;
    public AudioClip winGameMusicClip;
    public AudioClip refreshMusicClip;


    private AudioSource audioSource;





    public static MusicManager _instance;

    private void Awake()
    {
        _instance = this;

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void OnPlayClickMusic()
    {
        audioSource.PlayOneShot(clickMusicClip);
    }

    public void OnPlayDestroyMusic()
    {
        audioSource.PlayOneShot(destroyMusicClip);
    }

    public void OnPlayWinGame()
    {
        audioSource.PlayOneShot(winGameMusicClip);
    }

    public void OnPlayRefreshCard()
    {
        audioSource.PlayOneShot(refreshMusicClip);
    }
}
