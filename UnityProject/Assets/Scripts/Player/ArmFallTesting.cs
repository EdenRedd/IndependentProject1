using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script will detach arm on determined condition, on this temporary script it will be on space bar press
/// </summary>
public class ArmFallTesting : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<Transform>().parent = null;
            this.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
