using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movedoor : MonoBehaviour {

    [SerializeField]
    Vector3 Moveto;

    Vector3 Original;


    [SerializeField]
    Transform door;

    [SerializeField]
    float LerpTime;

    void Start()
    {
        Original = door.localPosition;
    }

    IEnumerator OpenDoor()
    {
        StopCoroutine(CloseDoor());
        float Rate = 1.0f / LerpTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            door.localPosition = Vector3.Lerp(door.localPosition, Moveto, i);
            yield return 0;
        }
    }

    IEnumerator CloseDoor()
    {
        StopCoroutine(OpenDoor());
        float Rate = 1.0f / LerpTime;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            door.localPosition = Vector3.Lerp(door.localPosition, Original, i);
            yield return 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            StartCoroutine("OpenDoor");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "player")
        {
            StartCoroutine("CloseDoor");
        }
    }
}
