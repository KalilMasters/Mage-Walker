using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndScreen : MonoBehaviour
{
    public AudioClip button;
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
        AudioManager.instance.PlaySound(button);
        SceneManager.LoadScene(SceneNames.StartScene.ToString());
    }
    public void Replay()
    {
        AudioManager.instance.PlaySound(button);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ActivateEndScreen()
    {
        EndScreenBackground.SetActive(true);
        CanvasUI.SetActive(false);
        CanvasAbility.SetActive(false);
        ScoreText.text = "SCORE:" + Mathf.FloorToInt(ScoreSystem.Instance.GameScore).ToString();
    }
    public void ActivateEndState()
    {
        MapScroller.Instance.SetScroll(false);
        StartCoroutine(DelayEndScreen());
		IEnumerator DelayEndScreen()
    	{
        	yield return new WaitForSeconds(3);
        	ActivateEndScreen();
    	}
    }
}
