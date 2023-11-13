using UnityEngine;
using System.Collections;
 
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {
	public Transform PlayerCamTarget;
	private Transform target;
	public Transform camOrigin;
	private Vector3 wallHitPos;
	public float tempDist;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
	
	public float xMinLimit = -30f;
    public float xMaxLimit = 50f;
 
    public float distanceMin = 5f;
    public float distanceMax = 15f;
 
    float x = 0.0f;
    float y = 0.0f;
	
	private GameObject objPlayer;
	private CharacterControl ptrCharacterControl;
 
	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		objPlayer = (GameObject) GameObject.FindWithTag ("Player");
		ptrCharacterControl = (CharacterControl) objPlayer.GetComponent( typeof(CharacterControl) );		
        Vector3 angles = transform.localEulerAngles;
        x = angles.y;
        y = angles.x;
	}
 
	void Update () {
		if (ptrCharacterControl.driving) {
						target = ptrCharacterControl.carCamPoint.transform;		
				} else {
			target = PlayerCamTarget;
		}
	}
    void LateUpdate () {
    if (target) {
			if(!ptrCharacterControl.rifleAiming){
			AvoidCollisions();
			}
			else{
				distance = distanceMin;
			}
        x += Input.GetAxis("Mouse X") * xSpeed *  0.05f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			if(ptrCharacterControl.shooting){//weapon kickback
				y -= ptrCharacterControl.currentWeapon.weaponKickBack;
				x -= Random.Range (ptrCharacterControl.currentWeapon.weaponKickBack*2.5f, -ptrCharacterControl.currentWeapon.weaponKickBack*2.5f);
			}
        y = ClampAngle(y, yMinLimit, yMaxLimit);
		if(ptrCharacterControl.atWall){
			x = ClampAngle(x, xMinLimit+ptrCharacterControl.currentWallAngleY, xMaxLimit+ptrCharacterControl.currentWallAngleY);

			}
        Quaternion rotation = Quaternion.Euler(y, x, 0); 
        //distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;
 
        transform.rotation = rotation;
        transform.position = position;
 
    }  		
}

	void AvoidCollisions(){
		RaycastHit wallHit = new RaycastHit ();
		if (Physics.Linecast (target.transform.position, camOrigin.transform.position, out wallHit)) {
			wallHitPos = new Vector3(wallHit.point.x, wallHit.point.y, wallHit.point.z);		
		}
		tempDist = Vector3.Distance(wallHitPos, this.transform.position);
		if (tempDist> distanceMax){
			tempDist = distanceMax;
		}
		distance = Mathf.Lerp (distance, tempDist, Time.deltaTime * 10f);
	}
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -720F)
            angle += 720F;
        if (angle > 720F)
            angle -= 720F;
        return Mathf.Clamp(angle, min, max);
    }
}