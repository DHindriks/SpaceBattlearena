using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStationManager : MonoBehaviour {

    [SerializeField]
    int ActiveAmount;

    [SerializeField]
    List<HealStation> HealStations = new List<HealStation>();


    List<HealStation> Actives = new List<HealStation>();
    List<HealStation> InActives = new List<HealStation>();
	// Use this for initialization
	void Start () {
        Getstations();
        SetManager();
	}
	
    public void Getstations()
    {
        Actives.Clear();
        InActives.Clear();

        foreach (HealStation station in HealStations)
        {
            if (station.Active == true)
            {
                Actives.Add(station);
            }else if (station.Skip == false)
            {
                InActives.Add(station);
            }
        }
        if (Actives.Count < ActiveAmount)
        {
            PickNew();
        }
        ResetSkippables();
    }

    void PickNew()
    {
        int RandomStation = Random.Range(0, InActives.Count);
        HealStation pickedStation = InActives[RandomStation];
        pickedStation.ToggleActive();
        Getstations();
    }

    void ResetSkippables()
    {
        foreach (HealStation station in HealStations)
        {
            station.Skip = false;
        }
    }

    void SetManager()
    {
        foreach (HealStation station in HealStations)
        {
            station.Manager = this;
        }
    }
}
