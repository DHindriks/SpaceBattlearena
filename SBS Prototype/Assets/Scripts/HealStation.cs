using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStation : MonoBehaviour {

    public bool Active;
    public bool Skip;
    public HealStationManager Manager;
    [SerializeField]
    List<ParticleSystem> Particles = new List<ParticleSystem>();

    [SerializeField]
    GameObject HealEffect;

    void Start()
    {
        if (Active)
        {
            foreach (ParticleSystem Particle in Particles)
            {
                Particle.Play();
            }
        }
        else
        {
            foreach (ParticleSystem Particle in Particles)
            {
                Particle.Stop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ship" && Active || other.tag == "player" && Active)
        {
            GameObject HealParticle = Instantiate(HealEffect);
            HealParticle.transform.SetParent(other.transform);
            HealParticle.transform.localPosition = new Vector3(0, 0, 0);
            Instantiate(HealEffect);
            other.transform.root.SendMessage("Damage", -100f, SendMessageOptions.DontRequireReceiver);
            ToggleActive();
        }    
    }

    public void ToggleActive()
    {

        Active = !Active;

        if (Active)
        {
            foreach (ParticleSystem Particle in Particles)
            {
                Particle.Play();
            }
        } else
        {
            foreach (ParticleSystem Particle in Particles)
            {
                Particle.Stop();
            }
            Skip = true;
            Manager.Getstations();
        }

    }

}
