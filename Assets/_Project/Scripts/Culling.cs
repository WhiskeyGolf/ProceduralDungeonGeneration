using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        col.GetComponent<MeshRenderer>().enabled = true;
    }

    void OnTriggerExit(Collider col)
    {
        col.GetComponent<MeshRenderer>().enabled = false;
    }
}
