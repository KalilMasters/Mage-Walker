using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    void Start()
    {
        CanvasEnabler.EnableCanvas("Pause",false);
        CanvasEnabler.EnableCanvas("Settings",false);
    }
    public void PauseButton()
    {
        Debug.Log("Pause");
        AudioManager.instance.PlaySound("button");
        Time.timeScale = 0.0f;
        CanvasEnabler.EnableCanvas("Pause", true);
    }
    public void ResumeButton() 
    {
        AudioManager.instance.PlaySound("button");
        Time.timeScale = 1.0f;
        CanvasEnabler.EnableCanvas("Pause", false);
    }
    public void ClickSettings()
    {
        AudioManager.instance.PlaySound("button");
        CanvasEnabler.EnableCanvas("Pause", false);
        CanvasEnabler.EnableCanvas("Settings", true);

    }
    public void ClickBack()
    {
        AudioManager.instance.PlaySound("button");

        CanvasEnabler.EnableCanvas("Pause", true);
        CanvasEnabler.EnableCanvas("Settings", false);
    }
    public void ClickExit()
    {
        AudioManager.instance.PlaySound("button");

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneNames.StartScene.ToString());
    }
}
