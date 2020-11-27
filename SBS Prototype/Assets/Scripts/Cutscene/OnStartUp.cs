using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnStartUp : MonoBehaviour {

    [SerializeField]
    GameObject gameManagerPrefab;

    [SerializeField]
    List<TextMesh> HighscoreTexts;

    float StartTime;

	// Use this for initialization
	void Start () {
		if (GameObject.FindWithTag("GameManager") == null)
        {
            Instantiate(gameManagerPrefab);
        }

        foreach(TextMesh text in HighscoreTexts)
        {
            text.text = "High Score" + Environment.NewLine + PlayerPrefs.GetString("HighScoreName") + Environment.NewLine + PlayerPrefs.GetInt("HighScore");
        }
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartTime = Time.time;
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (StartTime + 10 < Time.time)
            {
                PlayerPrefs.DeleteAll();
                Debug.LogWarning("Reset playerprefs");
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            StartTime = 0;
        }
    }
}
