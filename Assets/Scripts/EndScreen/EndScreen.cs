using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject EndScreenBackground;
    [SerializeField] GameObject CanvasUI;
    [SerializeField] GameObject CanvasAbility;
    [SerializeField] TMP_Text ScoreText;
    void Awake()
    {
        EndScreenBackground.SetActive(false);
    }

    public void Return()
    {
        SceneManager.LoadScene(SceneNames.StartScene.ToString());
    }
    public void Replay()
    {
        SceneManager.LoadScene(SceneNames.GameplayScene.ToString());
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
