using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    Rigidbody rb;

    GameObject target;
    [SerializeField]
    GameObject CollisionParticles;

    [SerializeField]
    GameObject Bullet;
    [SerializeField]
    Material EnergyMat;         //energy or team color, bullets will use this color
    public bool scored = false;

    public int health;
    int startHealth;            //the amount of health the enemy started with, used to visualize damage
    [SerializeField]
    ParticleSystem dmgState1;
    [SerializeField]
    ParticleSystem dmgState2;
    [SerializeField]
    GameObject DeathParticle;
    [SerializeField]
    List<GameObject> EnergyColors;

    Vector3 headingPos;

    GameObject LastCollision;
    int speed;
    int attackCooldown;         //frequency at which the enemy attacks
    int stoppingDistance;       //distance at which the enemy will stop attacking to avoid collision with the player
    float AttackTime;           //time at which the enemy started attacking
    float Firerate = 1.5f; 
    float Turnrate = 3;        //time between each turn
    float NextTurn;             //time at which enemy turns again;
    float NextShot;             //time at which enemy shoots again
    States State;
    public enum States
    {
        Patrol,
        Attack,
        Dead,
    }

    // Use this for initialization
    void Awake() {
        State = States.Patrol;
        rb = GetComponent<Rigidbody>();
        speed = Random.Range(750, 1500);
        health = Random.Range(75, 150);
        stoppingDistance = Random.Range(8, 16);
        attackCooldown = Random.Range(15, 45);
        startHealth = health;
        EnterPatrolMode();
        UpdateEnergy();
    }

    void EnterAttackMode()
    {
        State = States.Attack;
        NextShot = Time.time + Firerate * 2;
        AttackTime = Time.time;
    }

    void EnterPatrolMode(float CooldownReduction = 0)
    {
        State = States.Patrol;
        if (CooldownReduction == 0)
        {
            Invoke("EnterAttackMode", attackCooldown);
        }
        else
        {
            Invoke("EnterAttackMode", attackCooldown / CooldownReduction);
        }
    }

    void GetRandomRot()
    {
        headingPos = Random.rotation * Vector3.forward * 500;
    }

    // Update is called once per frame
    void Update() {
        if (State != States.Dead)
        {
            if (State == States.Attack && NextShot < Time.time)
            {
                NextShot = Time.time + Firerate;
                Shoot();
            }

            if (State == States.Patrol)
            {
                RotateToPosition(headingPos, 0.0001f);
                if (NextTurn < Time.time)
                {
                    NextTurn = Time.time + Turnrate;
                    GetRandomRot();
                }
            }

            //forward
            rb.AddForce(this.transform.TransformDirection(Vector3.forward * speed) * Time.deltaTime, ForceMode.Force);



            RaycastHit hit;
            //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit) && Vector3.Distance(hit.collider.ClosestPoint(transform.position), transform.position) < 8 || rotating == true && hit.collider != null)
            if (Physics.SphereCast(transform.position, 3, transform.TransformDirection(Vector3.forward), out hit, 18) && Vector3.Distance(hit.point, transform.position) < 8) //collider.ClosestPoint(transform.position)
            {
                RotateToPosition(hit.normal, 0.1f);

                if (hit.transform.root.gameObject.tag == "player" && State == States.Attack && Vector3.Distance(transform.position, target.transform.position) <= stoppingDistance || AttackTime < 10)
                {
                    EnterPatrolMode();
                }else if (State == States.Attack && hit.transform.root.tag != "player")
                {
                    EnterPatrolMode(10);
                }


            } else if (hit.collider == null && target != null && State == States.Attack || Vector3.Distance(hit.point, transform.position) > 8 && target != null && State == States.Attack)
            {
                RotateToPosition(target.transform.position - transform.position, 0.1f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        LastCollision = collision.gameObject;
        Damage(Mathf.RoundToInt(collision.relativeVelocity.magnitude * 5));
        rb.AddForce(-(collision.contacts[0].point - transform.position) * 2, ForceMode.Impulse);
        GameObject ColParticles = Instantiate(CollisionParticles);
        ColParticles.transform.position = collision.contacts[0].point;
        if (LastCollision == target.GetComponent<Player>().LastCollision && !scored && health <= 0)
        {
            target.GetComponent<Player>().AddScore(100);
            GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("That's one way to use that debris I suppose.");
            scored = true;
        }
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.contacts[0].point - transform.position) * 8, ForceMode.Impulse);
        }

    }

    void RotateToPosition(Vector3 targetDelta, float Rotationspeed = 0.1f)
    {
        //Vector3 targetDelta = target.position - transform.position;

        float angleDiff = Vector3.Angle(transform.forward, targetDelta);
        Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

        //apply torque
        rb.AddTorque(cross * angleDiff * Rotationspeed);
        rb.AddTorque(Vector3.Cross(transform.right, transform.right) * Vector3.Angle(transform.right, transform.right) * Rotationspeed * 4);

    }


    public void Damage(int amount)
    {
        health -= amount;
        if (health <= startHealth / 2)
        {
            dmgState1.Play();
        }
        if (health <= startHealth / 4)
        {
            dmgState2.Play();
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (State != States.Dead)
        {
            State = States.Dead;
            CancelInvoke("EnterAttackMode");
            rb.drag = 0.5f;
            rb.angularDrag = 0;
            rb.AddRelativeTorque(0, 0, Random.Range(-10, 10));
            Invoke("Explode", 5);
        }
    }

    void Explode()
    {
        GameObject particle = Instantiate(DeathParticle);
        GetComponent<AudioSource>().Play();
        particle.transform.position = this.transform.position;
        dmgState1.gameObject.transform.SetParent(particle.transform, true);
        dmgState1.Stop();

        dmgState2.gameObject.transform.SetParent(particle.transform, true);
        dmgState2.Stop();
        Destroy(this.gameObject, 1);
        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void Shoot()
    {
        GameObject BulletClone = Instantiate(Bullet);
        BulletClone.GetComponent<Renderer>().material = EnergyMat;
        BulletClone.transform.position = transform.position;
        BulletClone.transform.rotation = transform.rotation;

        if (target.GetComponent<Rigidbody>() != null)
        {
            BulletClone.transform.LookAt(target.transform.position + target.GetComponent<Rigidbody>().velocity);
        }
        else
        {
            BulletClone.transform.LookAt(target.transform.position + target.transform.forward * 4);
        }

        BulletClone.GetComponent<Rigidbody>().AddRelativeForce(BulletClone.transform.forward * 40, ForceMode.VelocityChange);
    }

    public void SetTarget(GameObject trgt)
    {
        target = trgt;
    }

    void UpdateEnergy()
    {
        foreach (GameObject energy in EnergyColors)
        {
            if (energy.GetComponent<Renderer>())
                energy.GetComponent<Renderer>().material = EnergyMat;
            if (energy.GetComponentInChildren<ParticleSystemRenderer>())
                energy.GetComponentInChildren<ParticleSystemRenderer>().material = EnergyMat;
            if (energy.GetComponentInChildren<TrailRenderer>())
                energy.GetComponentInChildren<TrailRenderer>().material = EnergyMat;

            if (energy.GetComponentInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = energy.GetComponentInChildren<ParticleSystem>().main;
                main.startColor = EnergyMat.color;
            }
        }
    }
}
