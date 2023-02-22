using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public AudioMixer mixer;
     AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void sound(AudioClip x)
    {
        audioSrc.PlayOneShot(x);
        Debug.Log("Played");
    }
    public void audioLevel(float slider)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10 (slider) * 20);
        mixer.SetFloat("Sfx", Mathf.Log10(slider) * 20);
    }
}
