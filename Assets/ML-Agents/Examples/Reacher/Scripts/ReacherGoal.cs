using UnityEngine;
using System.Collections;
using System.Threading;

public class ReacherGoal : MonoBehaviour
{
    public GameObject hand;
    public GameObject goalOn;
	public GameObject Random_Target_Gen;
	private IEnumerator coroutine;
	

    void OnStart() {
		coroutine = waiter();
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
		//if collision with hand
		if (other.gameObject == hand)
		{
			//send Game_Manager Script the touched GameObject with the triggered method
			StartCoroutine(waiter());
		}
	}
	
	private IEnumerator waiter()
	{
		//Wait for 2 seconds
		yield return new WaitForSecondsRealtime(1);
		Random_Target_Gen.GetComponent<Game_Manager>().triggered(gameObject);
	}
}
