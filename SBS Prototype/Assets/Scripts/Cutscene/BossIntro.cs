using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntro : MonoBehaviour {
    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject CamObject;
    GameObject CamPivot;

    [SerializeField]
    Transform camPos;

    Transform OriginalPos;

    GameObject player;
    DialogueManager dialogueManager;
    string BossName;

    public void SetCanvas(GameObject Canvas)
    {
        canvas = Canvas;
    }

    public void SetCam(GameObject cam)
    {
        CamObject = cam;
        CamPivot = cam.transform.root.gameObject;
    }

    // Use this for initialization
    void Awake () {
        player = GameObject.FindWithTag("player");
        BossName = gameObject.GetComponent<Boss>().EnemyName;
        dialogueManager = GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>();
        Invoke("Phase1", 0.5f);
    }

    void Phase1()
    {
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<Player>().Controllable = false;
        player.GetComponent<Player>().Invincible = true;
        dialogueManager.AddDialogue(GetIntro(), Characters.BossPilot);
        canvas.SetActive(false);
        OriginalPos = CamObject.transform;
        CamObject.transform.SetParent(camPos);
        CamObject.transform.position = camPos.position;
        CamObject.transform.rotation = camPos.rotation;
        Invoke("Phase2", 4);
    }

    void Phase2()
    {
        RestoreCam();
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Player>().Controllable = true;
        player.GetComponent<Player>().Invincible = false;
        canvas.SetActive(true);
    }

    void RestoreCam()
    {
        CamObject.transform.parent = CamPivot.transform;
        CamObject.transform.position = CamPivot.transform.GetChild(0).position;
        CamObject.transform.rotation = CamPivot.transform.GetChild(0).rotation;
    }

    void RestoreTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            CamObject.transform.SetParent(null);
            CamObject.transform.LookAt(Vector3.Lerp(other.transform.position, transform.position, 0.5f));
            Invoke("RestoreTime", 1);
            Invoke("RestoreCam", 1);
        }
    }

    string GetIntro()
    {
        if (BossName == PlayerPrefs.GetString("SurvivingBoss"))
        {
            return "I'm BAAACK!";
        }else
        {
            int randomdialogue = Random.Range(0, 2);
            switch (randomdialogue)
            {
                case 0:
                    return "So you've been ruining our plans, I " + BossName + " Shall take you down!";
                case 1:
                    return "I am here. Prepare to die.";
                case 2:
                    return BossName + " here, bring it on!";
            }
        }
        return "...";
    }
}
