using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gen_Target : MonoBehaviour
{
    // defines arbtriary to be selected by user Gameobject called "Targetprefab"
    public GameObject Targetobj;  // Taret object to be cloned or generated within the cubic space at random locations
    public GameObject goal; // select initial goal to initialize different starting point for it within the cubpic space at a random location

    // define two vectors of size 3 to store x,y,z location, centered and sized
    //according to the size of the empty game object, which defines all possible
    // locaiton in 3D space for 4 targets to spawn randomly

    public Vector3 center;
    public Vector3 size;


    // Define parameters for targets, since we start with an inital target, if
    // total number of targets desired is 4, numTargets = 3 + 1 initial target = 4, and so on.
    int numTargets = 3;
    int i = 0;

    void Start()
    {
        //randomize location of initial goal/target object
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        goal.transform.position = pos;

        while (i != numTargets)
        {
            SpawnTarget();
            i = i + 1;
            }
    }

    // define a color map for targets
    Color[] colors = {Color.yellow, Color.blue, Color.red};
	string[] colorNames = {"Yellow", "Blue", "Red"}; 
	
    public void SpawnTarget()
    {
        // takes center of empty square game object, plus minus max ranged divided 2 for x,y,z to generate
        // numTargets within the space at random location
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        GameObject newObject = Instantiate(Targetobj, pos, Quaternion.identity);
		// newObject sets parent to parent of Targetobj
		newObject.transform.parent = Targetobj.transform.parent;
        // assignes tag = "Target" for each generated target
        newObject.tag = "Target";
        // change color of Targets
        newObject.GetComponent<Renderer>().material.color = colors[i];
		newObject.transform.name = colorNames[i];
    }



    // Defines color, cube object form of the empty game object
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
