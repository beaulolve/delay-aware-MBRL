using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterControl : MonoBehaviour {
	public Animator _animator;
    public Collider[] ragDallColliders;//array of character's collider
    public Component[] rigidBodies;//array of character's rigidbodies
    public AudioClip[] m_FootstepSounds;
    public AudioClip[] m_GroundFootstepSounds;
    public AudioClip[] m_WaterFootSteps;
    public AudioClip landingSound;
	public AudioClip openCarDoor;
	public AudioClip closeCarDoor;
	public AudioClip shotGunShot;
	public AudioClip poringGasSound;
	private CharacterController _charCtrl;
	private LayerMask characterMask = 5;
    private HandPlacement handScrpt;
    private GetTerrainMat terrSurfScrpt;
	private bool _canSlideToCover;
	private bool _crouch;
	public bool _run;
	public bool shooting;
    public bool faceDown;
	private bool _kickDoor;
	private bool _jump;
	private bool _jumpOverObstcle;
	private bool _jumpUpHigh;
	private bool _stop;
	private bool _stopBack;
	private bool _coverRolling;
	private bool _getInCar;
	private bool _getInLowCar;
	private bool _glider;
    private bool _getOnBike;
    private bool _getOnHorse;
	public bool _ridingHorse; 
	private bool canRotate = true;
	private int doorSide=1;

	public bool alive = true;//i am alive
	public bool chopTree;
	public bool driving;
	public bool gliding;
	public bool usingRifle;
	public bool usingAxe;
	public bool atWall;
	public bool rifleAiming;
	public bool atTheLeftCorner;
	public bool atTheRightCorner;
	public bool holdingJerry;
    public bool canUseIK;

	private Vector3 moveDirection = Vector3.zero;//player's move direction

	public float health = 100;
	public float hydratation = 100;
	public float _speed;
	public float currentWallAngleY;
	public float gravity = 3f;//gravity force 
	private float rotY;
	private float AimYSpeed =4;
	private float t = 0f;
	private float x = 0f;
	private float _strafe;
	private float shootTime = 0f;
    private float bikeCurrSpeed;
    //IK
    private float lookIKweight = 1;
    private float bodyWeight = 0.2f;
    private float headWeight = 1;
    private float eyesWeight = 1;
    private float clampWeight = 1;

    public GameObject PLayerSpine;
    public GameObject curJerrycan;
	public GameObject currentHorse;
	public GameObject currDoor;
	public GameObject currCar;
	public GameObject currGlide;
    public GameObject currBike;
    public GameObject carCamPoint; 
	public GameObject currentWall;//THE wall we'll hide at
	public GameObject jerrycanPosition;
	private GameObject ShotGun;

    public Transform lookAtPos;
    private Transform mountPoint;
	private Transform axeHandPosistion;
	private Transform shotgunHandPosition;
	private Transform axeHolster;
	private Transform shotgunHolster;

	private AudioSource m_AudioSource;

	//WEAPON SECTION

	/// assign all the values in the inspector

	public WeaponInfo currentWeapon;
	public List<WeaponInfo> WeaponList = new List<WeaponInfo>();
	[System.Serializable]
	public class WeaponInfo{
		public string weaponName = "weapon name";
		public float fireRate = 0.1f;
		public Transform weaponTransform;
		public Transform weaponMuzzlePoint;
		public float weaponKickBack;
		public GameObject bulletPrefab;
		public int totalAmmo;
		public int magazine;
	}
	static int SlideToCoverState = Animator.StringToHash("Base Layer.SprintSlideToCower");
	//static int LocoState = Animator.StringToHash("Base Layer.Locomotion");
	// Use this for initialization
	void Start () {
		ShotGun = GameObject.Find("remmington");
		m_AudioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_charCtrl = GetComponent<CharacterController>();
        handScrpt = GetComponent<HandPlacement>();
        terrSurfScrpt = GetComponent<GetTerrainMat>();
		axeHandPosistion = GameObject.Find("axeMountPoint").transform;
		shotgunHandPosition = GameObject.Find("shotgunMountPoint").transform;
		axeHolster = GameObject.Find("axeHolster").transform;
		shotgunHolster = GameObject.Find("shotgunHolster").transform;

		WeaponList[1].weaponTransform.position = shotgunHolster.transform.position;
		WeaponList[1].weaponTransform.rotation = shotgunHolster.transform.rotation;
		WeaponList[1].weaponTransform.parent = shotgunHolster;

		WeaponList[0].weaponTransform.position = axeHolster.transform.position;
		WeaponList[0].weaponTransform.rotation = axeHolster.transform.rotation;
		WeaponList[0].weaponTransform.parent = axeHolster;

        rigidBodies = GetComponentsInChildren<Rigidbody>();//our character is a ragdoll, lets collect its rigidbodies        
        foreach (Rigidbody rb in rigidBodies){//lets make all reigidbodies of the character kinematic
            rb.isKinematic = true;
        foreach (Collider col in ragDallColliders){
               col.enabled = false;//turn on all colliders of the character
            }
        }
    }	
	// Update is called once per frame
	void LateUpdate () {
		if(alive){
			hydratation -= Time.deltaTime/10;//speed of hydratation level decrease
			_speed = Input.GetAxis("Vertical");//reading vertical axis input
			_strafe = Input.GetAxis("Horizontal");//reading horizontal axis input
			_run = Input.GetKey(KeyCode.LeftShift) ? true : false;//check if run button was pressed
			_crouch = Input.GetKey(KeyCode.LeftControl) ? true : false;//check if run button was pressed
			_jump = Input.GetButton("Jump") ? true : false;//check if jump button was pressed
	//check if we're running;
			if (_run &&(Input.GetAxis("Horizontal"))==0){  
				t += Time.deltaTime;
			}
			else {     
				t -= Time.deltaTime;
			}
			t = Mathf.Clamp(t, 0, 1);
			if(Input.GetAxis("Vertical")>=0){
				_speed += t;
			}
			if(Input.GetAxis("Vertical")<0){
				_speed -= t;
			}
			if (_run &&(Input.GetAxis("Vertical"))==0) {
				x += Time.deltaTime;		
			} else {
				x-=Time.deltaTime;		
			}
			x = Mathf.Clamp (x, 0, 1);
			if(Input.GetAxis("Horizontal")>=0){
				_strafe += x;
			}
			if (Input.GetAxis ("Horizontal") < 0) {
				_strafe -= x;
			}	
			if (_speed > 1.5f && !_run && (Input.GetAxis("Horizontal")==0)) {//check if we've stopped
				_stop = true;		
					} 
			else {
				_stop = false;
					}
			if (_speed < -1.5f && !_run && (Input.GetAxis("Horizontal")==0)) {
				_stopBack = true;		
			} 
			else {
				_stopBack = false;
			}
//PROCESSING ROTATION
			Vector3 aimPoint =  Camera.main.transform.forward*10f;
			if(!atWall && canRotate){
				Quaternion targetRotation = Quaternion.LookRotation(aimPoint);
				this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10* Time.deltaTime);
				this.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			}
			if (!rifleAiming) {//
				rotY += Input.GetAxis ("Mouse Y") * Time.deltaTime * AimYSpeed;
				rotY = Mathf.Clamp (rotY, -1, 1);
				} else {
					rotY = 0;		
			}
//PROCESS SHOOTING
			if (currentWeapon.magazine > 0 && rifleAiming) {
				if (Input.GetMouseButton (0)) {
					if (shootTime <= Time.time) {
						shootTime = Time.time + currentWeapon.fireRate;
						ShootRay ();
						shooting = true;
						if (usingRifle) {							
							_animator.SetTrigger("SGSReload");// lunch shotgun shoot reload animation
							ShotGun.GetComponent<Animator>().SetTrigger("reload");//lunch shotgun reload animation
							ShotGunShot();
						}
						currentWeapon.magazine -= 1;
						if (currentWeapon.magazine < 0) {
							currentWeapon.magazine = 0;
						}
						if (currentWeapon.magazine == 0 && currentWeapon.totalAmmo > 0) {
							shooting = false;
							_animator.SetTrigger ("ReloadWeapon");	
						}
					}
					else {
						shooting = false;
					}
				} 
				else{
					shooting = false;
				}
			}
//APPLYING GRAVITY TO CHARACTER WHEN IN THE AIR
				moveDirection.y -= gravity * Time.deltaTime;
				if(_charCtrl.enabled){
					_charCtrl.Move (moveDirection * Time.deltaTime);
					if(moveDirection.y < -15f){
						moveDirection.y = -15f;
					}
				}
			if (health <= 0 || hydratation <=0) {
				alive = false;
				Die ();
			}
			///setting mecanim parameters
			_animator.SetBool("OnHorse", _getOnHorse);
            _animator.SetBool("OnBike", _getOnBike);            
            _animator.SetFloat("Speed", _speed);
			_animator.SetFloat("Strafe", _strafe);
			_animator.SetBool("Stop", _stop);
			_animator.SetBool("Jump", _jump);
			_animator.SetBool("KickDoor", _kickDoor);
			_animator.SetBool("JumpOverObstcle", _jumpOverObstcle);
			_animator.SetBool("JumpUpHigh",_jumpUpHigh);
			_animator.SetFloat("AimHeight",rotY);
			_animator.SetBool("StopBack", _stopBack);
			_animator.SetBool("Aiming", rifleAiming);
			_animator.SetBool("Run", _run);
			_animator.SetBool("ChoppingTree", chopTree);
			_animator.SetBool("GetInCar", _getInCar);
			_animator.SetBool("GetInLowCar", _getInLowCar);
			_animator.SetBool("Crouch", _crouch);
			_animator.SetBool("UsingRifle", usingRifle);
			_animator.SetBool("AtThewall", atWall);
			_animator.SetBool("SlideToCover", _canSlideToCover);
			_animator.SetBool("Glider", _glider);
			if (!_ridingHorse){
				_animator.SetBool ("Grounded", isGrounded ());
			}
            if (currBike != null){
                _animator.SetFloat("BikeSpeed", currBike.GetComponent<BikeControl>().CurrentSpeed);
            }

            //SLIDING TO THE CLOSEST COVER LOGIC
            Vector3 ahead = transform.forward;
			Vector3 rayStart = new Vector3(this.transform.position.x, this.transform.position.y+1f, this.transform.position.z);
			Ray	ray = new Ray(rayStart, ahead);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 10f)){
				if(hit.transform.gameObject.name == ("wall")){
					float distToCover = Vector3.Distance(hit.transform.position, transform.position);
					if(distToCover > 5f && Input.GetKeyDown(KeyCode.E)){
						_canSlideToCover = true;						
					}
					else{
						_canSlideToCover = false;
					}
					Debug.DrawLine (ray.origin, hit.point, Color.blue);
				}
				else{
					_canSlideToCover = false;
				}
			}           
