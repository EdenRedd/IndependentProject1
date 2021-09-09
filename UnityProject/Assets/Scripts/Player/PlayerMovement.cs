using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform camera;
    public CharacterController controller;
    
    public float playerMovementSpeed = 6f;

    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;

    private void Update()
    {
        //Reading player input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //checks for movement on any direction
        if(direction.magnitude >= 0.1f)
        {
            //Makes the character turn in the direction the camera is facing
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //Moves the character based on what position the camera is facing
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * playerMovementSpeed * Time.deltaTime);
        }
    }
}
