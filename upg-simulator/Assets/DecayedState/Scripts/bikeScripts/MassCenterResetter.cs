using UnityEngine;
using System.Collections;

public class MassCenterResetter : MonoBehaviour {
	public Rigidbody rb;
	public float x;
	public float y;
	public float z;
    public float tiltAmount; 
    private CharacterControl playrScrpt;
    private BikeControl bikeScrpt;
    private GameObject Player;

    private float CoMZ;
    // Update is called once per frame
    void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        playrScrpt = Player.GetComponent<CharacterControl>();
        bikeScrpt = this.GetComponent<BikeControl>();        
        CoMZ = z;           
    }
    void Update() {
        if (transform.localEulerAngles.z > 30 && transform.localEulerAngles.z < 330)
        {

            if (Player.transform.parent != null && bikeScrpt.canControl)
            {
                playrScrpt.TurnToRagdoll(0);
            }
            bikeScrpt.canSit = false;
        }
        else {
            bikeScrpt.canSit = true;
        }
            if (transform.localEulerAngles.x > 10 && transform.localEulerAngles.x < 350)
        {
            //z = 0;
            z = Mathf.Lerp(0, z, Time.deltaTime * 10);
        }
        else {
            z = CoMZ;
        }

    }
	void FixedUpdate () {		
		rb.centerOfMass = new Vector3 (x, y, z);       
        if (rb.velocity.magnitude > 2)
        {
            rb.AddRelativeTorque(Vector3.forward * Input.GetAxis("Horizontal") * tiltAmount* rb.velocity.magnitude);
        }
    }
}