//JUMP OVER OBSTACLE
			if(Physics.Raycast(ray, out hit, 1f)){
				if(hit.transform.gameObject.CompareTag("wall")){
					if(Input.GetButton("Jump")){
						_jumpOverObstcle = true;
					}
					else{
						_jumpOverObstcle = false;
					}
				}
//GET ON HORSE
				if(hit.transform.gameObject.name == ("horse")){
					currentHorse = hit.transform.gameObject;
					mountPoint = currentHorse.transform.Find("horseMountPoint");
					if((!_getOnHorse)&&(Input.GetKey(KeyCode.E))){						
						_getOnHorse = true;						
						this.transform.position = mountPoint.position;
						this.transform.rotation = mountPoint.rotation;
						StartCoroutine(getOnHorse());
					}
				}
//JUMP UP HIGH
				if(hit.transform.gameObject.CompareTag("jumpUpZone")){
					if(Input.GetButton("Jump")){
                        _jumpUpHigh = true;
					}
					else{
						_jumpUpHigh = false;
					}
				}

//GET IN THE CAR
				if(hit.transform.gameObject.CompareTag("carDoor")){
					currCar = hit.transform.gameObject;
					if((currCar!=null && !driving)){
						if (Input.GetKeyDown (KeyCode.E)) {
							if (!holdingJerry) {
								if (currCar.GetComponent<CarControl> ().lowDoors) {									
									if (_charCtrl.enabled && canRotate) {
										_getInLowCar = true;
										StartCoroutine (GetInCar ());
									}
								} else {									
									if (_charCtrl.enabled && canRotate) {
										_getInCar = true;	
										StartCoroutine (GetInCar ());
									}
								}
							} else {
								_animator.SetBool ("Fueling", true);
								canRotate = false;
								Transform FuelPos = currCar.transform.Find ("fuelPoint");
								this.transform.position = FuelPos.position;
								this.transform.rotation = FuelPos.rotation;
							}
						}
					}
				}
//OPEN THE DOOR
				if(hit.transform.gameObject.CompareTag("doorFront")){
					GameObject doorCollisionDetector = hit.transform.parent.gameObject;
					currDoor = doorCollisionDetector.transform.parent.gameObject;
						if(Input.GetKeyDown(KeyCode.E)&&(currDoor.GetComponent<DoorController>().closedDoor)){
							_kickDoor = true;
							StartCoroutine(OpenDoor(doorSide = 1));
						}
					}
				if(hit.transform.gameObject.CompareTag("doorBack")){
					GameObject doorCollisionDetector = hit.transform.parent.gameObject;
					currDoor = doorCollisionDetector.transform.parent.gameObject;
						if(Input.GetKeyDown(KeyCode.E)&&(currDoor.GetComponent<DoorController>().closedDoor)){
							_kickDoor = true;
							StartCoroutine(OpenDoor(doorSide = 2));
						}
					}
//GET IN TO THE GLIDER
				if(hit.transform.gameObject.CompareTag("glider")){
					currGlide = hit.transform.gameObject;
					if(Input.GetKeyDown(KeyCode.E) && currGlide!=null){
						if(currGlide.GetComponent<FlightScript>().gliderBroken!=true){
							_animator.SetTrigger ("StopAll");
							TurnOffCollider ();						
							GameObject sitPoint = currGlide.transform.Find("sitPoint").gameObject;
							this.transform.position = sitPoint.transform.position;
							this.transform.rotation = sitPoint.transform.rotation;
							this.transform.parent = sitPoint.transform;
							_glider  = true;
							gliding = true;
						}
					}
				}				
				else{
					currGlide = null;			
				}
//GET ON THE BIKE
                if (hit.transform.gameObject.CompareTag("bike")) {                    
                    if (Input.GetKeyDown(KeyCode.E) && currBike == null) {                        
                        currBike = hit.transform.gameObject;
                        if (currBike.GetComponent<BikeControl>().canSit)
                        {
                            _getOnBike = true;
                            StartCoroutine(getOnBike());
                        }                   
                    }
                }      
			}
            if (currBike && !_getOnBike) {
                float distToBike = Vector3.Distance(currBike.transform.position, transform.position);
                if (distToBike > 2f) {
                    currBike = null;
                }
            }
				if(Input.GetKeyDown(KeyCode.E)){
					if(driving){
						GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;						
						driving = false;
						_getInCar = false;
						_getInLowCar = false;
					}
//PICK JERRYCAN
				if (curJerrycan!=null && !holdingJerry) {
					float distToCan = Vector3.Distance(curJerrycan.transform.position, transform.position);
					if(distToCan<1.5f){
					    if(curJerrycan.GetComponent<Jerrycan>().hasGas){//check if canister has gas
						    _animator.SetTrigger ("StopAll");
						    usingAxe = false;
						    usingRifle = false;
						    _animator.SetBool ("PickJerry", true);
						    }
					    }
				    }
			    }
