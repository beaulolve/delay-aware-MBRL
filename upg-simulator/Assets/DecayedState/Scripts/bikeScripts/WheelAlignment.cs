using UnityEngine;
using System.Collections;

public class WheelAlignment : MonoBehaviour {    
	public WheelCollider m_WheelCollider;
	public GameObject m_WheelMesh;
    public GameObject rearWheel;

    public bool isFrontWheel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion quat;
		Vector3 position;
		m_WheelCollider.GetWorldPose(out position, out quat);
		m_WheelMesh.transform.position = position;
        if (!isFrontWheel)
        {
            m_WheelMesh.transform.rotation = quat;
        }
        else {
            m_WheelMesh.transform.localEulerAngles = new Vector3(m_WheelMesh.transform.localEulerAngles.x, m_WheelCollider.steerAngle - m_WheelMesh.transform.localEulerAngles.z, m_WheelMesh.transform.localEulerAngles.z);
            //m_WheelMesh.transform.Rotate(quat.eulerAngles.x, 0, 0);
            m_WheelMesh.transform.localRotation = m_WheelMesh.transform.localRotation* rearWheel.transform.localRotation;
        }
	}
}
