using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour {
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Vector3 Torque;

	// Use this for initialization
	void Start () {
        rb.AddRelativeTorque(Torque, ForceMode.VelocityChange);	
	}
}
