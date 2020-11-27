using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {
    [SerializeField]
    List<DamageStation> stations;

    [SerializeField]
    DamageCore coolingStation;

    [SerializeField]
    DamageCore MainCore;

    [SerializeField]
    GameObject shield;

    [SerializeField]
    GameObject Cutscene;
	// Use this for initialization
	void Start () {
		foreach (DamageStation station in stations)
        {
            station.manager = this;
        }
        coolingStation.manager = this;
        MainCore.manager = this;
        MainCore.IsMain = true;
    }
	
    public int GetTime(int Default)
    {
        if (coolingStation.Destroyed)
        {
            return Default + 10;
        }else
        {
            return Default;
        }
    }

    public void Die()
    {
        Cutscene.SetActive(true);
        shield.SetActive(false);
        foreach (DamageStation station in stations)
        {
            station.Deactivate(GetTime(20), true, true);
        }
    }

    public void CheckStations()
    {
        int inactiveCount = 0;
        foreach (DamageStation station in stations)
        {
            if (!station.Active)
            {
                inactiveCount++;
            }
        }

        if (inactiveCount == stations.Count)
        {
            foreach (DamageStation station in stations)
            {
                station.Deactivate(GetTime(20), true);
            }

            shield.SetActive(false);
            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("I'm getting huge readings over here, that shield should be down now.", Characters.Zero);
            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("Get in the station quickly, destroy the reactor!", Characters.Zero);
        }
        else
        {
            shield.SetActive(true);
        }


    }
}
