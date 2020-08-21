using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // params for counter for decaying reward
    //public float timer = 0.0f;
    //int seconds;
    //int errortime;

    // Inter Stimulus Interval
    private IEnumerator coroutine;

    // Start intializes everything before the first frame update
    void Start()
    {
        //InvokeRepeating("OutputTime", 1f, 1f);  //1s delay, repeat every 1s
    }
   

    public void init()
    {

        targetObjects = GameObject.FindGameObjectsWithTag("Target");
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
            //Debug.Log("Touched = " + touchedSphere);
            // DebugLog for the active sphere that should be touched
            //Debug.Log("active Object = " + targetObjects[checkActive(gameSequence)]);
            // check if agent touches the active, correct target object
            if (touchedSphere == targetObjects[checkActive(gameSequence)])
            {
                // and reward if correct target has been touched
                agent.GetComponent<ReacherAgent>().AddReward(0.01f);

                // Somewhere here 20 Timestep inter-stimulus interval (see Coroutine below, not complete yet)
                // StartCoroutine(waiter());

                // then move onto the next round in the sequence
                // Debug.Log(failCounter); // print fail counter
                Debug.Log("rewarded"); // print message if correct target has been touched				
                gameSequence++; // move one ahead in the sequence


                //if we touched the last ball
                if (gameSequence > 3)
                {
                    // Debug.Log("newRound");
                    // sets the tag of GoalOn objct of the touchedSphere to Untagged
                    touchedSphere.transform.GetChild(0).gameObject.tag = "Untagged";
                    // set the color of the target back to its original
                    touchedSphere.GetComponent<Renderer>().material.color = temp;
                    // reset counters
                    gameSequence = 0;
                    failCounter = 0;
                    // Start a new round
                    initializeNewRound();
                }
                // else if it is not the last ball
                else
                {
                    failCounter = 0;
                    // since game sequence was set one ahead, checks which is the next active ball
                    int active = checkActive(gameSequence);
                    //sets the tag of GoalOn objct of the touchedSphere to Untagged
                    touchedSphere.transform.GetChild(0).gameObject.tag = "Untagged";
                    touchedSphere.GetComponent<Renderer>().material.color = temp;
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

    // Defines waiter for 20 Timestep inter-stimulus interval (see above )
    // coroutine however needs to include parts or whole chunk of code from above
    // maybe other way than coroutine?
    //private IEnumerator waiter()
    //{
    //    // Wait for 20 seconds Time steps
    //    yield return new WaitForSecondsRealtime(20);
    //}

    float elapsed = 0f;
    void Update()
    {
        // every frame find and collect Targets
        targetObjects = GameObject.FindGameObjectsWithTag("Target");

        // for every second deduce reward
        elapsed += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;
            deductReward();
        }

    }

    void deductReward()
    {
        // deduct a reward every second timestep
        agent.GetComponent<ReacherAgent>().AddReward(-0.01f);
        Debug.Log("deducted");
    }

}