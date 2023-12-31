using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDetector : MonoBehaviour
{
    //Movement
    private const float LANE_LENGTH = 2f;
    private const float TURN = 0.35f;

    private Animator animator;
    private CharacterController characterController;
    public float jumpForce = 8.0f;
    public float gravity = 12.0f;
    private float verticalVelocity;
    public float speed = 7.0f;
    private int desiredLane = 1;
    private bool isRunning = false;

    /// <summary>
    /// ////////
    /// </summary>
    private PlayerMove playerMove;

    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    public static bool isTap = false;
    private bool swipeDown = false;
    private bool swipeUp = false;



    public bool detectSwipeAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        swipeDown = false;
        swipeUp = false;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
                isTap = true;
            }
            //Detects Swipe while finger is still moving on screen
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeAfterRelease)
                {
                    fingerDownPos = touch.position;
                    DetectSwipe();
                }
            }

            //Detects swipe after finger is released from screen
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }
        }

        //Check 
        if (!isRunning)
            return;


        //Changes
        //Our future location
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_LENGTH;
        }
        else if (desiredLane == 2)
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
            if (swipeUp)
            {
                verticalVelocity = jumpForce;
            }
            else if (swipeDown)
            {
                StartRolling();
                Invoke("StopRolling", 0.75f);
            }
        }

        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            if (swipeDown)
            {
                verticalVelocity = -jumpForce;
                fastDown = true;
            }
        }

        if (fastDown)
        {
            //animator.SetTrigger("roll");
            StartRolling();
            Invoke("StopRolling", 0.75f);

        }


        moveVector.y = verticalVelocity;
        moveVector.z = speed;



        //Move Char
        characterController.Move(moveVector * Time.deltaTime);

        Vector3 direction = characterController.velocity;
        direction.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, direction, TURN);

    }

    void DetectSwipe()
    {

        if (VerticalMoveValue() > SWIPE_THRESHOLD && VerticalMoveValue() > HorizontalMoveValue())
        {
            //Debug.Log("Vertical Swipe Detected!");
            if (fingerDownPos.y - fingerUpPos.y > 0)
            {
                swipeUp = true;
            }
            else if (fingerDownPos.y - fingerUpPos.y < 0)
            {
                swipeDown = true;
            }
            fingerUpPos = fingerDownPos;

        }
        else if (HorizontalMoveValue() > SWIPE_THRESHOLD && HorizontalMoveValue() > VerticalMoveValue())
        {
            //Debug.Log("Horizontal Swipe Detected!");
            if (fingerDownPos.x - fingerUpPos.x > 0)
            {
                MoveLane(true);
            }
            else if (fingerDownPos.x - fingerUpPos.x < 0)
            {
                MoveLane(false);
            }
            fingerUpPos = fingerDownPos;

        }
        else
        {
            //Debug.Log("No Swipe Detected!");
        }
    }

    float VerticalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    float HorizontalMoveValue()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }
    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(characterController.bounds.center.x, (characterController.bounds.center.y - characterController.bounds.extents.y) + 0.2f, characterController.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
    }

    private void StartRolling()
    {
        animator.SetBool("isRoll", true);
        characterController.height /= 2;
        characterController.center = new Vector3(characterController.center.x, 0.5f, characterController.center.z);
    }

    private void StopRolling()
    {
        animator.SetBool("isRoll", false);
        characterController.height *= 2;
        characterController.center = new Vector3(characterController.center.x, 0.8f, characterController.center.z);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Obstacles")
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        isRunning = false;
    }
}