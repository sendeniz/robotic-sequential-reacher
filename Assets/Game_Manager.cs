using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class Game_Manager : MonoBehaviour
{
    // defines a  Gameobject "agent" to be selected in the Unity envir.
    // this game object is identical to the original agent in the Reacher ML Agents example
    public GameObject agent;
    // defines a  Gameobject "hand" to be selected in the Unity envir.
    // this game object is identical to the original agent in the Reacher ML Agents example
    public GameObject hand;
    // define empty array of size 4 to collect objects in 
    public GameObject[] targetObjects = new GameObject[4];
    // defines a 4 by for matrix to initialize and determine activation status of targets
    Matrix4x4 sequence = new Matrix4x4();
    int gameSequence = 0;
    int failCounter = 0;
    public bool random_sequence = false;
    // define temporary color to change color of activated targets
    Color temp;

    float rewardToGive = 1;

    public GameObject parent;

    // Inter Stimulus Interval
    private IEnumerator coroutine;
    public GameObject empty;
   

    void Start()
    {

    }
   
    public void init()
    {
        coroutine = CoroutineAction();
        // targetObjects = GameObject.FindGameObjectsWithTag("Target");

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

            // identity matrix with zeros on its diagonals, which gives identiy/active state
            // of game objects to be touched in sequence
            sequence = Matrix4x4.identity;

            //sets the GoalOn Object of first sphere to Target Active. Used for the observation agent
            setFirstActive();
        }
        else if (random_sequence == true)
        {
            sequence = Shuffle(Matrix4x4.identity);
            // Debug.Log(sequence);
            setFirstActive();
        }


    }
    // Fisher-Yates shuffle
    private Matrix4x4 Shuffle(Matrix4x4 matrix)
    {
        var n = 4; // 4 by 4 matrix
        var shuffleLimit = 2; // n - 1 - 1 (no need to shuffle last element)
        for (var i = 0; i < shuffleLimit; i++)
        {
            int j = Random.Range(i, n);

            // Swap rows of the 4 by 4 matrix
            Vector4 temp = matrix.GetRow(i);
            matrix.SetRow(i, matrix.GetRow(j));
            matrix.SetRow(j, temp);
        }
        return matrix;
    }

    // Initialize First Target activation 
    private void setFirstActive()
    {
        for (int i = 0; i < 4; i++)
        {
            // iterate over the first row only and find column in the sequence matrix, where column i is equal to 1
            // this column then represents which of the four objects is the first active target
            // i.e. first row [0,0,1,0], 3rd is first active
            if (sequence[0, i] == 1)
            {
                // if condition fulfilled, set target to active
                targetObjects[i].transform.GetChild(0).gameObject.tag = "Active";
                //targetObjects[i].transform.GetChild(1).name = "Active";
                // set temp to color of active target i.e. blue, yellow etc.
                temp = targetObjects[i].GetComponent<Renderer>().material.color;
                targetObjects[i].GetComponent<Renderer>().material.color = new Color(224, 224, 224);
            }
        }
    }
    // check for active target/goal object
    // round represents which of the four target should be touched within
    // a round as defined in sequenc matrix above
    int checkActive(int round)
    {
        for (int j = 0; j < 4; j++)
        {
            // for each round check which target is active, that is which is equal to 1 in the sequence matrix
            // i.e. assume in the first round the first element indicates activitiy [1,0,0,9]
            // thus the first row first col needs to be select with sequence[0,0] index. As such for the
            // first round as described round=0 and j=0, is the index where target is equal to 1.
            // In round two second target is active [0,1,0,0], that is sequence[1,1].
            // As such only the index j is requiered to return activity status of the target.
            if (sequence[round, j] == 1)
            {
                return j;
            }
        }
        // return 0  never occurs. Why do we use it? The method needs an int value as a return.
        // As before the activation matrix is an identity (or shufled ) matrix with 1 on the diagonals, and 1
        // represents an active target state. Since one target will always be active as 1, we wil
        // return j instead of 0. Normally we would include a return null command, however c#
        // and unity does not support this. As such we use return 0.
        return 0;
    }
    
    //When a sphere is touched the triggered method is called
    public void triggered(GameObject touchedSphere)
    {
        // if more than than 1 target object (in our case we have 4 target objects)
        if (targetObjects.Length > 1)
        {
            // Console logs for meta information
            // DebugLog for touched sphere
            // Debug.Log("Touched = " + touchedSphere);
            // DebugLog for the active sphere that should be touched
            // Debug.Log("active Object = " + targetObjects[checkActive(gameSequence)]);
            // check if agent touches the active, correct target object
            if (touchedSphere == targetObjects[checkActive(gameSequence)])
            {
                // and reward if correct target has been touched
                //agent.GetComponent<ReacherAgent>().AddReward(1.0f);
                agent.GetComponent<ReacherAgent>().AddReward(rewardToGive);


                // then move onto the next round in the sequence
                // Debug.Log(failCounter); // print fail counter
                Debug.Log("rewarded by " + rewardToGive); // print message if correct target has been touched				
                gameSequence++; // move one ahead in the sequence
                rewardToGive = 1.0f;
                failCounter = 0;
                // sets the tag of GoalOn objct of the touchedSphere to Untagged
                touchedSphere.transform.GetChild(0).gameObject.tag = "Untagged";
                // touchedSphere.transform.GetChild(1).name = "Unactive";
                // set the color of the target back to its original
                touchedSphere.GetComponent<Renderer>().material.color = temp;
                empty.tag = "Active";
                StartCoroutine(CoroutineAction());
            }
            else
            {
                //if wrong sphere is touched fail counter increments 
                failCounter++;
                //TODO: fail increment per frame. needs to be reduced to 1 touch per collision 
            }
        }
    }

    public GameObject getActive()
    {
        return GameObject.FindGameObjectsWithTag("Active")[0];
    }

    private void initializeNewRound()
    {
        sequence = Shuffle(Matrix4x4.identity);
        setFirstActive();
        //Debug.Log(sequence);
    }

    void Update()
    {
        // every frame find and collect Targets
        // targetObjects = GameObject.FindGameObjectsWithTag("Target");
        int c = 0;
        foreach (Transform child in parent.transform)
        {
            if (child.tag == "Target")
            {
                targetObjects[c] = child.gameObject;
                c++;
            }
        }
        // deduct reward per frame/timestep
        //Debug.Log("deducted");

        rewardToGive = rewardToGive * 0.99f;
        rewardToGive = Math.Max(0.1f, rewardToGive);

        // Box Mueller Transformation to simulate values from a gaussian normal distribution 
        System.Random rand = new System.Random(); 
        double u1 = 1.0 - rand.NextDouble(); //uniform distribution [0,1]
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double mean = 0; // mean mu
        double stdDev = 0.1; // standard deviation sigma 
        double randNormal = mean + stdDev * randStdNormal; // generate values from the normal
        float gen_val = (float)randNormal; // convert to float for position change per frame in vector below
        //Debug.Log(gen_val);
        // add gaussian noise distributed with mean=0,sigma = 0.1 to input vector during intertrial interval
        //empty.transform.localPosition = new Vector3(0 , -4 , 0);
        empty.transform.localPosition = new Vector3(0 + gen_val, -4 + gen_val, 0 + gen_val);
    }

    // Counts frames for the intertrial interval
    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }

    // Coroutine defines the scheduling or execution time of code during the intertrial interval
    public IEnumerator CoroutineAction()
    {

        if (gameSequence > 3)
        {
            gameSequence = 0;
            failCounter = 0;

            yield return StartCoroutine(WaitFor.Frames(20)); // wait for 20 frames
                                                             // do some actions after 20 frames
            empty.tag = "Untagged";
            //Debug.Log(gameSequence);

            // Start a new round
            initializeNewRound();
        }
        // else if it is not the last ball
        else
        {
            yield return StartCoroutine(WaitFor.Frames(20)); // wait for 20 frames
                                                             // do some actions after 20 frames
            empty.tag = "Untagged";
            // Debug.Log(gameSequence);
            int active = checkActive(gameSequence);
            // sets the tag of the next balls GoalOn objct Active
            targetObjects[active].transform.GetChild(0).gameObject.tag = "Active";
            // targetObjects[active].transform.GetChild(1).name = "Active";
            // save its color to temp
            temp = targetObjects[checkActive(gameSequence)].GetComponent<Renderer>().material.color;
            // let it light up
            targetObjects[active].GetComponent<Renderer>().material.color = new Color(224, 224, 224);

        }

    }

}