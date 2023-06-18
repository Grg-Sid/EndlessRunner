using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    //Character Movement
    private CharacterController characterController;
    public float jumpForce = 4.0f;
    public float gravity = 12.0f;
    private float verticalVelocity;
    public float speed = 7.0f;
    private int desiredLane = 1;



    private Vector2 pressedPos;
    private Vector2 releasePos;

    public bool swipeDetectAfterRelease  = false;

    public float swipeThreshold = 20f;

    public float swipe = 1f;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    private void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                pressedPos = touch.position;
                releasePos = touch.position;
            }

            if(touch.phase == TouchPhase.Moved) 
            { 
                if(!swipeDetectAfterRelease)
                {
                    releasePos = touch.position;
                    DetectSwipe();
                }
            }

            if(touch.phase == TouchPhase.Ended)
            {
                releasePos = touch.position;
                DetectSwipe();
            } 

        }
    }

    void DetectSwipe()
    {
        if(VerticalValue() > swipeThreshold && VerticalValue() > HorizontalValue())
        {
            Debug.Log("vertical swipe detected");
            if (releasePos.y - pressedPos.y > 0)
            {
                OnSwipeUp();
            }
            else if (pressedPos.y - releasePos.y > 0)
            {
                OnSwipeDown();
            }
        }

        else if(HorizontalValue() > swipeThreshold && HorizontalValue() > VerticalValue())
        {
            Debug.Log("horizontal swipe detected");
            if(releasePos.x - pressedPos.x > 0)
            {
                OnSwipeRight();
            }
            else if(releasePos.x - pressedPos.x < 0)
            {
                OnSwipeLeft();
            }
        }
        else
        {
            Debug.Log("No Swipe detected");
        }
    }

    float VerticalValue()
    {
        return Mathf.Abs(releasePos.y - pressedPos.y);
    }
    float HorizontalValue()
    {
        return Mathf.Abs(releasePos.x - pressedPos.x);
    }

    void OnSwipeUp()
    {
        Debug.Log("Up");
    }

    void OnSwipeDown()
    {
        Debug.Log("Down");
    }

    void OnSwipeLeft()
    {
        //Left
        desiredLane--;
        if(desiredLane == -1)
        {
            desiredLane = 0;
        }

        if(desiredLane == 0)
        {

        }
        //Debug.Log("Left");
        //transform.Translate(Vector3.left * swipe * Time.deltaTime, Space.World);
    }

    void OnSwipeRight()
    {
        //right
        Debug.Log("Right");
        if(desiredLane == 3) 
        {
            desiredLane = 2;
        }


        //transform.Translate(Vector3.right * swipe * Time.deltaTime, Space.World);
    }

}
