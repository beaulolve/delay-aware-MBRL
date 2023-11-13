using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private UnityEngine.AI.NavMeshAgent agent;

	public Collider [] ragDallColliders;//array of character's collider
	public Component[] rigidBodies;//array of character's rigidbodies
	public AudioClip[] aggroSound;
	public AudioClip[] hitSound;
	public AudioClip feedingSound;
	private float attackTime = 1;
	private float attackTimeReset = 1;
	public float health;//health count
	public float idlingTime = 3f;//time we're going to spend for idling
	public float idleTimeReset = 3f;//idlinf time reseter
	public float walkRadius = 10f;
	private int speed = 6;
	private int attackID = 0;
	private float timer = 1f;//time we're going to spend searching player
	private float timeCheck;//

	public bool feeding;
	public bool aggro;
	public bool alive = true;//i am alive
	public bool idling = false;//i am idling
	private EnemySences mySences;
	private AudioSource source;
	private CharacterControl playerScript;
	public enum State {//enumeration of states
		Idle,
		Patrol,
		Attack,
		Aggro,
		Feed,
	}
	public State state;
	private Animator _animator;
	private GameObject Player;//Player

	private Vector3 target;//coordinates of the target
	private Vector3 moveDirection;//movement direction
	private Vector3 lookDirection;//look direction
	private Vector3 distanceToTarget;//distance to target

	void Start () {		
///////CACHING VARIABLES
		_animator = GetComponent<Animator> ();
		mySences = GetComponent<EnemySences>();
		source = GetComponent<AudioSource>();
		agent = (UnityEngine.AI.NavMeshAgent)this.GetComponent("NavMeshAgent");
		Player = (GameObject)GameObject.FindGameObjectWithTag("Player");
		playerScript = Player.GetComponent<CharacterControl>();
		rigidBodies = GetComponentsInChildren<Rigidbody>();//our character is a ragdoll, lets collect its rigidbodies
		state = Enemy.State.Idle;//starting state set to idle
		StartCoroutine("FSMach");//start our state machine	
		foreach (Rigidbody rb in rigidBodies){//lets make all reigidbodies of the character kinematic
			rb.isKinematic = true;
		}	
	}

	private IEnumerator FSMach(){
		while(alive){//if character is still alive
			switch(state){
			case State.Patrol:
				Patrol();
				break;
			case State.Attack:
				Attack();
				break;
			case State.Idle:
				Idle();
				break;
			case State.Aggro:
				Aggro();
				break;
			case State.Feed:
				Feed();
				break;
			}
			yield return null;
		}
	}
	private void Patrol(){//patrol state
		Moving();
		speed = 1;
		if(mySences.playerInSight){
			idling = false;//we're not idling anymore
			state = Enemy.State.Aggro;//our current state is attack
			source.clip = aggroSound[Random.Range(0,aggroSound.Length)];
			source.Play();
		}
		if(Time.time - timeCheck > 5.0){ //if 5 sec. passed
			Vector3 randomDirection = Random.insideUnitSphere * walkRadius;			
			randomDirection += transform.position;
			UnityEngine.AI.NavMeshHit hit;
			UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
			Vector3 finalPosition = hit.position;
			target = finalPosition;
			timeCheck = Time.time;//time check equal to absolute time
			timer++;//increase timer by 1
			}
		if(timer>5){//while timer is lesser than 5
			state = Enemy.State.Idle; // changing state to patrol
			timer=0;//zeroing timer
			}
		}
	private void Attack(){//attack state
		idling = false;//we're not idling anymore
		speed=1;//change movement speed from 0 to 1
		target = Player.transform.position; // Player is current target
		Moving(); //run movement function
		if(((Player.transform.position - this.transform.position).magnitude < 1.8f)){//if distance to player is lesser than 2 or character is hitted
			speed=0;//stop movement
			lookDirection = target - this.transform.position;  // calculating look  direction (towards target)
			lookDirection.y = 0; // restricting Y axis rotation
			Quaternion newRot = Quaternion.LookRotation (lookDirection);//rotation that looks along forward with the the head upwards along upwards
			this.transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * 3); //smooth linear interpolation from our current rotation to new rotation
			if (attackTime > 0){//while attack time is greater than zero 
				attackTime -= Time.deltaTime;//descrease attack timer by Time.deltaTime
                attackID = 0;

            }
			if (attackTime < 0){ 
				attackTime = 0; //don't make attack timer lesser than 0
			}
			if (attackTime == 0){//when attack timer became zero
				attackTime = attackTimeReset;//lets reset attack timer
				attackID = Random.Range(1,3);
				}
		}
		else{//if distance to player is bigger than 2 and character is not hitted
			speed=1;//move character faster
		}
		if(((Player.transform.position - this.transform.position).magnitude > 30f)){//if player escaped from character (distance between tham greater than 20)
			state = Enemy.State.Idle;//switch state to Idle 
		}
		if(!playerScript.alive){
			state = Enemy.State.Feed;
		}
	}
	private void Idle(){//idling state
		speed = 0;
		idling = true;
		if(idling){
			if (idlingTime > 0)//while idling time is greater than 0
				idlingTime -= Time.deltaTime;//descrease idling time by Time.deltaTime
			if(mySences.playerInSight){
				idling = false;//we're not idling anymore
				state = Enemy.State.Aggro;//our current state is attack
				source.clip = aggroSound[Random.Range(0,aggroSound.Length)];
				source.Play();
			}
			if (idlingTime < 0) 
				idlingTime = 0;//idling time can not be lesser than 0
			if (idlingTime == 0)//when idling time become 0
			{
				idling = false;////we're not idling anymore
				state = Enemy.State.Patrol;//we're patroling
				idlingTime = idleTimeReset;//reset our idling timer
				speed = 1;//change movement speed from 0 to 2
			}
		}
	}
	private void Aggro(){//hoarding state
		idling = true;
		speed = 0;
		aggro = true;
	}
