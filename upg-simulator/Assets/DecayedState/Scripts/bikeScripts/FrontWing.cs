using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontWing : MonoBehaviour {
    public GameObject frontWheel;
    public GameObject FrontFork;
    public float Zpos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Zpos = frontWheel.transform.localPosition.z;
        FrontFork.transform.localPosition = new Vector3(FrontFork.transform.localPosition.x, FrontFork.transform.localPosition.y, FrontFork.transform.localPosition.z+Zpos);
	}
}
