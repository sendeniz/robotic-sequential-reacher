using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class Game_Manager : MonoBehaviour
{
    public GameObject agent;
    public GameObject hand;
    // empty array of size 4 to collect target objects 
    public GameObject[] targetObjects = new GameObject[4];
    // 4x4 matrix to initialize and determine activation status of targets
    Matrix4x4 sequence = new Matrix4x4();
    int gameSequence = 0;
    int failCounter = 0;
    public bool random_sequence = false;
    Color temp;
    float rewardToGive = 1;
    public GameObject parent;
    public bool collision = false;
    public bool sequence_end = false;
    public bool curriculum_learning = false;
    public ReacherAgent Reacher_Agent;
    
    void Start()
    {
    }
   
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

        if (random_sequence == false)
        {
            // set empty game object "Random_Gen_Target" as parent of Agent
            agent.transform.parent = gameObject.transform;
            sequence = Matrix4x4.identity;
            //sets the GoalOn Object of first sphere to Target Active
            setFirstActive();
        }
        else if (random_sequence == true)
        {
            sequence = Yates.Shuffle(Matrix4x4.identity);
            setFirstActive();
        }
    }

    // Initialize First Target activation:
    // Iterates over the first row and finds col i = 1
    private void setFirstActive()
    {
        for (int i = 0; i < 4; i++)
        {
            if (sequence[0, i] == 1)
            {
                // if condition fulfilled, set target to active
                targetObjects[i].transform.GetChild(0).gameObject.tag = "Active";
                //targetObjects[i].transform.GetChild(1).name = "Active";
                
                temp = targetObjects[i].GetComponent<Renderer>().material.color;
                targetObjects[i].GetComponent<Renderer>().material.color = new Color(224, 224, 224);
            }
        }
    }

    // for each round check which target is active
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
    
    public void triggered(GameObject touchedSphere)
    {
        // if more than than 1 target object 
        if (targetObjects.Length > 1)
        {
            if (touchedSphere == targetObjects[checkActive(gameSequence)])
            {
                collision = true;
                // and reward if correct target has been touched
                agent.GetComponent<ReacherAgent>().AddReward(rewardToGive);
                Debug.Log("rewarded by " + rewardToGive);				
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
                    sequence_end = true;
                    if (random_sequence == false)
                    {
                        initializeFixedRound();
                    }
                    else if (random_sequence == true)
                    {
                        initializeNewRound();
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

    private void initializeNewRound()
    {
        sequence = Matrix4x4.identity;
        sequence = Yates.Shuffle(sequence);
        setFirstActive();
    }

    private void initializeFixedRound()
    {
        sequence = Matrix4x4.identity;
        setFirstActive();
    }

    private int time_steps_c = 0;
    void Update()
    {
        // Debug.Log("Sensor ball location1:" + hand.transform.position);
        // Debug.Log("Sensor ball location2:" + hand.transform.parent.position);
        Vector3 target_pos = getActive().transform.parent.localPosition;
        // Vector3 a = getActive().transform.position;
        Vector3 sensor_pos = hand.transform.parent.localPosition;
        // Vector3 b = hand.transform.position;
        // Debug.Log("Target active locaiton:" + getActive().transform.parent.localPosition);
        // Debug.Log("Sensor ball location3:" + hand.transform.parent.localPosition);
        float diff_dist = Vector3.Distance(target_pos, sensor_pos);
        // Debug.Log("Distance(Sensor,Active)" + diff_dist);
        // Debug.Log(Distance(hand.transform.parent.localPosition, test));
        // Debug.Log("Sensor ball location4:" + hand.transform.localPosition);
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
        // Debug.Log(time_steps_c);
        if (curriculum_learning == false)
        {
            // Debug.Log("Regular learning active");
            rewardToGive = rewardToGive * 0.9995f;
            rewardToGive = Math.Max(0.5f, rewardToGive);
        }

        else if(curriculum_learning == true)
        {
            // Debug.Log("Curriculum learning active 1");
            
            if (time_steps_c >= 40000)
            {
                // Debug.Log("Regular learning active 2");
                float dist_pen = diff_dist * -0.00001f;
                rewardToGive = rewardToGive * 0.9995f + dist_pen;
                rewardToGive = Math.Max(0.5f, rewardToGive);
                // Debug.Log(dist_pen);
            }
            else
            {
                rewardToGive = rewardToGive * 0.9995f;
                rewardToGive = Math.Max(0.5f, rewardToGive);
            }
        }

    }
}