using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    float speed = 2000;
    float Shield = 100;
    float Health = 100;

    public bool Controllable = true;
    public bool Invincible = false;
    public bool GameOver = false;
    public int Score;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    Material EnergyMat;


    [SerializeField]
    GameObject CollisionParticles;

    [SerializeField]
    GameObject DeathParticles;

    [SerializeField]
    List<ParticleSystem> DyingParticles;

    [SerializeField]
    GameObject Crosshair; //crosshair vor aiming

    [SerializeField]
    GameObject ShieldUI; //shieldbar in UI

    [SerializeField]
    GameObject HealthUI; //healthbar in UI

    [SerializeField]
    Text ScoreText;

    [SerializeField]
    GameObject GameOverScreen;

    [Range(0, 1)]
    float Throttle = 0; //forward speed of the player

    float LastDamaged; //time at which player last damaged

    bool Rolling; //if player is currently rolling
    KeyCode LastPress; //most recently pressed strafe button
    float LastPressTime; //time at which player last pressed a strafe button
    Material currentEnergy;
    public GameObject LastCollision;
    Rigidbody rb;

    [SerializeField]
    List<GameObject> EnergyColors;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
        UpdateEnergy();
	}
	
	// Update is called once per frame
	void Update () {
        if (Controllable == true)
        {
            //throttle up / down
            UpdateThrottle(KeyCode.S, KeyCode.W);

            //forward
            rb.AddForce(this.transform.TransformDirection(Vector3.forward * (Throttle * speed)) * Time.deltaTime);

            if (currentEnergy != EnergyMat)
            {
                UpdateEnergy();
            }

            //set crosshair position
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Crosshair.transform.position = Camera.main.WorldToScreenPoint(hit.point);
            }
            else
            {
                Crosshair.transform.position = Camera.main.WorldToScreenPoint(transform.forward * 50 + transform.position);
            }

            //Roll left
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (!Rolling && LastPress == KeyCode.A && LastPressTime + 0.25f >= Time.time)
                {
                    //do roll
                    StartCoroutine("FlipHorizontal", 360);
                    rb.AddForce(this.transform.TransformDirection(Vector3.left * (speed / 4)) * Time.deltaTime, ForceMode.Impulse);
                    Rolling = true;
                    LastPressTime = Time.time;
                }
                else
                {
                    //don't roll
                    LastPress = KeyCode.A;
                    LastPressTime = Time.time;
                }
            }

            //Roll right
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (!Rolling && LastPress == KeyCode.D && LastPressTime + 0.25f >= Time.time)
                {
                    //do roll
                    StartCoroutine("FlipHorizontal", -360);
                    rb.AddForce(this.transform.TransformDirection(Vector3.right * (speed / 4)) * Time.deltaTime, ForceMode.Impulse);
                    Rolling = true;
                    LastPressTime = Time.time;
                }
                else
                {
                    //don't roll
                    LastPress = KeyCode.D;
                    LastPressTime = Time.time;
                }
            }
            //strafe
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(this.transform.TransformDirection(Vector3.left * (speed / 2)) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(this.transform.TransformDirection(Vector3.right * (speed / 2)) * Time.deltaTime);
            }

            //shoot
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }

    public void FlipAnim()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            StartCoroutine("FlipHorizontal", -360);
        }else
        {
            StartCoroutine("FlipHorizontal", 360);
        }
    }

    //Barrel roll IEnumerator
    IEnumerator FlipHorizontal(float Degrees)
    {
        float startRotation = this.transform.GetChild(0).transform.localEulerAngles.z;
        float endRotation = startRotation + Degrees;
        float t = 0.0f;
        float duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            this.transform.GetChild(0).transform.localEulerAngles = new Vector3(this.transform.GetChild(0).transform.localEulerAngles.x, this.transform.GetChild(0).transform.localEulerAngles.y, zRotation);
            yield return null;
        }
        Rolling = false;
    }

    public void AddScore(int amount)
    {
        Score += amount;
        ScoreText.text = "Score: " + Score;
    }

    void OnCollisionEnter(Collision collision)
    {
        LastCollision = collision.gameObject;
        Invoke("ResetCollision", 4);
        Damage(Mathf.RoundToInt(collision.relativeVelocity.magnitude * 4));
        rb.AddForce(-(collision.contacts[0].point - transform.position) * 2, ForceMode.Impulse);
        GameObject ColParticles = Instantiate(CollisionParticles);
        ColParticles.transform.position = collision.contacts[0].point;

        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.contacts[0].point - transform.position) * 8, ForceMode.Impulse);
        }

    }

    void ResetCollision()
    {
        LastCollision = null;
    }

    public void Damage(int amount)
    {
        if (Invincible == false)
        {
            if (amount < 0)
            {
                Health -= amount;
                if (Health > 100)
                {
                    Health = 100;
                }

            }
            else
            {
                if (Shield <= 0)
                {
                    Health -= amount;
                }
                else
                {
                    Shield -= amount;
                }

                if (Shield < 0)
                {
                    Shield = 0;
                }

                if (Health <= 0 && Controllable == true)
                {
                    Die();
                }
                LastDamaged = Time.time;
                StartCoroutine(RegenerateShield(3f));
            }

        }
        UpdateHealthShieldUI();
    }

    void Die()
    {
        rb.drag = 0.5f;
        rb.angularDrag = 0;
        rb.AddRelativeForce(0, 0, Random.Range(-50, 50));
        Throttle = 0;
        Controllable = false;
        foreach(Transform child in Crosshair.transform.root)
        {
            child.gameObject.SetActive(false);
        }
        foreach(ParticleSystem particle in DyingParticles)
        {
            particle.Play();
        }
        Invoke("destroy", 3);
    }

    void destroy()
    {
        GameObject particle = Instantiate(DeathParticles);
        GetComponent<AudioSource>().Play();
        particle.transform.position = transform.position;
        GameOver = true;
        foreach(ParticleSystem dyingparticle in DyingParticles)
        {
            dyingparticle.gameObject.transform.SetParent(particle.transform, true);
            dyingparticle.Stop();
        }
        rb.isKinematic = true;
        transform.GetChild(0).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Invoke("GetGameOverScreen", 0.5f);
    }

    void GetGameOverScreen()
    {
        GameOverScreen.SetActive(true);
        GameOverScreen.transform.GetChild(0).GetComponent<Text>().text = "Score: " + Score;
        if (Score > PlayerPrefs.GetInt("HighScore"))
        {
            GameOverScreen.transform.GetChild(1).gameObject.SetActive(true);
            GameOverScreen.transform.GetChild(2).gameObject.SetActive(false);
        }else
        {
            GameOverScreen.transform.GetChild(1).gameObject.SetActive(false);
            GameOverScreen.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    //regenerate shield after delay
    IEnumerator RegenerateShield(float Delay)
    {
        float Delaytime = Delay;
        if (Delay != 0)
        {
            yield return new WaitForSeconds(Delay);
            Delay = 0;
        }

        while (Shield < 100 && Time.time - Delaytime > LastDamaged)
        {
            Shield += 1.5f;
            UpdateHealthShieldUI();
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return null;
        
    }

    //sets health and shield bar to the current values
    void UpdateHealthShieldUI()
    {
        ShieldUI.transform.localScale = new Vector3(Shield / 100, 1, 1);
        HealthUI.transform.localScale = new Vector3(Health / 100, 1, 1);
    }

    void Shoot ()
    {
        GameObject BulletClone = Instantiate(Bullet);
        BulletClone.GetComponent<Renderer>().material = EnergyMat;
        BulletClone.transform.position = transform.position;
        BulletClone.transform.rotation = transform.rotation;
        BulletClone.GetComponent<Rigidbody>().AddRelativeForce(BulletClone.transform.forward * 60, ForceMode.VelocityChange);
    }

    void UpdateThrottle (KeyCode DecreaseKey, KeyCode IncreaseKey)
    {
        float TargetThrottle = Throttle;

        if (Input.GetKey(IncreaseKey))
        {
            TargetThrottle = 1;
        }
        else if (Input.GetKey(DecreaseKey))
        {
            TargetThrottle = 0;
        }
        Throttle = Mathf.MoveTowards(Throttle, TargetThrottle, Time.deltaTime * 0.75f);
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
            if (energy.GetComponent<RawImage>())
                energy.GetComponent<RawImage>().color = EnergyMat.color;

            if (energy.GetComponentInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = energy.GetComponentInChildren<ParticleSystem>().main;
                main.startColor = EnergyMat.color;
            }
            currentEnergy = EnergyMat;
        }
    }

}
