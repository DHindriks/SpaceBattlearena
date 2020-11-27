using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{

    [SerializeField]
    bool DebugActivate = false;

    [SerializeField]
    string name;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject CamObject;

    public GameObject Prefab;
    [SerializeField]
    int MaxShips; //max amount of ships that THIS SPAWNER can spawn

    GameObject Target;

    private void Update()
    {
        if (DebugActivate == true)
        {
            Spawn();
            DebugActivate = false;
        }
    }

    public void Spawn()
    {
        if (Target != null)
        {

            if (GameObject.FindWithTag("player") != null)
            {
                GameObject gameObject = Instantiate(Prefab);
                gameObject.GetComponent<Boss>().SetTarget(Target);
                if (PlayerPrefs.GetString("SurvivingBoss") != null)
                {
                    name = PlayerPrefs.GetString("SurvivingBoss");
                }
                gameObject.GetComponent<Boss>().EnemyName = name;
                gameObject.GetComponent<BossIntro>().SetCam(CamObject);
                gameObject.GetComponent<BossIntro>().SetCanvas(canvas);
                gameObject.transform.position = transform.position;
                gameObject.transform.rotation = transform.rotation;
            }


        }
        else
        {
            Target = GameObject.FindWithTag("player");
        }
    }
}
