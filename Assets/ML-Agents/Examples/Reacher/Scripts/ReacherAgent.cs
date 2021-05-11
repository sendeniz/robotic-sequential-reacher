using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;

public class ReacherAgent : Agent
{
    public GameObject pendulumA;
    public GameObject pendulumB;
    public GameObject hand;
    public GameObject goal;
    public GameObject active;
    public Game_Manager GameManager;
    float m_GoalDegree;
    Rigidbody m_RbA;
    Rigidbody m_RbB;
    // speed of the goal zone around the arm (in radians)
    float m_GoalSpeed = 0;
    // radius of the goal zone
    float m_GoalSize;
    // Magnitude of sinusoidal (cosine) deviation of the goal along the vertical dimension
    float m_Deviation;
    // Frequency of the cosine deviation of the goal along the vertical dimension
    float m_DeviationFreq;
    public GameObject agent;
    private Vector3 size = new Vector3(14,14,14);
    private Vector3 center;
    private Vector3 target_observations;

    EnvironmentParameters m_ResetParams;

    /// <summary>
    /// Collect the rigidbodies of the reacher in order to resue them for
    /// observations and actions.
    /// </summary>
    public override void Initialize()
    {
        m_RbA = pendulumA.GetComponent<Rigidbody>();
        m_RbB = pendulumB.GetComponent<Rigidbody>();

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        SetResetParameters();
        // Custom init to resolve initalisaiton conflict that result in errors
        gameObject.transform.parent.GetComponent<Gen_Target>().init();
        gameObject.transform.parent.GetComponent<Game_Manager>().init();
        // Call get.Active from Game Manager, because Game Manager is initlized after Reacher Agent
        active = gameObject.transform.parent.GetComponent<Game_Manager>().getActive();
        // collect center location 
        center = agent.transform.localPosition;
    }