//GETTING OFF THE HORSE
			if((_ridingHorse)&&(_getOnHorse)){
				if(Input.GetKey(KeyCode.F)){
					_getOnHorse = false;
					currentHorse.GetComponent<HorseControl>().canControll = false;
					this.transform.parent = null;
					StartCoroutine(getOffHorse());
				}
			}
//GETTING OFF THE BIKE
            if (currBike != null) {
                if (handScrpt.handIk) {
                    if (Input.GetKeyDown(KeyCode.E)&& currBike.GetComponent<BikeControl>().CurrentSpeed < 1)
                    {
                        _getOnBike = false;
                        StartCoroutine(getOffBike());
                    }
                }
            }
//MOVING ALONG THE WALL LOGIC
			if((Input.GetKeyDown(KeyCode.E))&&(currentWall)&&(!atWall)){
				transform.rotation = currentWall.transform.rotation;//face player to the wall
				atWall = true;
				currentWallAngleY = currentWall.transform.rotation.eulerAngles.y;
			}
			if(atWall){
				transform.rotation = currentWall.transform.rotation;//face player to the wall
				if(Input.GetKeyDown(KeyCode.H)){//
					atWall = false;

                }
			}
//AT WALL LOGIC
			if(atWall && usingRifle){
				if(Input.GetMouseButtonDown(1)){
					rifleAiming= true;
                }
				if(Input.GetMouseButtonUp(1)){
					rifleAiming= false;
                }
			}
			if(!atWall && rifleAiming){
				rifleAiming = false;
			}
