using UnityEngine;
using System.Collections;

public class lookAt : MonoBehaviour {
	public Transform boneToRotate;
	public float OffsetX;
	public float OffsetY;
	public float OffsetZ;
	private GameObject objPlayer;
	private CharacterControl ptrCharContrlScript;
	public bool canAim;
	// Use this for initialization
	void Start () {	
		objPlayer = (GameObject) GameObject.FindWithTag ("Player");//caching player's GO
		ptrCharContrlScript = (CharacterControl) objPlayer.GetComponent( typeof(CharacterControl) );//caching player control script

	}	
	void LateUpdate(){//bone rotation
        if (ptrCharContrlScript.rifleAiming)
        {
            StartCoroutine(Aim());
            if (canAim)
            {
                Vector3 aimPoint = Camera.main.transform.position + Camera.main.transform.forward * 10f;
                boneToRotate.LookAt(aimPoint);//rotate bone towards aimpoint
                boneToRotate.transform.Rotate(boneToRotate.transform.rotation.x + OffsetX, boneToRotate.transform.rotation.y + OffsetY, boneToRotate.transform.rotation.z + OffsetZ);
                //as bone's axis are not the same as aimPoint's axis lets reorient bone properly
            }
        }
        else {
            canAim = false;
        }
	}
    IEnumerator Aim() {
        if (!ptrCharContrlScript.atWall)
        {
            canAim = true;
        }
        else {
            yield return new WaitForSeconds(.4f);
            canAim = true;
        }
    }
}
