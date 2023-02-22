using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject[] Canvases;
    // Start is called before the first frame update
    void Start()
    {
        ResetCanvases();
        Canvases[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetCanvases()
    {
        foreach(GameObject x in Canvases)
        {
            x.SetActive(false);
        }
    }
    public void ReturnButton()
    {
        ResetCanvases();
        Canvases[0].SetActive(true);
    }
    public void PlayButton()
    {
        ResetCanvases();
        Canvases[1].SetActive(true);
    }
    public void SetttingsButton()
    {
        ResetCanvases();
        Canvases[2].SetActive(true);
    }
    public void PlayNormalButton()
    {
        // Load scene with Hardcore mode off
    }
    public void PlayerHardcoreButton()
    {
        // Load scene with Hardcore mode on
    }
}
