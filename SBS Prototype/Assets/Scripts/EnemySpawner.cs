using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject Prefab;
    [SerializeField]
    int MaxShips; //max amount of ships that THIS SPAWNER can spawn

    GameObject Target;
    List<GameObject> enemies = new List<GameObject>();

    // Use this for initialization
    void Start () {
        InvokeRepeating("Spawn", 0, 7.5f);
	}

    void Spawn()
    {
        if (Target != null)
        {
            if (enemies.Count < MaxShips)
            {
                if (GameObject.FindWithTag("player") != null)
                {
                    GameObject gameObject = Instantiate(Prefab);
                    gameObject.GetComponent<Enemy>().SetTarget(Target);
                    enemies.Add(gameObject);
                    gameObject.transform.position = transform.position;
                    gameObject.transform.rotation = transform.rotation;
                }

            }
            Checklist();
        }
        else
        {
            Target = GameObject.FindWithTag("player");
        }
    }

    //removes enemies who have been destroyed
    void Checklist()
    {
        foreach (GameObject enemy in enemies.ToArray())
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
