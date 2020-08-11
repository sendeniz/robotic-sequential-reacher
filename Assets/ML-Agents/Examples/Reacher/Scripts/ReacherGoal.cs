using UnityEngine;

public class ReacherGoal : MonoBehaviour
{
    public GameObject hand;
    public GameObject goalOn;
	//public Game_Manager game_Manager;
	public GameObject Random_Target_Gen;
	

    void OnStart() {

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
	void OnTriggerStay(Collider other)
	{
		//game_Manager.triggered(other);
		Random_Target_Gen.GetComponent<Game_Manager>().triggered(other);
	}

}
