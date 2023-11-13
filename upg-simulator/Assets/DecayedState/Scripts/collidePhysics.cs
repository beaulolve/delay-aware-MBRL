using UnityEngine;
using System.Collections;

public class collidePhysics : MonoBehaviour {
	private bool collided;
    private Transform myTransf;
    public bool thisIsHedgeHigh;
    public GameObject HedgeHighpBrokPref;
    // Use this for initialization
    void Start () {
        myTransf = this.transform;
	}
	void OnCollisionEnter (Collision collision){
		if (collision.relativeVelocity.magnitude > 1 && !collided) {
            if (thisIsHedgeHigh)
            {
                collided = true;
                Instantiate(HedgeHighpBrokPref, myTransf.position, myTransf.rotation);
                Destroy(gameObject);
            }
            else {
                collided = true;
                this.gameObject.AddComponent<Rigidbody>();
            }       
		}
	}
}
