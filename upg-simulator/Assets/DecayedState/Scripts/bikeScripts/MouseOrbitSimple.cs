using UnityEngine;
using System.Collections;

public class MouseOrbitSimple : MonoBehaviour {
	public Transform PlayerCamTarget;
	private Camera playerCam;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float x = 0.0f;
	public float y = 0.0f;
	public float z = 0.0f;


	// Use this for initialization
	void Start () {
		playerCam = Camera.main;
		Vector3 angles = transform.localEulerAngles;
		x = angles.y;
		y = angles.x;
		z = angles.z;
	}
	void FixedUpdate () {
		

		if (PlayerCamTarget) {
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			if (Vector3.Dot(playerCam.transform.up, Vector3.down) > 0)
			{
				x -= Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			}else{
				x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			}
			Quaternion rotation = Quaternion.Euler(y, x, 0); 
			transform.rotation = Quaternion.Lerp(transform.rotation,rotation,Time.deltaTime  * 5);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = transform.rotation * negDistance + PlayerCamTarget.position;
			transform.position = position;
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		}   
	}
	public void cameraHyperDrive(){
		Vector3 originalPos = transform.localPosition;
		transform.localPosition = originalPos + Random.insideUnitSphere * 0.5f;
	}
}