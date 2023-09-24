using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class EndScreen : MonoBehaviour
{
    Audio adio;
    public AudioClip button;
    [SerializeField] GameObject EndScreenBackground;
    [SerializeField] GameObject CanvasUI;
    [SerializeField] GameObject CanvasAbility;
    [SerializeField] TMP_Text ScoreText;
    void Awake()
    {
        adio = FindObjectOfType<Audio>();
        EndScreenBackground.SetActive(false);
    }

    public void Return()
    {
        adio.sound(button);
        SceneManager.LoadScene(SceneNames.StartScene.ToString());
    }
    public void Replay()
    {
        adio.sound(button);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ActivateEndScreen()
    {
        MapManager.Instance.SetScroll(false);
        EndScreenBackground.SetActive(true);
        CanvasUI.SetActive(false);
        CanvasAbility.SetActive(false);
        ScoreText.text = "SCORE:" + ScoreSystem.Instance.GetScore().ToString();
    }
}
