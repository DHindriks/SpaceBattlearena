using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void SubmitScore(Text text)
    {
        PlayerPrefs.SetString("HighScoreName", text.text);
        PlayerPrefs.SetInt("HighScore", GameObject.FindWithTag("player").GetComponent<Player>().Score);
        SceneManager.LoadScene("MainMenu");
    }
    
    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
