using UnityEngine;

public class ReacherGoal : MonoBehaviour
{
    public GameObject agent;
    public GameObject hand;
    public GameObject goalOn;
    public GameObject[] testObject = new GameObject[4];
    int []active = new int [4];
    int[,] sequence = new int[4,4];
    int gameSequence = 0;

    void OnStart() {
        // matrix which gives identiy of game objects to be touched in sequence
        sequence[0,0] = 1;
        sequence[0,1] = 0;
        sequence[0,2] = 0;
        sequence[0,3] = 0;

        sequence[1,0] = 0;
        sequence[1,1] = 1;
        sequence[1,2] = 0;
        sequence[1,3] = 0;

        sequence[2,0] = 0;
        sequence[2,1] = 0;
        sequence[2,2] = 1;
        sequence[2,3] = 0;

        sequence[3,0] = 0;
        sequence[3,1] = 0;
        sequence[3,2] = 0;
        sequence[3,3] = 1;
    }

    //obj_count = GameObject.FindGameObjectsWithTag("Target").length;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == hand)
        {
            goalOn.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == hand)
        {
            goalOn.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

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

    void OnTriggerStay(Collider other)
    {
        int c = 0; // counter c
        foreach(GameObject test in GameObject.FindGameObjectsWithTag("Target")){
            testObject[c] = test;
            c++;
        }
        c = 0;

        if (testObject.Length > 1)
        {
            // Do SMT

      
            if (gameObject == testObject[checkActive(gameSequence)]) {
                // reward if correct target has been touched
                agent.GetComponent<ReacherAgent>().AddReward(0.01f);
                // then if rewarded and the correct target has been touched
                // move onto the next round in the sequence
                gameSequence++;
            }
            //Debug.Log(testObject[3]);
            //Debug.Log(gameObject);
            //Debug.Log(testObject[2]);
            //Debug.Log(testObject[3]);
            

        }
        else
        {
            if (other.gameObject == hand)
            {
                agent.GetComponent<ReacherAgent>().AddReward(0.01f);
            }
        }
        
    }
}
