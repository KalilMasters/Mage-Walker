using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    Audio adio;
    public AudioClip button;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject Settings;
    // Start is called before the first frame update
    void Start()
    {
        adio = FindObjectOfType<Audio>();
        pauseMenu.SetActive(false);
        Settings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PauseButton()
    {
        Debug.Log("Pause");
        adio.sound(button);
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
    }
    public void ResumeButton() 
    {
        adio.sound(button);
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
    }
    public void ClickSettings()
    {
        adio.sound(button);
        pauseMenu.SetActive(false);
        Settings.SetActive(true);
    }
    public void ClickBack()
    {
        adio.sound(button);
        Settings.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void ClickExit()
    {
        adio.sound(button);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