//WEAPON SELECTION
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				if(!usingAxe){	
					if (holdingJerry) {
						_animator.SetTrigger ("SGSReload");
						curJerrycan.transform.parent = null;
						curJerrycan.GetComponent<Rigidbody> ().isKinematic = false;
						curJerrycan.GetComponent<BoxCollider> ().enabled = true;
						holdingJerry = false;
					}
					StartCoroutine(SwapAxe());
				}
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)){
				if(!usingRifle){
					if (holdingJerry) {
						curJerrycan.transform.parent = null;
						curJerrycan.GetComponent<Rigidbody> ().isKinematic = false;
						curJerrycan.GetComponent<BoxCollider> ().enabled = true;
						holdingJerry = false;
					}
					StartCoroutine(SwapRifle());
				}
			}
			if (usingRifle && !atWall && canRotate && !_run) {
							if (Input.GetMouseButton (1)) {
									rifleAiming = true;
							}
							if (Input.GetMouseButtonUp (1)) {
									rifleAiming = false;
							}
					}
		}
        if (atWall || driving || gliding || _getOnHorse) {//DO NOT PROCESS IK LOOK AT IN THESE CONDITIONS
            lookIKweight = 0;
        }
        else
        {
            lookIKweight = 1;
        }
	}
//TRIGGER DETECTION
	void OnTriggerEnter(Collider trigg){
		if(trigg.gameObject.name == "wall"){//if we've triggered the wall behind which we can hind
			currentWall = trigg.gameObject;//make this wall the ONE we'll hide at
			if((_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == SlideToCoverState)||(_animator.GetNextAnimatorStateInfo(0).fullPathHash == SlideToCoverState)){
				transform.rotation = currentWall.transform.rotation;
				atWall = true;
				currentWallAngleY = currentWall.transform.rotation.eulerAngles.y;
			}
		}
		if(trigg.gameObject.name == "jerrycan"){
			if (curJerrycan == null) {
				curJerrycan = trigg.gameObject.transform.root.gameObject;	
			}
		}
	}
