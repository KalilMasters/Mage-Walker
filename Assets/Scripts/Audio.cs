using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    public void sound(AudioClip x)
    {
        //Debug.Log("Played");
        audioSrc.PlayOneShot(x, .15f);
    }
    public void sound(AudioClip x, bool looop)
    {
        if (looop)
        {
            audioSrc.loop = true;
            //Debug.Log("Played");
            audioSrc.PlayOneShot(x, .15f);
        }
    }
    public void audioLevel(float slider)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10 (slider) * 20);
        mixer.SetFloat("Sfx", Mathf.Log10(slider) * 20);
    }
}
