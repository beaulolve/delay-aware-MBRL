using UnityEngine;
using System.Collections;

public class HorseControl : MonoBehaviour {
	private Animator _animator;
	private CharacterController _charCtrl;
	
	private Vector3 moveDirection;//movement direction
	private bool _run;
	public bool canControll;
	
	private float _speed;
	private float _strafe;
	private float _walkSpeed = 2f;
	private float rotationSpeed = 60f;//speed of rotating
	private float gravity = 10.0f;//gravity affecting character
	private float speed = 6.0f;//character's movement speed
	public GameObject backBone;
	public GameObject horseSaddle;
    public GameObject horseRoot;
    private AudioSource m_AudioSource;
    public AudioClip[] gallopSounds;
    public AudioClip[] walkSounds;
    // Use this for initialization
    void Start () {
		_animator = GetComponent<Animator>();
		_charCtrl = GetComponent<CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if(canControll){
		_run = Input.GetKey(KeyCode.LeftShift) ? true : false;//check if run button was pressed
		_speed = Input.GetAxis("Vertical");//reading vertical axis input
		_strafe = Input.GetAxis("Horizontal");
		if(Input.GetAxis("Vertical") > 0) {
			_charCtrl.Move(transform.forward *_walkSpeed *Time.deltaTime);
		}
		if(Input.GetAxis("Vertical") < 0) {
			_charCtrl.Move(transform.forward *-_walkSpeed/2 *Time.deltaTime);
		}
//////////ROTATING RIGHT
		if(Input.GetAxis("Horizontal") > 0) {
			transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed *Time.deltaTime, 0);
			}
//////////ROTATING LEFT
		if(Input.GetAxis("Horizontal") < 0) {
			transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed *Time.deltaTime, 0);
			}
		if(_run){
			_walkSpeed = 8;
		}
		else{
			_walkSpeed = 3;
		}
		
		}
		else{
			_speed = 0;
			_strafe = 0;
		}
		_charCtrl.Move(moveDirection *speed *Time.deltaTime); //moving character controller forward with speed over time
		moveDirection.y -= gravity * Time.deltaTime;// applying gravity
		
	}
	void FixedUpdate(){
        
            _animator.SetFloat("Speed", _speed);
            _animator.SetFloat("Strafe", _strafe);
            _animator.SetBool("Running", _run);
        
	}
    void LateUpdate() {
        if (canControll){
            CheckGroundNrm();
        }
    }
    void CheckGroundNrm() {
        //SHOOTING RAY DOWN
        Vector3 bottom = -transform.up;
        Vector3 btmRayStart = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        Ray btmRay = new Ray(btmRayStart, bottom);
        RaycastHit hit;
        Debug.DrawRay(btmRayStart, transform.forward, Color.green);
        if (Physics.Raycast(btmRay, out hit, 2f)){
            Quaternion fro = Quaternion.FromToRotation(horseRoot.transform.up, hit.normal) * horseRoot.transform.rotation;
            horseRoot.transform.rotation = Quaternion.Slerp(horseRoot.transform.rotation, fro, Time.deltaTime * 2f);
            horseRoot.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }
    //FOOTSTEP SOUND FUNCTION IS CALLED FROM ANIMATION EVENT 
    void Gallop()
    {
        if (_walkSpeed == 8)
        {
            int n = Random.Range(1, gallopSounds.Length);
            m_AudioSource.clip = gallopSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            //			move picked sound to index 0 so it's not picked next time
            gallopSounds[n] = gallopSounds[0];
            gallopSounds[0] = m_AudioSource.clip;
        }
        else {
            int n = Random.Range(1, walkSounds.Length);
            m_AudioSource.clip = walkSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            //			move picked sound to index 0 so it's not picked next time
            walkSounds[n] = walkSounds[0];
            walkSounds[0] = m_AudioSource.clip;
        }
     }
}
