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
    public static ScoreSystem Instance;
    // Start is called before the first frame update
    void Start()
    {
        GameStart = false;
        Score = 0;
        ScoreText.text = "Score: " + Score.ToString();
    }
    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (MapManager.Instance.IsScrolling)
        {
            if (Timer >= 1) // Passively gain points
            {
                AddPoints(PointsPerSecond);
                Timer -= 1;
                ScoreText.text = "Score: " + Score.ToString();
            }
            else
            {
                Timer += Time.deltaTime;
            }
        }
    }
    public float GetScore()
    { return Score; }
    public void AddPoints(float points)
    {
        Score += points;
    }
}