//process movement
	private void Moving(){//movement function
		if(!idling){
			agent.SetDestination(target);
		}
	}
	public void SwitchToAttack(){//this function is lunched by animation event (zombieAggro)
		aggro = false;
		state = Enemy.State.Attack;//we're attacking
	}
	public void Feed(){
		idling = false;//we're not idling anymore
		speed=1;//change movement speed from 0 to 1
		target = Player.transform.position; // Player is current target
		if (((Player.transform.position - this.transform.position).magnitude < 0.5f)) {//if distance to player is lesser than 2 or character is hitted
			speed = 0;//stop movement
			lookDirection = target - this.transform.position;  // calculating look  direction (towards target)
			lookDirection.y = 0; // restricting Y axis rotation
			Quaternion newRot = Quaternion.LookRotation (lookDirection);//rotation that looks along forward with the the head upwards along upwards
			this.transform.rotation = Quaternion.Slerp (transform.rotation, newRot, Time.deltaTime * 3); //smooth linear interpolation from our current rotation to new rotation
				if (!feeding){
					_animator.SetTrigger ("Feed");
				}
			feeding = true;
			source.clip = feedingSound;
			source.Play();
		}
	}
	void FixedUpdate(){
        if (idling)
        {
            agent.isStopped = true;
        }
        else {
            agent.isStopped = false;
        }
        if (health <= 0) {
			alive = false;
			Die ();
		}
		_animator.SetInteger ("Speed", speed);
		_animator.SetBool ("Aggressive", aggro);
		_animator.SetInteger("AttackId", attackID);
	}
	public void Die() {//dieing function (turning character to ragdoll)
		agent.isStopped=true;
		alive = false;
		_animator.enabled = false;//disable animation component
		foreach (Collider col in ragDallColliders){
			col.enabled = true;//turn on all colliders of the character
		}
		foreach (Rigidbody rb in rigidBodies){
			rb.isKinematic = false;//make all charcters rigidbodies nonkinematic
		}
	}
	public void EnemySounds(){
		source.clip = hitSound[Random.Range(0,hitSound.Length)];
		source.Play();
	}
	//this function is executed by animation event in zombie attack animation clip;
	public void ApplyDamage(float dmg){
		if((Player.transform.position - this.transform.position).magnitude < 1.7f){
		Player.GetComponent<CharacterControl>().health -= dmg;
		}
	}
	public void ReceiveDamage(float dmg){	
		_animator.SetTrigger ("Hited");	
		health -= dmg;	
		idling = false;//we're not idling anymore
		state = Enemy.State.Attack;
	}
}
