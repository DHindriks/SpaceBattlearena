using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStation : MonoBehaviour {
    [SerializeField]
    Material ActiveMat;

    [SerializeField]
    Material DeactiveMat;

    [SerializeField]
    ParticleSystem ActiveParticle;

    [SerializeField]
    List<ParticleSystem> OverheatParticles;

    public bool Active;
    bool destroyed;
    public BossManager manager;

    void Start()
    {
        Active = true;
        destroyed = false;
    }

    void Damage(int amount)
    {
        Deactivate(manager.GetTime(15));
    }

    public void Deactivate(int seconds, bool overHeat = false, bool Permanent = false)
    {
        if (!destroyed)
        {
            CancelInvoke();
            Active = false;
            SetMat(DeactiveMat);
            ActiveParticle.Stop();
            if (!Permanent)
            {
                Invoke("Restore", seconds);
            }else if (Permanent)
            {
                destroyed = true;
            }
            if (overHeat)
            {
                foreach (ParticleSystem particle in OverheatParticles)
                {
                    particle.Play();
                }
            }else
            {
                manager.CheckStations();
            }

        }
    }

    void Restore()
    {
        SetMat(ActiveMat);
        Active = true;
        ActiveParticle.Play();
        manager.CheckStations();
        GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("A station has restarted, you need to be quick");
        foreach (ParticleSystem particle in OverheatParticles)
        {
            particle.Stop();
        }
    }

    void SetMat(Material Mat)
    {
        GetComponent<Renderer>().material = Mat;
    }

}
