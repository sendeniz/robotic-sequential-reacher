using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// gameObject[] defines an array of size 4 to collect target objects in.
/// 4x4 matrix to initialize and determine activation status of targets, since we use
/// Fisher-Yates Shuffle. 
/// </summary>
public class Game_Manager : MonoBehaviour
{
    public GameObject agent;
    public GameObject hand;
    public GameObject[] targetObjects = new GameObject[4];
    Matrix4x4 sequence = new Matrix4x4();
    int gameSequence = 0;
    int failCounter = 0;
    public bool RandomSequence = false;
    Color temp;
    float rewardToGive = 1;
    public GameObject parent;
    public bool collision = false;
    public bool SequenceEnd = false;
    public bool UseCurriculum = false;
    public ReacherAgent Reacher_Agent;
    private int time_steps_c = 0;
    public float DecayFactor;
    public float DisplacementFactor;
    public float minReward;
    public int StartCurriculumAfterTimestep;

    void Start()
    {

    }

    /// <summary>
	/// Collects target objects at initialisation and Random_Gen_Target as parent of Agent
	/// in agent.transform.parent = gameObject.transform. Furthermore, we setthe first target
	/// in the shuffled identtiy matrix to active with setFirstActive().
	/// <summary>
    public void init()
    {
        int c = 0;
        foreach (Transform child in parent.transform)
        {
            if (child.tag == "Target")
            {
                targetObjects[c] = child.gameObject;
                c++;
            }
        }

        if (RandomSequence == false)
        {
            
            agent.transform.parent = gameObject.transform;
            sequence = Matrix4x4.identity;
            
            setFirstActive();
        }
        else if (RandomSequence == true)
        {
            sequence = Yates.Shuffle(Matrix4x4.identity);
            setFirstActive();
        }
    }

    /// <summary>
	/// Initalizes the first target activation and iterates by iteration over the first row.
	/// Since the 1s in the identiy determine activationa dnt he identiy has been shuffeled
	/// iterating over the first row and finding the column where i = 1 represents the
	/// active target. If we find this target, the if condition is fulfilled and the object
	/// is set to active.
	/// <summary>
    private void setFirstActive()
    {
        for (int i = 0; i < 4; i++)
        {
            if (sequence[0, i] == 1)
            {
                targetObjects[i].transform.GetChild(0).gameObject.tag = "Active";
                temp = targetObjects[i].GetComponent<Renderer>().material.color;
                targetObjects[i].GetComponent<Renderer>().material.color = new Color(224, 224, 224);
            }
        }
    }

    /// <summary>
    /// Defines a method which for each round of the game checks which target is active.
    /// <summary>
    int checkActive(int round)
    {
        for (int j = 0; j < 4; j++)
        {
            if (sequence[round, j] == 1)
            {
                return j;
            }
        }
        return 0;
    }

    /// <summary>
    /// Defines a method which checks touch or collision between the sensor and the target object
	/// and manages the majority of the game, like initalizing a new round after the agent completed
	/// a sucessfuly round, sets targets to active and unactive after collision or touch and also
	/// manages the color of by setting the active target color to white and setting it back to
	/// its native color. Here we also define the reward the agent obtains if the correct target has been touched.
    /// <summary>
    public void triggered(GameObject touchedSphere)
    {
        // if more than than 1 target object 
        if (targetObjects.Length > 1)
        {
            if (touchedSphere == targetObjects[checkActive(gameSequence)])
            {
                collision = true;
                agent.GetComponent<ReacherAgent>().AddReward(rewardToGive);
                //Debug.Log("rewarded by " + rewardToGive);				
                gameSequence++;
                rewardToGive = 1.0f;
                failCounter = 0;
                // sets the tag of GoalOn objct of the touchedSphere to Untagged
                touchedSphere.transform.GetChild(0).gameObject.tag = "Untagged";
                // set the color of the target back to its original
                touchedSphere.GetComponent<Renderer>().material.color = temp;
                if (gameSequence > 3)
                {
                    gameSequence = 0;
                    failCounter = 0;
                    SequenceEnd = true;
                    if (RandomSequence == false)
                    {
                        initializeFixedRound();
                        SequenceEnd = false;
                        gameSequence = 0;
                    }
                    else if (RandomSequence == true)
                    {
                        initializeNewRound();
                        SequenceEnd = false;
                        gameSequence = 0;
                    }
                }
                else
                {
                    int active = checkActive(gameSequence);
                    // sets the tag of the next balls GoalOn objct Active
                    targetObjects[active].transform.GetChild(0).gameObject.tag = "Active";
                    // save its color to temp
                    temp = targetObjects[checkActive(gameSequence)].GetComponent<Renderer>().material.color;
                    // let it light up
                    targetObjects[active].GetComponent<Renderer>().material.color = new Color(224, 224, 224);
                }
            }
            else
            {
                // if wrong sphere fail counter increments 
                failCounter++;
                //TODO: fail increment per frame. needs to be reduced to 1 touch per collision 
            }
        }
    }

