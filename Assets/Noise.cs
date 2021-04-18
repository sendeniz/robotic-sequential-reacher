using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

// Box Mueller Transformation to simulate values from a gaussian normal distribution 
public class Noise : MonoBehaviour
{
    public static float gauss(float mu, double sigma)
    {
        System.Random rand = new System.Random();
        double u1 = 1.0 - rand.NextDouble(); //uniform distribution [0,1]
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double mean = mu; // mean mu
        double stdDev = sigma; // standard deviation sigma 
        double randNormal = mean + stdDev * randStdNormal; // generate values from the normal
        float gen_val = (float)randNormal; // convert to float for position change per frame in vector below
        return gen_val;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
