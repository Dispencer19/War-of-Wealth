using UnityEngine;

public class BGMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public AudioSource musicSource;
    public AudioClip musicClip;
    void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

}
