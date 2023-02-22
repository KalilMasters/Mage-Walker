using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] float Score;
    [SerializeField] float PointsPerSecond;
    [SerializeField] float Timer;
    [SerializeField] TMP_Text ScoreText;

    [SerializeField] bool GameStart;
    [SerializeField] MapManager map;
    // Start is called before the first frame update
    void Start()
    {
        GameStart = false;
        Score = 0;
        ScoreText.text = "Score: " + Score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameStart)
        //{
        if (map.IsScrolling)
        {
            if (Timer >= 1) // Passively gain points
            {
                Score += PointsPerSecond;
                Timer -= 1;
                ScoreText.text = "Score: " + Score.ToString();
            }
            else
            {
                Timer += Time.deltaTime;
            }
        }
            
        //}
    }
    public float GetScore()
    { return Score; }
}
