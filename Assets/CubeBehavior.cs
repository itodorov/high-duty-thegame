using UnityEngine;
using System.Collections;
using System;

public class CubeBehavior : MonoBehaviour
{
    public float speed = 10f;
    public int playerNumber;
    // Use this for initialization
    void Start()
    {
        
        
    }
     
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	private Vector3 moveDirection = Vector3.zero;

	void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && !hasCrouched)
        {
            crouchReceived = true;
        }
        CharacterController controller = (CharacterController)GetComponent("CharacterController");
        float x = GetX();
       // Debug.Log(x);
        if (controller.isGrounded)
        {
            
            // We are grounded, so recalculate
            // move direction directly from axes
            moveDirection = new Vector3(x, 0, 1);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetKey(KeyCode.Space) || jumpReceived)
            {
                moveDirection.y = jumpSpeed;
                jumpReceived = false;
            }
        }

        if (hasCrouched && (DateTime.Now - crouchedTime).TotalMilliseconds > 1000.0 * 24 / 30)
        {
            hasCrouched = false;
            controller.height = 0.09f;
            controller.center = new Vector3(0, 0.05f, 0);
        }
        else if(crouchReceived)
        {
            Debug.Log("crouching " + DateTime.Now);
            crouchReceived = false;
            crouchedTime = DateTime.Now;
            hasCrouched = true;
            animation.Play("PetkoDodge");
            controller.height = 0.06f;
            controller.center = new Vector3(0, 0.035f, 0);
        }

        if (!hasCrouched)
        {
            animation.Play("PetkoRiding");
        }
        else
        {
            animation.Play("PetkoDodge");
        }
      //  Debug.Log("grounded " + controller.isGrounded+ " " + moveDirection);

		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		// Move the controller
		controller.Move(moveDirection * Time.deltaTime);
	}

    float GetX()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            return Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        }

        if (rightReceived)
        {
            return 1;
        }

        if (leftReceived)
        {
            return -1;
        }

        return 0;
    }

    DateTime crouchedTime;
    bool hasCrouched= false;

    bool jumpReceived;
    bool crouchReceived;
    bool leftReceived;
    bool rightReceived;

    public void SendCommand(string command)
    {
        Debug.Log("Player " + playerNumber + " received " + command);   
        switch (command)
        {
            case "left":
                leftReceived = true;
                rightReceived = false;
                break;
            case "right":
                rightReceived = true;
                leftReceived = false;
                break;
            case "jump":
                jumpReceived = true;
                break;
            case "crouch":
                if (!hasCrouched)
                {
                    crouchReceived = true;
                }
                break;
            case "straight":
                leftReceived = rightReceived = false;
                break;
        }
    }
}
