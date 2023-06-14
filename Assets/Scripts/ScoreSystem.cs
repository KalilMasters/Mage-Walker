using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private float _activeScore;
    public float GameScore { get { return (MapManager.Instance ? MapManager.Instance.PlayerScore : 0) + _activeScore; } }
    [SerializeField] float _pointsPerSecond;
    [SerializeField] float _timer;
    [SerializeField] TMP_Text _scoreText;

    [SerializeField] GameObject HardcoreModeText;

    public static ScoreSystem Instance;
        
    private void Awake()
    {
        Instance = this;

        _activeScore = 0;
        _scoreText.text = "Score: " + GameScore.ToString();

        if (HardcoreModeText != null)
            HardcoreModeText.SetActive(MapManager.IsHardMode);
    }

    void Update()
    {
        if (!MapScroller.Instance.IsScrolling) return;

        if (_timer < 1)
        {
            _timer += Time.deltaTime;
            return;
        }

        // Passively gain points
        AddPoints(_pointsPerSecond);
        _timer -= 1;
    }
    public void AddPoints(float points)
    {
        _activeScore += points;
        SetScoreText();
    }
    void SetScoreText()
    {
        if(_scoreText != null)
            _scoreText.SetText("Score: " + Mathf.FloorToInt(GameScore).ToString());
    }
}
