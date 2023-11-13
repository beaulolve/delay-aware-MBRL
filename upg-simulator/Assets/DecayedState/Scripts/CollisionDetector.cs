using UnityEngine;
using System.Collections;

public class CollisionDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider trigg){
		if(trigg.gameObject.name == "jerrycan"){
			Debug.Log ("JERRy");				
		}
	}
}
