using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class EndScreen : MonoBehaviour
{
    [SerializeField] ScoreSystem ScoreSys;
    [SerializeField] GameObject EndScreenBackground;
    [SerializeField] GameObject CanvasUI;
    [SerializeField] GameObject CanvasAbility;
    [SerializeField] TMP_Text ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        EndScreenBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
    public void Replay()
    {
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
