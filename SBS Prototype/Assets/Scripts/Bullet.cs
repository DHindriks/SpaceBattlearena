using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [HideInInspector]
    public GameObject Origin;
    [SerializeField]
    GameObject DeathParticle;

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
                other.transform.root.GetComponent<Enemy>().Damage(20);
                if(!other.transform.root.GetComponent<Enemy>().scored && other.transform.root.GetComponent<Enemy>().health <= 0 && Origin.tag == "player")
                {
                    other.transform.root.GetComponent<Enemy>().scored = true;
                    Origin.GetComponent<Player>().AddScore(50);
                    int randomdialogue = Random.Range(0, 5);
                    switch (randomdialogue)
                    {
                        case 0:
                            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("Enemy down.");
                            break;
                        case 1:
                            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("Going down!", Characters.GenericPilot);
                            break;
                        case 2:
                            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("You got another one.");
                            break;
                        case 3:
                            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("Arg! We need backup!", Characters.GenericPilot);
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
                other.transform.SendMessageUpwards("Damage", 20, SendMessageOptions.DontRequireReceiver);
            }
            Killbullet();
        }
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
