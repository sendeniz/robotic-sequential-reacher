using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define gameobject targetobj to be selected in the unity UI.
/// We use this target to clone it n number of times, with n = int numTargets = 3
/// totalling to 1 original + 3 cloned targets.
/// The gameobject goal is the green target sphere, which we clone.
/// The random location parameter is a boolen to be set true or false in the UI
/// and determines whether all target objects will initalize at a random location
/// within the reach of the reacher agent.
/// The vector center is the center location of the agent and size determines the
/// radial dimension of where targets will be randomly located if random location is
/// set to true. 
/// </summary>
public class Gen_Target : MonoBehaviour
{
    public GameObject Targetobj;
    public Game_Manager GameManager;
    public GameObject goal;
    public GameObject agent;
    public bool RandomLocation = false;
    public GameObject parent;
    public GameObject[] targetObjects = new GameObject[4];
    private Vector3 center;
    public Vector3 size;
    int numTargets = 3;
    int i = 0;

    void Start()
    {

    }

    /// <summary>
	/// Initialization method, which either 1) initialized targets at random
	/// location within the spheric reach of the agent or 2) at a fixed position equidistant to
	/// the reacher agent. 
    /// </summary>
    public void init()
    {
        if (RandomLocation == true)
        {
            center = transform.Find("Agent").transform.position;
            //randomize location of initial goal/target object
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            goal.transform.position = pos;
        }
        else if (RandomLocation == false)
        {
            Vector3 pos = new Vector3(8.5f + agent.transform.position.x, -3.1f + agent.transform.position.y, 0.9f + agent.transform.position.z);
        }

        while (i != numTargets)
        {
            SpawnTarget(i);
            i = i + 1;
        }
    }


    /// <summary>
    /// Defines the color of each cloned target and spawn targets either at a fixed or random location.
	/// Compared to the above method, which is only called at the initialisaiton phase, this method
	/// is called later on as well to randomize location after a sucessfully round (i.e., all targets
	/// have been toucehd in the correct order). We take the center of the agent and randomize location
	/// based on x,y,z coordinates within the agents reach. 
    /// </summary>
    Color[] colors = { Color.yellow, Color.blue, Color.red };
    string[] colorNames = { "Yellow", "Blue", "Red" };

    public void SpawnTarget(int i)
    {
        if (RandomLocation == true)
        {
            center = transform.Find("Agent").transform.position;
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            GameObject newObject = Instantiate(Targetobj, pos, Quaternion.identity);
            newObject.transform.parent = Targetobj.transform.parent;
            newObject.tag = "Target";
            newObject.GetComponent<Renderer>().material.color = colors[i];
            newObject.transform.name = colorNames[i];
        }
        else if (RandomLocation == false)
        {
            Vector3 pos = new Vector3(8.5f + agent.transform.position.x, -3.1f + agent.transform.position.y, 0.9f + agent.transform.position.z);
            switch (i)
            {
                case 0:
                    pos = new Vector3(-8.5f + agent.transform.position.x, -3.1f + agent.transform.position.y, 0.9f + agent.transform.position.z);
                    break;
                case 1:
                    pos = new Vector3(0 + agent.transform.position.x, -3.1f + agent.transform.position.y, -8.5f + agent.transform.position.z);
                    break;
                case 2:
                    pos = new Vector3(0 + agent.transform.position.x, -3.1f + agent.transform.position.y, 8.5f + agent.transform.position.z);
                    break;
            }
            
            GameObject newObject = Instantiate(Targetobj, pos, Quaternion.identity);
            newObject.transform.parent = Targetobj.transform.parent;
            newObject.tag = "Target";
            newObject.GetComponent<Renderer>().material.color = colors[i];
            newObject.transform.name = colorNames[i];
        }
    }

    /// <summary>
    /// Defines a translucent red sphere around the agent, which represents its reaching space
	/// and the space in which targets can be randomly allocated within if rando location is set true.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        center = transform.Find("Agent").transform.position;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(center, 14);
    }

    /// <summary>
    /// Defines operations to be done every timestep or frame. Collects all target location with respect
	/// to its parent to ensure that target objects are not searched globally but only
	/// within its own parent. 
    /// </summary>
    void Update()
    {
        center = transform.Find("Agent").transform.position;
        if (GameManager.SequenceEnd == true)
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
            for (int i = 0; i < 4; i++)
            {
                if (RandomLocation == true)
                {
                    center = transform.Find("Agent").transform.position;
                    Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
                    targetObjects[i].transform.position = pos;
                    GameManager.SequenceEnd = false;
                }
            }
        }
    }
}
