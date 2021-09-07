using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    void Update()
    {
        //Reading player input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        //Moving the player or object
        if(movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= _speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }

    }
}
