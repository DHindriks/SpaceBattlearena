using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationCutscene : MonoBehaviour {

    [SerializeField]
    List<GameObject> PostDeathArena;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    GameObject CamObject;
    GameObject CamPivot;

    GameObject player;

    [SerializeField]
    Transform playerPos;

    [SerializeField]
    Transform camPos1;

    [SerializeField]
    Transform camPos2;

    // Use this for initialization
    void Awake() {
		player = GameObject.FindWithTag("player");
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<Player>().Controllable = false;
        player.GetComponent<Player>().Invincible = true;
        CamPivot = CamObject.transform.parent.gameObject;
        CamPivot.GetComponent<CameraScript>().SetRotation(new Vector3(-16, 16, 0));
        CamPivot.transform.rotation = Quaternion.Euler(-16, 16, 0);
        canvas.SetActive(false);
        CamObject.transform.SetParent(null);
        CamObject.transform.position = camPos1.position;
        CamObject.transform.rotation = camPos1.rotation;
        player.transform.position = playerPos.position;
        player.transform.rotation = playerPos.rotation;
        GameObject.FindWithTag("DialogueManager").GetComponent<DialogueManager>().AddDialogue("You did it! the station has been disabled!");
        Invoke("Phase2", 8);
    }

    void Phase2()
    {
        CamObject.transform.position = camPos2.position;
        CamObject.transform.rotation = camPos2.rotation;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Rigidbody>().drag = 0;
        player.GetComponent<Rigidbody>().AddForce(player.transform.TransformDirection(Vector3.forward) * 12.5f, ForceMode.VelocityChange);
        Invoke("PlayerRoll", 3);
        Invoke("RestoreCam", 4);

    }

    void PlayerRoll()
    {
        player.GetComponent<Player>().FlipAnim();
    }

    void RestoreCam()
    {
        CamObject.transform.parent = CamPivot.transform;
        StartCoroutine(LerpPos(CamObject, new Vector3(0, 1.25f, -3.5f), 0.25f));
        StartCoroutine(LerpRot(CamObject, new Vector3(0, 0, 0)));
        player.GetComponent<Rigidbody>().drag = 2;
        Invoke("SetControls", 0.5f);
    }

    void SetControls()
    {
        canvas.SetActive(true);
        player.GetComponent<Player>().Controllable = true;
        player.GetComponent<Player>().Invincible = false;
        foreach (GameObject debris in PostDeathArena)
        {
            debris.SetActive(true);
        }
    }

    IEnumerator LerpPos(GameObject objectToMove, Vector3 newLocalPos, float Duration = 0.5f)
    {
        Vector3 OldLocalPos = objectToMove.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            objectToMove.transform.localPosition = Vector3.Lerp(OldLocalPos, newLocalPos, elapsed / Duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.localPosition = newLocalPos;
    }

    IEnumerator LerpRot(GameObject objectToMove, Vector3 newLocalRot, float Duration = 0.5f)
    {
        Quaternion OldLocalRot = objectToMove.transform.localRotation;
        Quaternion newLocalRotQ = Quaternion.Euler(newLocalRot.x, newLocalRot.y, newLocalRot.z);

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            objectToMove.transform.localRotation = Quaternion.Lerp(OldLocalRot, newLocalRotQ, elapsed / Duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objectToMove.transform.localEulerAngles = newLocalRot;
    }
}
