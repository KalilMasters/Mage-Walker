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
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] MapManager MapM;
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
        MapM.SetScroll(false);
        EndScreenBackground.SetActive(true);
        ScoreText.text = "SCORE:" + ScoreSys.GetScore().ToString();
    }
}
