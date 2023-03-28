using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject[] Canvases;
    // Start is called before the first frame update
    void Start()
    {
        ResetCanvases();
        Canvases[0].SetActive(true);
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
    public void PlayGame(bool IsHardMode)
    {
        // Load scene with Hardcore mode off
        MapManager.isHardMode = IsHardMode;
        SceneManager.LoadScene(SceneNames.GameplayScene.ToString());
    }
}

public enum SceneNames { StartScene, GameplayScene }