//TRIGGER EXIT
	void OnTriggerExit(Collider trigg){
		if(trigg.gameObject.name == "wall"){
			atWall = false;
			currentWall = null;
		}
		if (trigg.gameObject.name == "jerrycan") {
			if (!holdingJerry) {
				curJerrycan = null;
			}
		}
	}
//GETTING IN THE CAR COROUTINE
	IEnumerator GetInCar(){
		hideAllweapon ();
		_charCtrl.enabled = false;
		canRotate = false;
		_animator.SetTrigger ("StopAll");
		yield return new WaitForSeconds(0.1f);
		GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
		carCamPoint = currCar.transform.Find ("carCamPoint").gameObject;
		currCar.GetComponent<CarControl>().openDoor = true;
		this.transform.parent = sitPoint.transform;
		this.transform.position = sitPoint.transform.position;
		this.transform.rotation = sitPoint.transform.rotation;
		yield return new WaitForSeconds(4.5f);
		driving = true;
		currCar.GetComponent<CarControl>().openDoor = false;
		Transform Seat = currCar.transform.Find("seat");
		this.transform.parent = Seat.transform;
		this.transform.position = Seat.transform.position;
		this.transform.rotation = Seat.transform.rotation;
	}
//GETTING OFF THE CAR COROUTINE
    IEnumerator GetOutCar(){
		currCar.GetComponent<CarControl>().engineStopped = true;       
        GameObject sitPoint = currCar.transform.Find("sitPoint").gameObject;
		carCamPoint = null;
		currCar.GetComponent<CarControl>().openDoor = true;
		yield return new WaitForSeconds(5f);
		_charCtrl.enabled = true;
		canRotate = true;
		driving = false;
		currCar.GetComponent<CarControl>().openDoor = false;
		this.transform.position = sitPoint.transform.position;
		this.transform.rotation = sitPoint.transform.rotation;
		sitPoint = null;
		if(usingRifle){
			_animator.SetLayerWeight(1, 1f);
		}

		this.transform.parent = null;
	}
//DOOR OPENNING COROUTINE
    IEnumerator OpenDoor(int doorSide){
		canRotate = false;
		if(doorSide ==1){
			currDoor.GetComponent<DoorController>().breakDoorForward = true;
			currDoor.GetComponent<DoorController>().closedDoor = false;
		}
		if(doorSide ==2){
			currDoor.GetComponent<DoorController>().breakDoorBackward = true;
			currDoor.GetComponent<DoorController>().closedDoor = false;
		}
		currDoor.GetComponent<DoorController>().DoorSound();
		yield return new WaitForSeconds(0.5f);
		_kickDoor = false;
		canRotate = true;
	}
//SWITCHING TO AXE
	IEnumerator SwapAxe(){
		WeaponList[1].weaponTransform.position = shotgunHolster.transform.position;
		WeaponList[1].weaponTransform.rotation = shotgunHolster.transform.rotation;
		WeaponList[1].weaponTransform.parent = shotgunHolster;
		_animator.SetBool("SwapAxe", true);
		usingAxe = true;
		usingRifle = false;
		currentWeapon = WeaponList[0];	
		yield return new WaitForSeconds(0.5f);			
		WeaponList[0].weaponTransform.position = axeHandPosistion.transform.position;
		WeaponList[0].weaponTransform.rotation = axeHandPosistion.transform.rotation;
		WeaponList[0].weaponTransform.parent = axeHandPosistion.transform;	
		_animator.SetBool("SwapAxe", false);
	}
