using UnityEngine;
using UnityEngine.SceneManagement;
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

        EndScreenBackground.SetActive(true);
        CanvasUI.SetActive(false);
        CanvasAbility.SetActive(false);
        ScoreText.text = "SCORE:" + Mathf.FloorToInt(ScoreSystem.Instance.GameScore).ToString();
    }
    public void ActivateEndState()
    {
        MapManager.Instance.SetScroll(false);
        StartCoroutine(DelayEndScreen());
		IEnumerator DelayEndScreen()
    	{
        	yield return new WaitForSeconds(3);
        	ActivateEndScreen();
    	}
    }
}
