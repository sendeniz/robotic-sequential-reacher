using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gen_Target : MonoBehaviour
{
    // defines a  Gameobject "Targetprefab" to be selected in the Unity envir.
    // that is to be generated/cloned n number of times (see int numTargets=3)
    public GameObject Targetobj;
    // defines a Gameobject "goal", which is the initial green goal object in the ML agents reacher example
    public GameObject goal;

    // defines the condition whether target generation is at random locations or fixed to be select in Unity envir.
    public bool random_location = false;

    // define two vectors of size 3 to store x,y,z coordinates, centered and sized
    // according to the size of the empty game object, which defines all possible
    // locaiton in 3D space for 4 targets to spawn randomly
    // center used to determine middle point of the 3D cubic spawning area

    public Vector3 center;
    public Vector3 size;

    // Defines parameters for targets, since we start with 1 inital target, if
    // total number of targets desired is 4, numTargets = 3 new targets + 1 initial target = 4 total
    int numTargets = 3;
    int i = 0;

    void Start()
    {
        //if (random_location == true)
        //{
        //    //randomize location of initial goal/target object
        //    Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        //    goal.transform.position = pos;
        //}
        //else if (random_location == false)
        //{
        //    Vector3 pos = center + new Vector3(8, 0, 0);
        //    goal.transform.position = pos;
        //}
        
        //while (i != numTargets)
        //{
        //    SpawnTarget(i);
        //    i = i + 1;
        //    }
    }

    public void init() {

        if (random_location == true)
        {
            //randomize location of initial goal/target object
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            goal.transform.position = pos;
        }
        else if (random_location == false)
        {
            Vector3 pos = center + new Vector3(8, 0, 0);
            goal.transform.position = pos;
        }

        while (i != numTargets)
        {
            SpawnTarget(i);
            i = i + 1;
        }
    }

    // define a color map for targets
    Color[] colors = {Color.yellow, Color.blue, Color.red};
	string[] colorNames = {"Yellow", "Blue", "Red"}; 
	
    public void SpawnTarget(int i)
    {
        if (random_location == true) {
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
        else if (random_location == false) {
            // takes center of empty square game object, plus minus max ranged divided 2 for x,y,z to generate
            // numTargets within the space at random location
            //Vector3 pos = center + new Vector3((size.x / 2 )+i, (size.y / 2)+i, (size.z / 2)+i);
            Vector3 pos = center + new Vector3(8, 0, 0);
            switch (i)
            {
                case 0:
                    pos = center + new Vector3(-8 , 0, 0);
                    break;
                case 1:
                    pos = center + new Vector3(0, 0, -8 );
                    break;
                case 2:
                    pos = center + new Vector3(0 , 0, 8);
                    break; 
            }
            //Vector3 pos = center + new Vector3(8+i, -3, 1+i);
            GameObject newObject = Instantiate(Targetobj, pos, Quaternion.identity);
            // newObject sets parent to parent of Targetobj
            newObject.transform.parent = Targetobj.transform.parent;
            // assignes tag = "Target" for each generated target
            newObject.tag = "Target";
            // change color of Targets
            newObject.GetComponent<Renderer>().material.color = colors[i];
            newObject.transform.name = colorNames[i];
        }
    }



    // Defines color, cube object form of the empty game object
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