//SWITCHING TO SHOTGUN
	IEnumerator SwapRifle(){
		WeaponList[0].weaponTransform.position = axeHolster.transform.position;
		WeaponList[0].weaponTransform.rotation = axeHolster.transform.rotation;
		WeaponList[0].weaponTransform.parent = axeHolster;
		_animator.SetBool("SwapShotGun", true);
		currentWeapon = WeaponList[1];
		yield return new WaitForSeconds(0.5f);	
		usingAxe = false;
		usingRifle = true;
		WeaponList[1].weaponTransform.position = shotgunHandPosition.transform.position;
		WeaponList[1].weaponTransform.rotation = shotgunHandPosition.transform.rotation;
		WeaponList[1].weaponTransform.parent = shotgunHandPosition.transform;	
		_animator.SetBool("SwapShotGun", false);
	}
//GETTING ON HORSE COROUTINE
	IEnumerator getOnHorse(){
        TurnOffCollider();
        _animator.SetTrigger ("StopAll");
		if(usingRifle){
			_animator.SetLayerWeight(1, 1f);
		}
		_ridingHorse = true;
		yield return new WaitForSeconds(1.6f);
		currentHorse.GetComponent<HorseControl>().canControll = true;
		this.transform.parent = currentHorse.GetComponent<HorseControl>().backBone.transform;
		this.transform.position = currentHorse.GetComponent<HorseControl>().backBone.transform.position;
		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-0.25f, this.transform.position.z);
		this.transform.rotation = currentHorse.GetComponent<HorseControl>().horseSaddle.transform.rotation;
	}
//GETTING OFF HORSE COROUTINE
    IEnumerator getOffHorse(){
		yield return new WaitForSeconds(1.6f);
		_ridingHorse = false;
		this.transform.position = Vector3.Slerp(this.transform.position, mountPoint.position, 10* Time.deltaTime);
		this.transform.rotation = Quaternion.Slerp(transform.rotation, mountPoint.rotation, 10* Time.deltaTime);
		TurnOnCollider ();
	}
//GETTING ON BIKE COROUTINE
    IEnumerator getOnBike(){
            hideAllweapon();
        TurnOffCollider();
        _animator.SetTrigger("StopAll");
        yield return new WaitForSeconds(0.1f);
        Transform mountPoint = currBike.transform.Find("sitPoint");
        this.transform.position = mountPoint.position;
        this.transform.rotation = mountPoint.rotation;
        this.transform.parent = mountPoint.transform;
        yield return new WaitForSeconds(1.6f);
        this.transform.parent = currBike.transform;
        Transform sitPoint = currBike.transform.Find("sittingPoint");
        this.transform.position = sitPoint.position;
        this.transform.rotation = sitPoint.rotation;
        handScrpt.handIk = true;
        handScrpt.LeftHandPos = currBike.GetComponent<BikeControl>().LeftHandPos;
        handScrpt.RightHandPos = currBike.GetComponent<BikeControl>().RighthandPos;
        currBike.GetComponent<BikeControl>().canControl = true;           
    }
//GETTING OFF BIKE COROUTINE
    IEnumerator getOffBike(){
        currBike.GetComponent<BikeControl>().canControl = false;
        handScrpt.handIk = false;
        yield return new WaitForSeconds(1.6f);
        Transform mountPoint = currBike.transform.Find("sitPoint");
        this.transform.parent = null;
        this.transform.position = Vector3.Slerp(this.transform.position, mountPoint.position, 10 * Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, mountPoint.rotation, 10 * Time.deltaTime);
        currBike = null;
        TurnOnCollider();
    }
//HOLSTER ALL WEAPONS
    void hideAllweapon(){
		usingAxe = false;
		usingRifle = false;
		WeaponList[0].weaponTransform.position = axeHolster.transform.position;
		WeaponList[0].weaponTransform.rotation = axeHolster.transform.rotation;
		WeaponList[0].weaponTransform.parent = axeHolster;
		WeaponList[1].weaponTransform.position = shotgunHolster.transform.position;
		WeaponList[1].weaponTransform.rotation = shotgunHolster.transform.rotation;
		WeaponList[1].weaponTransform.parent = shotgunHolster;

	}
