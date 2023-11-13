using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRenderer : MonoBehaviour {
    private LineRenderer cable;
    public Transform cableStart;
    public Transform cableEnd;
    // Use this for initialization
    void Start () {
        cable = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        cable.SetPosition(0, cableStart.transform.position);
        cable.SetPosition(1, cableEnd.transform.position);
    }
}
