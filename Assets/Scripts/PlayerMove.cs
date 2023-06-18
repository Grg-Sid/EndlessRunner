using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    private const float LANE_LENGTH  = 2f;
    private const float TURN  = 0.35f;

    private Animator animator;
    private CharacterController characterController;
    public float jumpForce = 8.0f;
    public float gravity = 12.0f;
    private float verticalVelocity;
    public float speed = 7.0f;
    private int desiredLane = 1;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Lane prediction
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("left");
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
            //animator.SetBool("isRight", true);
            targetPosition += Vector3.right * LANE_LENGTH;
        }

        //Move Delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        //Y axis

        bool fastDown = false;

        if (IsGrounded())
        {
            verticalVelocity = -0.1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
        }

        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = -jumpForce;
                fastDown = true;
            }
        }

        if (fastDown)
        {
            animator.SetTrigger("roll");
        }

        
        moveVector.y = verticalVelocity;
        moveVector.z = speed;



        //Move Char
        characterController.Move(moveVector * Time.deltaTime);

        Vector3 direction = characterController.velocity;
        direction.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, direction, TURN);
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(characterController.bounds.center.x, (characterController.bounds.center.y - characterController.bounds.extents.y) + 0.2f, characterController.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction,Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

}
