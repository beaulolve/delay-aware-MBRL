using UnityEngine;
using System.Collections;

public class lookAtBike : MonoBehaviour {
	public Transform boneToRotate;
	public float OffsetX;
	public float OffsetY;
	public float OffsetZ;
    public Transform objToLook;
    public float rotSpeed = 2;
    private Vector3 localPos;
    // Use this for initialization
    void Start () {	


	}	
	void LateUpdate(){//bone rotation
        Vector3 aimPoint = objToLook.transform.position;
        boneToRotate.LookAt(aimPoint, objToLook.transform.right);//rotate bone towards aimpoint
        boneToRotate.transform.Rotate(boneToRotate.transform.localRotation.x + OffsetX, 0 + OffsetY, 0 + OffsetZ);
    }
}
