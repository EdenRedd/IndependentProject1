using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationForCamera : MonoBehaviour
{
    [SerializeField] Vector2 _look;
    [SerializeField] float rotationPower = 5;
   
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
    }
}
