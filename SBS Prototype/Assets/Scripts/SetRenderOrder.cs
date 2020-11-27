using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderOrder : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<MeshRenderer>().material.renderQueue = 3001;
	}
}
