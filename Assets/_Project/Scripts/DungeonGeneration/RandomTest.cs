using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour {

    public int seed;

	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            System.Random pseudoRNG = new System.Random(seed);
            for (int i = 0; i < 50; i++)
            {
                Debug.Log(pseudoRNG.Next(4,15));
            }
        }
	}
}
