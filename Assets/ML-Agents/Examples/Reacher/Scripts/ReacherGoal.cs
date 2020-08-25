using UnityEngine;
using System.Collections;
using System.Threading;

public class ReacherGoal : MonoBehaviour
{
    public GameObject hand;
    public GameObject goalOn;
	public GameObject Random_Target_Gen;
	
    void OnStart() {
    }

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
		if (other.gameObject == hand)
		{
			Random_Target_Gen.GetComponent<Game_Manager>().triggered(gameObject);

		}
	}
}