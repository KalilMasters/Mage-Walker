using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    public AudioClip button;
    Audio adio;
    [SerializeField] GameObject[] Canvases;
    [SerializeField] SceneNames playScene;
    [SerializeField] GameObject[] Models;
    // Start is called before the first frame update
    void Start()
    {
        adio = FindObjectOfType<Audio>();
        ResetScreen();
        MainScene();
    }
    public void ResetScreen()
    {
        ResetCanvases();
        ResetModels();
    }
    public void ResetCanvases()
    {
        foreach(GameObject x in Canvases)
        {
            x.SetActive(false);
        }
    }
    public void ResetModels()
    {
        foreach(GameObject x in Models)
        { 
            x.SetActive(false); 
        }
    }
    public void MainScene()
    {
        Canvases[0].SetActive(true);
        Models[0].SetActive(true);
    }
    public void ReturnButton(int previousCanvasNumber)
    {
        adio.PlaySound(button);
        ResetScreen();
        if(previousCanvasNumber == 0)
        {
            MainScene();
        }
        else
        {
            Canvases[previousCanvasNumber].SetActive(true);
        }
    }
    public void PlayButton() // Goes to the primary ability screen
    {
        adio.PlaySound(button);
        ResetScreen();
        Canvases[1].SetActive(true);
    }
    public void NextButton(int nextCanvasNumber)
    {
        adio.PlaySound(button);
        ResetScreen();
        Canvases[nextCanvasNumber].SetActive(true);
        if(nextCanvasNumber == 3)
            Models[1].SetActive(true);
    }
    public void SetttingsButton()
    {
        adio.PlaySound(button);
        ResetScreen();
        Canvases[4].SetActive(true);
    }
    public void PlayGame(bool IsHardMode)
    {
        adio.PlaySound(button);
        // Load scene with Hardcore mode off
        MapManager.IsHardMode = IsHardMode;
        SceneManager.LoadScene(playScene.ToString());
    }
}

public enum SceneNames { StartScene, GameplayScene , MichaelTest, KalilTest, GageTest }