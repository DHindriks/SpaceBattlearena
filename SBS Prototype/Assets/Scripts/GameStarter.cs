using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject camObject;

    [SerializeField]
    GameObject canvasObject;

    [SerializeField]
    ParticleSystem speedparticles;

    [SerializeField]
    Transform door;

    Rigidbody rb;
    bool Flying;
    float Direction;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
        rb.AddForce(transform.TransformDirection(Vector3.forward) * 100, ForceMode.VelocityChange);
        Invoke("StopShip", 5);
	}

    void StopShip()
    {
        rb.drag = 1;
        StartCoroutine(LerpPosRot(door.gameObject, new Vector3(0, -2.05f, -3.2f), new Vector3(-122, 0, 0)));
        Invoke("EjectPlayer", 1.5f);
        speedparticles.Stop();
    }

    void EjectPlayer()
    {
        player.SetActive(true);
        player.transform.SetParent(null);
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Rigidbody>().AddForce(player.transform.TransformDirection(-Vector3.forward) * 20, ForceMode.VelocityChange);
        Invoke("FlyAway", 1);
        Invoke("SetControls", 1.5f);
    }

    void FlyAway()
    {
        rb.drag = 0;
        Direction = Random.Range(4, -2) * 2;
        Flying = true;
        StartCoroutine(LerpPosRot(door.gameObject, new Vector3(0, -2.05f, -3.2f), new Vector3(-90, 0, 0)));
        StartCoroutine(LerpPosRot(camObject.gameObject, new Vector3(0, 1.25f, -3.5f), new Vector3(0, 0, 0), 0.5f));
        GameObject camPivot = camObject.transform.parent.gameObject;
        StartCoroutine(LerpPosRot(camPivot, new Vector3(camPivot.transform.position.x, camPivot.transform.position.y, camPivot.transform.position.z), new Vector3(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z), 0.5f));
        Destroy(gameObject, 15);
    }

    void SetControls()
    {
        camObject.transform.parent.rotation = Quaternion.Euler(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z);
        player.GetComponent<Player>().Controllable = true;
        canvasObject.SetActive(true);
        speedparticles.Play();
    }

    IEnumerator LerpPosRot(GameObject objectToMove, Vector3 newLocalPos, Vector3 newLocalRot, float Duration = 1)
    {
        Vector3 OldLocalPos = objectToMove.transform.localPosition;
        Quaternion OldLocalRot = objectToMove.transform.localRotation;

        Quaternion newLocalRotQ = Quaternion.Euler(newLocalRot.x, newLocalRot.y, newLocalRot.z);

        float elapsed = 0f;
        while(elapsed < Duration)
        {
            objectToMove.transform.localPosition = Vector3.Lerp(OldLocalPos, newLocalPos, elapsed / Duration);
            objectToMove.transform.localRotation = Quaternion.Lerp(OldLocalRot, newLocalRotQ, elapsed / Duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.localPosition = newLocalPos;
        objectToMove.transform.localEulerAngles = newLocalRot;
    }

    void Update()
    {
        this.transform.LookAt(this.transform.position + this.GetComponent<Rigidbody>().velocity);
        if (Flying == true)
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward) * 50, ForceMode.Acceleration);
            rb.AddForce(transform.TransformDirection(Vector3.up) * 5, ForceMode.Acceleration);
            rb.AddForce(transform.TransformDirection(new Vector3(Direction, 0, 0)) * 5, ForceMode.Acceleration);
        }
    }
}
