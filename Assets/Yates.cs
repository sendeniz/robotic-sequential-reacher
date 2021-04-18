using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yates : MonoBehaviour
{
    public static Matrix4x4 Shuffle(Matrix4x4 matrix)
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