//SHOOTING LOGIC
	void ShootRay(){
		float x = Screen.width / 2;
		float y = Screen.height / 2;
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (x, y, 0));
		RaycastHit hit;
		GameObject bullet = Instantiate(currentWeapon.bulletPrefab, currentWeapon.weaponMuzzlePoint.position, Quaternion.identity) as GameObject;
		LineRenderer bulletTrail = bullet.GetComponent<LineRenderer> ();
		Vector3 startPos = currentWeapon.weaponMuzzlePoint.TransformPoint (Vector3.zero);
		Vector3 endPos = Vector3.zero;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			currentWeapon.weaponMuzzlePoint.transform.LookAt(hit.point);
			if(hit.transform.GetComponent<Rigidbody>()){
				Vector3 direction = hit.transform.position - transform.position;
					direction = direction.normalized;
				hit.transform.GetComponent<Rigidbody>().AddForce(direction * 200);
				}
			if(hit.collider.transform.root.transform.GetComponent<Enemy>()){
				if (hit.collider.gameObject.name == "Bip001 Head") {//headshot detection
					hit.collider.transform.root.transform.GetComponent<Enemy> ().ReceiveDamage (100);
					} else {
					hit.collider.transform.root.transform.GetComponent<Enemy> ().ReceiveDamage (20);
					}
				}
			if (hit.collider.GetComponent<glassBreaker>()) {
				hit.collider.GetComponent<glassBreaker> ().BreakerGlass();
				}
			
			endPos = hit.point;
		}
		else{			
			currentWeapon.weaponMuzzlePoint.transform.LookAt(ray.GetPoint(100));
			endPos = ray.GetPoint(100);
		}
		bulletTrail.SetPosition (0, startPos);
		bulletTrail.SetPosition (1, endPos);
	}
//TURNING CHARACTER CONTROLLER OFF
	void TurnOffCollider (){
		_charCtrl.enabled = false;
		atWall = false;
		currentWall = null;
		canRotate = false;
        lookIKweight = 0;
        if (usingRifle){
			_animator.SetLayerWeight(1, 0f);
		}
	}
//TURNING CHARACTER CONTROLLER ON
	void TurnOnCollider (){
		_charCtrl.enabled = true;
		_jumpOverObstcle = false;
		_jumpUpHigh = false;
		canRotate = true;
		chopTree = false;
        lookIKweight = 1;
        if (usingRifle){
			_animator.SetLayerWeight(1, 1f);
		}
	}
//GETTING OF GLIDER
	public void GetOfGlider (){
		_glider = false;
		gliding = false;
		currGlide = null;
		this.transform.parent = null;
		_charCtrl.enabled = true;
		canRotate = true;
		if(usingRifle){
			_animator.SetLayerWeight(1, 1f);
		}
	}
//CHECKING IF WE'RE GROUNDED
	bool isGrounded()	{
		return Physics.CheckSphere(transform.position, 0.5f, characterMask | 1<<9);
	}
	public void Die() {//dieing function (turning character to ragdoll)
		alive = false;
		_animator.SetLayerWeight(1, 0f);//setting torso layer to zero in case we're holding gun
		_animator.SetTrigger("Die");		
		_charCtrl.GetComponent<Collider>().enabled = false;
        //WE CAN TURN PLAYER TO RAGDOLL 
        //TurnToRagdoll();
    }
    //IK LOOK AT

    void OnAnimatorIK(int layerIndex){
        if (_charCtrl.enabled)
        {
            _animator.SetLookAtPosition(lookAtPos.position);
            _animator.SetLookAtWeight(lookIKweight, bodyWeight, headWeight, eyesWeight, clampWeight);
        }
    }
//FOOTSTEP SOUND FUNCTION IS CALLED FROM ANIMATION EVENT 
    void footStep()
    {
        if (isGrounded() && _charCtrl.enabled == true){
            //SHOOTING RAY DOWN
            Vector3 bottom = -transform.up;
            Vector3 btmRayStart = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
            Ray btmRay = new Ray(btmRayStart, bottom);
            RaycastHit hit;
            Debug.DrawRay(btmRayStart, transform.forward, Color.green);
            if (Physics.Raycast(btmRay, out hit, 2f)){
                if (hit.transform.gameObject.layer == 9){
                    if (terrSurfScrpt.surfaceIndex == 5)
                    {
                        int n = Random.Range(1, m_GroundFootstepSounds.Length);
                        m_AudioSource.clip = m_WaterFootSteps[n];
                        m_AudioSource.PlayOneShot(m_AudioSource.clip);
                        //			move picked sound to index 0 so it's not picked next time
                        m_WaterFootSteps[n] = m_WaterFootSteps[0];
                        m_WaterFootSteps[0] = m_AudioSource.clip;

                    }
                    else {
                        int n = Random.Range(1, m_GroundFootstepSounds.Length);
                        m_AudioSource.clip = m_GroundFootstepSounds[n];
                        m_AudioSource.PlayOneShot(m_AudioSource.clip);
                        //			move picked sound to index 0 so it's not picked next time
                        m_GroundFootstepSounds[n] = m_GroundFootstepSounds[0];
                        m_GroundFootstepSounds[0] = m_AudioSource.clip;
                    }
                }
                else {
                    int n = Random.Range(1, m_FootstepSounds.Length);
                    m_AudioSource.clip = m_FootstepSounds[n];
                    m_AudioSource.PlayOneShot(m_AudioSource.clip);
                    //			move picked sound to index 0 so it's not picked next time
                    m_FootstepSounds[n] = m_FootstepSounds[0];
                    m_FootstepSounds[0] = m_AudioSource.clip;
                }
            }
        }
    }
