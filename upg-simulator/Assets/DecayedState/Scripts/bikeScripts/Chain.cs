using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour {
    public GameObject rearWheel;
    public GameObject chain;//
    public float Ypos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Ypos = rearWheel.transform.localPosition.y;
        chain.transform.localPosition = new Vector3(chain.transform.localPosition.x, Ypos*2f, chain.transform.localPosition.z);
	}
}