    /// <summary>
    /// We collect the normalized rotations, angularal velocities, and velocities of both
    /// limbs of the reacher as well as the relative position of the target and hand.
	/// That is, here the observations for the input vector are collected.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // if the ITI is active suspend the active target observation vector
        if (iti_active == true)
        {
            // joint coordinates (vec. 3)
            sensor.AddObservation(pendulumA.transform.localPosition);
            // rotation of joint (quant. 4)
            sensor.AddObservation(pendulumA.transform.rotation);
            // (vec 3)
            sensor.AddObservation(m_RbA.angularVelocity);
            // (vec 3)
            sensor.AddObservation(m_RbA.velocity);
            // joint coordinates (vec 3)
            sensor.AddObservation(pendulumB.transform.localPosition);
            // rotation of joint (quat. 4)
            sensor.AddObservation(pendulumB.transform.rotation);
            // (vec 3)
            sensor.AddObservation(m_RbB.angularVelocity);
            // (vec 3)
            sensor.AddObservation(m_RbB.velocity);
            // (vec 3) replaces active target coordinates in observations vector
            target_observations = center + new Vector3(Random.Range(-size.x / 4, size.x / 4), Random.Range(-size.y / 4, size.y / 4), Random.Range(-size.z / 4, size.z / 4));
            // Debug.Log("Intertrial interval observation vector" + target_observations);
            sensor.AddObservation(target_observations);
            // hand cooardiantes (vec 3)
            sensor.AddObservation(hand.transform.localPosition); 
            // vec (1) Goals are non-moving, therefore initalised with zeros
            sensor.AddObservation(m_GoalSpeed);
            // total number of params. in input vector = 33

        }
        // if the ITI is false use the regular observation vector including active target location 
        else if (iti_active == false)
        {
            // joint coordinates (vec. 3)
            sensor.AddObservation(pendulumA.transform.localPosition);
            // rotation of joint (quat. 4)
            sensor.AddObservation(pendulumA.transform.rotation);
            // (vec 3)
            sensor.AddObservation(m_RbA.angularVelocity);
            // (vec 3)
            sensor.AddObservation(m_RbA.velocity);
            // joint coordinates (vec 3)
            sensor.AddObservation(pendulumB.transform.localPosition);
            // rotation of joint (quat. 4)
            sensor.AddObservation(pendulumB.transform.rotation);
            // (vec 3)
            sensor.AddObservation(m_RbB.angularVelocity);
            // (vec 3)
            sensor.AddObservation(m_RbB.velocity);
            // (vec 3) of target coordinates
            // Debug.Log(iti_active);
            target_observations = active.transform.parent.localPosition;
            // Debug.Log("Regular observation vector" + target_observations);
            sensor.AddObservation(target_observations);
            // hand cooardiantes (vec 3)
            sensor.AddObservation(hand.transform.localPosition);
            // vec (1) Goals are non-moving, therefore initalised with zeros
            sensor.AddObservation(m_GoalSpeed);
            // total number of params. in input vector = 33
        }
    }

    /// <summary>
    /// The agent's four actions correspond to torques on each of the two joints.
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        m_GoalDegree += m_GoalSpeed;
        //UpdateGoalPosition();

        var torqueX = Mathf.Clamp(vectorAction[0], -1f, 1f) * 150f;
        var torqueZ = Mathf.Clamp(vectorAction[1], -1f, 1f) * 150f;
        m_RbA.AddTorque(new Vector3(torqueX, 0f, torqueZ));

        torqueX = Mathf.Clamp(vectorAction[2], -1f, 1f) * 150f;
        torqueZ = Mathf.Clamp(vectorAction[3], -1f, 1f) * 150f;
        m_RbB.AddTorque(new Vector3(torqueX, 0f, torqueZ));
    }

    /// <summary>
    /// Used to move the position of the target goal around the agent.
    /// </summary>
    //void UpdateGoalPosition()
    //{
    //    var radians = m_GoalDegree * Mathf.PI / 180f;
    //    var goalX = 8f * Mathf.Cos(radians);
    //    var goalY = 8f * Mathf.Sin(radians);
    //    var goalZ = m_Deviation * Mathf.Cos(m_DeviationFreq * radians);
    //    goal.transform.position = new Vector3(goalY, goalZ, goalX) + transform.position;
    //}

    /// <summary>
	/// Defines the intertrial interval in terms of time_steps for the duration an interval
	/// denoted as time_step_interval. This interval is adjustable in the Unity environment
	/// itself, however should be moves somewhere more visible. Currently it can be found
	/// attached to the Agent. A more appropriate position would be the Game Manager.
	/// <summary>
    private int time_steps = 0;
    public bool iti_active = false;
    IEnumerator WatchForEnoughSteps(int time_steps_interval)
    {
        while (time_steps < time_steps_interval)
        {
            yield return null;
        }
        time_steps = 0;
        iti_active = false;
    }

    /// <summary>
	/// Defines operations conducted each time step. Here we update the time_step counter for the
	/// intertrial interval and randomly sample position within the agents reach to replace the
	/// active target coordinates for when the inter trial interval is active. Replacement is necessary
	/// for 2 reasons: 1) to ensure that the observations vector has the same length and suspended active
	/// target coordinates are not padded to zero when the intertrial interval is active; 2) to add noise
	/// to the experimental set up and see whether a centering strategy still emerges.
    /// <summary>
    void Update()
    {
        // time step counter
        time_steps++;
        // Debug.Log(time_steps);
        // Debug.Log("ITI 1"+iti_active);
        // Debug.Log(target_observations);
        // observation noise vector for when the intertrial interval is active
        // Vector3 noise_observations = center + new Vector3(Random.Range(-size.x / 4, size.x / 4), Random.Range(-size.y / 4, size.y / 4), Random.Range(-size.z / 4, size.z / 4));
        // Debug.Log(noise_observations);
        // collect active target
        active = gameObject.transform.parent.GetComponent<Game_Manager>().getActive();
        if (GameManager.collision == true)
        {   
            GameManager.collision = false;
            iti_active = true;
            // Debug.Log("ITI 2"+iti_active);
            // initalises the intertrial interval
            time_steps = 0;
            StartCoroutine(WatchForEnoughSteps(250));
            // Debug.Log("ITI 3" + iti_active);
        }
        else
        {
            // do nothing
        }
    }
    /// <summary>
    /// Resets the position and velocity of the agent and the goal.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        pendulumA.transform.position = new Vector3(0f, -4f, 0f) + transform.position;
        pendulumA.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        m_RbA.velocity = Vector3.zero;
        m_RbA.angularVelocity = Vector3.zero;

        pendulumB.transform.position = new Vector3(0f, -10f, 0f) + transform.position;
        pendulumB.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        m_RbB.velocity = Vector3.zero;
        m_RbB.angularVelocity = Vector3.zero;

        m_GoalDegree = Random.Range(0, 360);
        // UpdateGoalPosition();

        SetResetParameters();

        goal.transform.localScale = new Vector3(m_GoalSize, m_GoalSize, m_GoalSize);
    }

    public void SetResetParameters()
    {
        m_GoalSize = m_ResetParams.GetWithDefault("goal_size", 5);
        // m_GoalSpeed = Random.Range(-1f, 1f) * m_ResetParams.GetWithDefault("goal_speed", 1);
        m_Deviation = m_ResetParams.GetWithDefault("deviation", 0);
        m_DeviationFreq = m_ResetParams.GetWithDefault("deviation_freq", 0);
    }
}