//SOUND FUNCTIONS ARE CALLED FROM CORRESPONDING ANIMATION EVENT 
    void landing(){
		m_AudioSource.clip = landingSound;
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
	}
	void openCarDoorSound(){
		m_AudioSource.clip = openCarDoor;
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
	}
	void closeCarDoorSound(){
		m_AudioSource.clip = closeCarDoor;
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
	}
    void PouringGasSound(){
        GameObject go = new GameObject("Audio");
        go.transform.position = curJerrycan.transform.position;
        go.transform.parent = curJerrycan.transform;        
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = poringGasSound;
        source.volume = 1f;
        source.Play();
        Destroy(go, source.clip.length);
    }
//PICKING UP JERRYCAN
    void jerrycanPick(){
		_animator.SetBool ("PickJerry", false);
		curJerrycan.GetComponent<Rigidbody> ().isKinematic = true;
		curJerrycan.GetComponent<BoxCollider> ().enabled = false;
		curJerrycan.transform.parent = jerrycanPosition.transform;
		curJerrycan.transform.position = jerrycanPosition.transform.position;
		curJerrycan.transform.rotation = jerrycanPosition.transform.rotation;
		holdingJerry = true;
	}
	void finishFuelling(){
		currCar.GetComponent<CarControl> ().fuel = 100;//fill up car gas tank
		_animator.SetBool ("Fueling", false);
		canRotate = true;
		curJerrycan.GetComponent<Jerrycan> ().hasGas = false;
		curJerrycan.transform.parent = null;
		curJerrycan.GetComponent<Rigidbody> ().isKinematic = false;
		curJerrycan.GetComponent<BoxCollider> ().enabled = true;
		_animator.SetTrigger ("SGSReload");
		holdingJerry = false;
	}
	private void ShotGunShot(){
		GameObject go = new GameObject("Audio");
		go.transform.position = currentWeapon.weaponMuzzlePoint.position;
		go.transform.parent = currentWeapon.weaponMuzzlePoint;
		//Create the source
		AudioSource source = go.AddComponent<AudioSource>();
		source.clip = shotGunShot;
		source.volume = 0.5f;
		source.Play();
		Destroy(go, source.clip.length);
	}
//TURN TO RAGDOLL
    public void TurnToRagdoll(float CurrentSpeed){//turning character to ragdoll
        this.transform.parent = null;       
        handScrpt.handIk = false;
        if (currBike != null)
        {
            currBike.GetComponent<BikeControl>().canControl = false;
            currBike.GetComponent<BikeControl>().stabilisationSpeed = 10;
        }    
        _getOnBike = false;
        _animator.enabled = false;//disable animation component
        TurnOffCollider();
        foreach (Collider col in ragDallColliders){
            col.enabled = true;//turn on all colliders of the character
        }
        foreach (Rigidbody rb in rigidBodies){
            rb.isKinematic = false;//make all charcters rigidbodies nonkinematic
        }
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.AddForce(transform.forward * CurrentSpeed*20);
        }
        currBike = null;
        StartCoroutine(GetUp()); 
    }
//GET UP TURN RAGDOLL OFF
    IEnumerator GetUp() {
        yield return new WaitForSeconds(2.5f);//waiting 
        if (!_animator.enabled){
            if (Physics.Raycast(PLayerSpine.transform.position, PLayerSpine.transform.up, 1)){
                faceDown = true;
            }
            else {
                faceDown = false;
            }
        }
        this.transform.position = new Vector3(PLayerSpine.transform.position.x, PLayerSpine.transform.position.y-0.2f, PLayerSpine.transform.position.z);//repositioning player according its spine bone position
        this.transform.eulerAngles = new Vector3(0, PLayerSpine.transform.eulerAngles.y+90, 0);        
        if (faceDown){
            _animator.SetTrigger("GetUpFaceDown");
        }
        else {
            _animator.SetTrigger("GetUpFaceUp");
        }
            _animator.enabled = true;
            yield return new WaitForSeconds(2f);//waiting for 1 sec
            TurnOnCollider();
        foreach (Collider col in ragDallColliders){
            col.enabled = false;//turn on all colliders of the character
        }
        foreach (Rigidbody rb in rigidBodies){
            rb.isKinematic = true;//make all charcters rigidbodies nonkinematic
        }
    }    
}
