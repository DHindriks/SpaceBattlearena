using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCore : MonoBehaviour {

    public bool Destroyed = false;
    public bool IsMain;
    public BossManager manager;
    [SerializeField]
    Material destroyedMat;

    void Damage()
    {
        Destroyed = true;
        if (IsMain)
        {
            manager.Die();
        }
        this.GetComponent<Renderer>().material = destroyedMat;
    }
}
