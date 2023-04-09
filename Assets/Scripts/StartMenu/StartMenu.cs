using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    public AudioClip button;
    Audio adio;
    [SerializeField] GameObject[] Canvases;
    [SerializeField] SceneNames playScene;
    // Start is called before the first frame update
    void Start()
    {
        adio = FindObjectOfType<Audio>();
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
        adio.sound(button);
        ResetCanvases();
        Canvases[0].SetActive(true);
    }
    public void PlayButton()
    {
        adio.sound(button);
        ResetCanvases();
        Canvases[1].SetActive(true);
    }
    public void SetttingsButton()
    {
        adio.sound(button);
        ResetCanvases();
        Canvases[2].SetActive(true);
    }
    public void PlayGame(bool IsHardMode)
    {
        adio.sound(button);
        // Load scene with Hardcore mode off
        MapManager.IsHardMode = IsHardMode;
        SceneManager.LoadScene(playScene.ToString());
    }
}

public enum SceneNames { StartScene, GameplayScene , MichaelTest, KalilTest, GageTest }