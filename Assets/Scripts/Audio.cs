using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
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
        audioSrc.PlayOneShot(x, .15f);
        Debug.Log("Played");
    }
}
