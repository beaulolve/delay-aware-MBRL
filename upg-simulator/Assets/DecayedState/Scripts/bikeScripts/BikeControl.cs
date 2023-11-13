using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BikeControl : MonoBehaviour {

    private GameObject Player;
    private CharacterControl playrScrpt;
    public AudioSource additionalAS;
    public AudioClip engineLoopSound;
    public AudioClip engineBikeStart;
    public AudioClip engineBikeStop;
    private AudioSource m_HighAccel; // Source for the high acceleration sounds
    public float engineVolume;
    public float pitchMultiplier = 1f;                                          // Used for altering the pitch of audio clips
    public float lowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
    public float lowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
    public float highPitchMultiplier = 0.25f;
    public bool m_StartedSound; // flag for knowing if we have started sounds
    public bool engineOn;
    public Transform frontCollider;
    public Transform steeringWheel; //wheel bar
    public Transform LeftHandPos;
    public Transform RighthandPos;
    public bool canSit;
    public bool canControl;
    public bool forceStabilisation;
    public float stability = 0.3f;
    public float stabilisationSpeed = 2.0f;
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
    public float AccelInput { get; private set; }
    public float BrakeInput { get; private set; }
    private float m_SteerAngle;
    private float distToPlayer;
    public float CurrentSteerAngle { get { return m_SteerAngle; } }
    public float MaxSpeed { get { return m_Topspeed; } }
    public float m_CurrentTorque;
    public float chassisTorque = 50f;
    private int m_GearNum;
    private float m_GearFactor;
    private Rigidbody m_Rigidbody;
    private float maxRolloffDistance = 120;
    public float Revs;

    [SerializeField]
    private float m_MaximumSteerAngle;
    [SerializeField]
    private WheelCollider[] m_WheelColliders = new WheelCollider[2];    
    [SerializeField]
    private float m_MaxHandbrakeTorque;
    [SerializeField]
    private float m_BrakeTorque;
    [SerializeField]
    private float m_ReverseTorque;
    [SerializeField]
    private float m_Topspeed = 200;
    [SerializeField]
    private static int NoOfGears = 5;
    [SerializeField]
    private float m_Downforce = 100f;
    [SerializeField]
    private float m_RevRangeBoundary = 1f;

    // Use this for initialization
    void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        playrScrpt = Player.GetComponent<CharacterControl>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

	void FixedUpdate () {        
        if (canControl){
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float handbrake = Input.GetAxis("Jump");           
            Move(h, v, v, handbrake);
            forceStabilisation = false;

            Vector3 rayStart = new Vector3(frontCollider.transform.position.x, frontCollider.transform.position.y, frontCollider.transform.position.z);
            Ray ray = new Ray(rayStart, transform.forward);
            RaycastHit hit;
            Debug.DrawRay(rayStart, transform.forward, Color.red);
            if (Physics.Raycast(ray, out hit, 1f))
            {
                if (CurrentSpeed > 20)
                {
                    playrScrpt.TurnToRagdoll(CurrentSpeed);
                    canControl = false;
                }
            }
        }
        if (m_WheelColliders[1].isGrounded || forceStabilisation)
        {
            Vector3 predictedUp = Quaternion.AngleAxis(m_Rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilisationSpeed, m_Rigidbody.angularVelocity) * transform.up;
            Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            m_Rigidbody.AddTorque(torqueVector * stabilisationSpeed * stabilisationSpeed);
        }
        if (!canControl){
            m_WheelColliders[1].brakeTorque = m_MaxHandbrakeTorque;
            distToPlayer = Vector3.Distance(Player.transform.position, transform.position);
            if (distToPlayer < 3f)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    stabilisationSpeed = 40;
                    forceStabilisation = true;
                }
            }
        }
        //rotate steering wheel
        steeringWheel.rotation = m_WheelColliders[0].transform.rotation * Quaternion.Euler(steeringWheel.transform.rotation.x, 35, m_WheelColliders[0].steerAngle);
//<-SOUND        
        // start the sound if not playing and it is nearer than the maximum distance
        // get the distance to main camera
        float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;
        if (!m_StartedSound && camDist < maxRolloffDistance){
            StartSound();
        }
        if (m_StartedSound && engineOn)
        {
            // The pitch is interpolated between the min and max values, according to the car's revs.
            float pitch = ULerp(lowPitchMin, lowPitchMax, Revs);

            // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
            pitch = Mathf.Min(lowPitchMax, pitch);

            m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            m_HighAccel.volume = engineVolume;
            m_HighAccel.spatialBlend = 1;            
        }

    }
    public void Move(float steering, float accel, float footbrake, float handbrake) {
        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        //Set the steer on the front wheels.
        //Assuming that wheels 0 is the front wheel.
        m_SteerAngle = steering * m_MaximumSteerAngle;
        m_WheelColliders[0].steerAngle = m_SteerAngle;

        ApplyDrive(accel, footbrake);

        //Set the handbrake.
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * m_MaxHandbrakeTorque;
            m_WheelColliders[0].brakeTorque = hbTorque;
            m_WheelColliders[1].brakeTorque = hbTorque;
        }
        else {
            m_WheelColliders[0].brakeTorque = 0;
            m_WheelColliders[1].brakeTorque = 0;
        }
        CalculateRevs();
        GearChanging();
        AddDownForce();
    }
    private void ApplyDrive(float accel, float footbrake){

        float thrustTorque;       
        thrustTorque = accel * (m_CurrentTorque );
            for (int i = 0; i < 2; i++)
                {
                    m_WheelColliders[i].motorTorque = thrustTorque;
                }      

        for (int i = 0; i < 2; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
            {
                m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
            }
            else if (footbrake > 0)
            {
                m_WheelColliders[i].brakeTorque = 0f;
                m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
            }
        }
    }
    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

        if (m_GearNum > 0 && f < downgearlimit)
        {
            m_GearNum--;
        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {
            m_GearNum++;
        }
    }
    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }
    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    }


    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }
    // this is used to add more grip in relation to speed
    private void AddDownForce() {
        m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
            m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
    }
    private void StartSound()
    {
        if (playrScrpt.currBike == this.gameObject){
            StartCoroutine(playEngineSound());
        }

    }
    // sets up and adds new audio source to the gane object
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;

        // start the clip from a random point
        source.time = Random.Range(0f, clip.length);
        source.Play();
        source.minDistance = 5;
        source.maxDistance = maxRolloffDistance;
        source.dopplerLevel = 0;
        return source;
    }
    // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
    IEnumerator playEngineSound()
    {
        additionalAS.clip = engineBikeStart;
        additionalAS.Play();    
        m_StartedSound = true;    
        yield return new WaitForSeconds(additionalAS.clip.length);
        engineOn = true;        
        m_HighAccel = SetUpEngineAudioSource(engineLoopSound);
    }

}