    public GameObject getActive()
    {
        Transform[] AllChildren = GetComponentsInChildren<Transform>();
        GameObject active_obj = null;
        List<Transform> active_obj_list = new List<Transform>();
        foreach (var child in AllChildren)
        {
            if (child.gameObject.tag == "Active")
            {
                active_obj_list.Add(child);
                active_obj = active_obj_list[0].gameObject;
                break;
            }
        }
        return active_obj;
    }

    /// <summary>
    /// Defines a method to initalize a new random round and shuffes the identity matrix using Yates Fisher Shuffle.
    /// <summary>
    private void initializeNewRound()
    {
        sequence = Matrix4x4.identity;
        sequence = Yates.Shuffle(sequence);
        setFirstActive();
    }

    /// <summary>
    /// Defines a method to a deterministic fixed sequence round.
    /// <summary>
    private void initializeFixedRound()
    {
        sequence = Matrix4x4.identity;
        setFirstActive();
    }

    /// <summary>
    /// Defines operations to be execute every timestep. Time steps are counted.
	/// Collects target objects every timestep and defines the curriculum learning
	/// schedule. Here we also govern the reward decay structure, which we reduce
	/// by a factor of .995 and also add an additive displacement penality of -.001
	/// for the second lesson in curriculim learning. The second lesson of the curriculum
	/// learning starts after 400.000 timesteps which is equal to 100 epochs.
    /// <summary>
    void Update()
    {
        Debug.Log(sequence);
        Vector3 target_pos = getActive().transform.parent.localPosition;      
        Vector3 sensor_pos = hand.transform.parent.localPosition;
        float diff_dist = Vector3.Distance(target_pos, sensor_pos);

        // every frame find and collect Targets
        int c = 0;
        foreach (Transform child in parent.transform)
        {
            if (child.tag == "Target")
            {
                targetObjects[c] = child.gameObject;
                c++;
            }
        }
        time_steps_c++;
        if (UseCurriculum == false)
        {
            if (Reacher_Agent.ItiActive == false)
            {
                //rewardToGive = rewardToGive * 0.995f;
                rewardToGive = rewardToGive * DecayFactor;
                rewardToGive = Math.Max(minReward, rewardToGive);
            }
            else if (Reacher_Agent.ItiActive == true)
            {
                rewardToGive = 1.0f;
            }

        }

        else if(UseCurriculum == true)
        {

            //if (time_steps_c >= 400000)
            if (time_steps_c >= StartCurriculumAfterTimestep)
            {
                if (Reacher_Agent.ItiActive == false)
                {

                    // float dist_pen = diff_dist * -0.001f;
                    float dist_pen = diff_dist * DisplacementFactor;
                    // rewardToGive = rewardToGive * 0.995f + dist_pen;
                    rewardToGive = rewardToGive * DecayFactor + dist_pen;
                    rewardToGive = Math.Max(minReward, rewardToGive);
                }

                else if (Reacher_Agent.ItiActive == true)
                {
                    rewardToGive = 1.0f;
                }

            }
            else
            {
                if (Reacher_Agent.ItiActive == false)
                {
                    // rewardToGive = rewardToGive * 0.995f;
                    rewardToGive = rewardToGive * DecayFactor;
                    rewardToGive = Math.Max(minReward, rewardToGive);
                }

                else if (Reacher_Agent.ItiActive == true)
                {
                    rewardToGive = 1.0f;
                }
                
            }
        }
    }
}