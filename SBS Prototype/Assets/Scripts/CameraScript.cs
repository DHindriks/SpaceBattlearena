using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    [SerializeField]
    GameObject Player = null;

    [SerializeField]
    GameObject stars;

    [SerializeField]
    Transform target;

    float Rotationspeed = 0.1f;

    float speed = 2.0f;

    float yaw = 0.0f;
    float pitch = 0.0f;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}

    private void Update()
    {

        transform.position = Player.transform.position;
        stars.transform.position = transform.position;

        if (!Input.GetKey(KeyCode.Mouse2) && Player.GetComponent<Player>().Controllable == true)
        {
            RotatePlayer();
        }
        if (Player.GetComponent<Player>().Controllable == true)
        {
            yaw += speed * Input.GetAxis("Mouse X");
            if (pitch < -90 - speed)
            {
                pitch = -90;
            }
            else if (pitch > 90 + speed)
            {
                pitch = 90;
            } else
            {
                pitch -= speed * Input.GetAxis("Mouse Y");
            }
            transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void SetRotation(Vector3 rot)
    {
        pitch = rot.x;
        yaw = rot.y;
    }

    void RotatePlayer()
    {
        Vector3 targetDelta = target.position - Player.transform.position;

        float angleDiff = Vector3.Angle(Player.transform.forward, targetDelta);
        Vector3 cross = Vector3.Cross(Player.transform.forward, targetDelta);

        //apply torque
        Player.GetComponent<Rigidbody>().AddTorque(cross * angleDiff * Rotationspeed);
        Player.GetComponent<Rigidbody>().AddTorque(Vector3.Cross(Player.transform.right, transform.right) * Vector3.Angle(Player.transform.right, transform.right) * Rotationspeed * 4);

    }
}
