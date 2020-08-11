using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
	public GameObject agent;
    public GameObject hand;
	//Array of all goal objects labled with tag "Target"
	public GameObject[] targetObjects = new GameObject[4];
	int []active = new int [4];
    int[,] sequence = new int[4,4];
    int gameSequence = 0;
	int failCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
		agent.transform.parent = gameObject.transform;
		// identity matrix with zeros on its diagonals, which gives identiy/active state
        // of game objects to be touched in sequence

        // active state of first target (green) others inactive
        sequence[0,0] = 1;
        sequence[0,1] = 0;
        sequence[0,2] = 0;
        sequence[0,3] = 0;

        // active state of second target (yellow) others inactive
        sequence[1,0] = 0;
        sequence[1,1] = 1;
        sequence[1,2] = 0;
        sequence[1,3] = 0;

        // active state of third target (blue) others inactive
        sequence[2,0] = 0;
        sequence[2,1] = 0;
        sequence[2,2] = 1;
        sequence[2,3] = 0;

        // active state of fourth target (red) others inactive
        sequence[3,0] = 0;
        sequence[3,1] = 0;
        sequence[3,2] = 0;
        sequence[3,3] = 1;
		
		targetObjects = GameObject.FindGameObjectsWithTag("Target");

    }

    // check for active target/goal object
    // round represents which of the four target should be touched within
    // a round as defined in sequenc matrix above
	int checkActive(int round)
    {
            for (int j = 0; j < 4; j++)
            {
            // check for each round which target is active, that is which is equal to 1
            // example: round 1 sequence[0,0] is active, so for round=0 and j=0, it is
            // equal to target 1 is thus active sequence[0,1] is in active in round on
            // so it is not returned etc. etc. in round two second target is active,
            // that is sequence[1,1] since we only need the last index j to indentify
            // the target, we return j
                if (sequence[round, j] == 1)
                {
                    return j;
                }
            }
        // this never occurs. Why do we use it? The method needs an int value as a return value.
        // Since the activation matrix is an identity matrix with 1 on the diagonals, and 1
        // represents an active target state. Since one target will always be active as 1, we wil
        // return j instead of 0. Normally we would include a return null command, however c#
        // and unity does not support this. As such we use return 0.
        return 0;
       
    }
	
	public void triggered(Collider other)
    //void OnTriggerStay(Collider other)
    {
        int c = 0; // counter c
        foreach(GameObject gameObject in targetObjects){ 
            targetObjects[c] = gameObject;
            c++;
        }
        c = 0;

        // if more than than 1 target object (in our case we have 4 target objects)
        if (targetObjects.Length > 1)
        {
            // check if agent touches the active, correct target object
            if (gameObject == targetObjects[checkActive(gameSequence)]) {
                // and reward if correct target has been touched
                agent.GetComponent<ReacherAgent>().AddReward(0.01f);
                // then move onto the next round in the sequence
				Debug.Log(failCounter); // print fail counter
                Debug.Log("rewarded"); // print message if correct target has been touched
                gameSequence++; // move one ahead in the sequence
				failCounter = 0; 
            }          

        }
        // else if number of targets is equal to 1 as in the original task perform the same
        // but for a task with only a single object
        else
        {
            if (other.gameObject == hand)
            {
                agent.GetComponent<ReacherAgent>().AddReward(0.01f);
            }
			else 
			{
				failCounter++;
			}
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        targetObjects = GameObject.FindGameObjectsWithTag("Target");
    }
}
