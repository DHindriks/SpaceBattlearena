using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    [HideInInspector]
    public GameObject Origin;
    [SerializeField]
    GameObject DeathParticle;

    GameObject Target;

    public void Start()
    {
        Invoke("Killbullet", 5);
        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = GetComponent<Renderer>().material.color;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (Origin == null)
        {
            Origin = other.transform.root.gameObject;
        }
        if (other.transform.root.gameObject != Origin && !other.isTrigger)
        {
            if (other.transform.root.GetComponent<Enemy>() != null)
            {
                other.transform.root.GetComponent<Enemy>().Damage(40);
                if(!other.transform.root.GetComponent<Enemy>().scored && other.transform.root.GetComponent<Enemy>().health <= 0 && Origin.tag == "player")
                {
                    other.transform.root.GetComponent<Enemy>().scored = true;
                    Origin.GetComponent<Player>().AddScore(50);
                    int randomdialogue = Random.Range(0, 0);
                    switch (randomdialogue)
                    {
                        case 0:
                            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("Heavy missiles! Argh!", Characters.GenericPilot);
                            break;
                    }
                }
            }
            else if (other.transform.root.GetComponent<Player>() != null)
            {
                other.transform.root.GetComponent<Player>().Damage(20);
            }
            else
            {
                other.transform.SendMessageUpwards("Damage", 40, SendMessageOptions.DontRequireReceiver);
            }
            Killbullet();
        }
    }

    void Update()
    {
        GetComponent<Rigidbody>().AddForce(this.transform.TransformDirection(Vector3.forward * 4) * Time.deltaTime);

    }

    void Killbullet()
    {
        CancelInvoke("Killbullet");
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<Rigidbody>().isKinematic = true;
        GameObject deathparticle = Instantiate(DeathParticle);
        ParticleSystem.MainModule main = deathparticle.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<Renderer>().material.color;
        deathparticle.transform.position = transform.position;
        deathparticle.transform.position -= transform.forward;
        Destroy(this.gameObject, 4);
    }
}
