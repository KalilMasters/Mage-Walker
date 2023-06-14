using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Audio : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource audioSrc;
    static float temp;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioLevel(0.5f);
    }
    public void sound(AudioClip x)
    {
        //Debug.Log("Played");
        audioSrc.PlayOneShot(x, temp);
    }

    public void audioLevel(float slider)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10 (slider) * 20);
        mixer.SetFloat("Sfx", Mathf.Log10(slider) * 20);
        if (SceneManager.GetActiveScene().name != "MichaelTest") 
            temp = slider; 
        if (SceneManager.GetActiveScene().name == "MichaelTest")
        {
            temp = slider;
            mixer.SetFloat("MusicVol", Mathf.Log10(temp) * 20);
            mixer.SetFloat("Sfx", Mathf.Log10(temp) * 20);
        }
    }
}
