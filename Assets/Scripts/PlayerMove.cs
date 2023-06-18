using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    private const float LANE_LENGTH  = 2f;

    private CharacterController characterController;
    public float jumpForce = 4.0f;
    public float gravity = 12.0f;
    private float verticalVelocity;
    public float speed = 7.0f;
    private int desiredLane = 1;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        //Debug.Log("Working");

        //Lane prediction
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("hello");
            MoveLane(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(true);
        }

        //Our future location
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if(desiredLane == 0) 
        {
            targetPosition += Vector3.left * LANE_LENGTH;
        }
        else if(desiredLane == 2) 
        {
            targetPosition += Vector3.right * LANE_LENGTH;
        }

        //Move Delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        moveVector.y = -0.1f;
        moveVector.z = speed;

        //Move Char
        characterController.Move(moveVector * Time.deltaTime);
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }
}
