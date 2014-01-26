using UnityEngine;
using System.Collections;
using System;

public class CubeBehavior : MonoBehaviour
{
    public float maxSpeed = 20f;
    float currentSpeed = 5f;
    const float acceleration = 10;
    public int playerNumber;
    public int playerScore = 0;
    public bool scoreChanged;
    public CubeBehavior opponent;
    private DateTime boosted;
    private const int boostDuration = 1700;
    int turnSpeed;

    void Boost()
    {
        Debug.Log("BOOST!");
        boosted = DateTime.Now;
    }

    // Use this for initialization
    void Start()
    {


    }

    public float jumpSpeed = 8.0f;
    public float gravity = 40.0f;

    private Vector3 moveDirection = Vector3.zero;

    void Update()
    {
        if (stopped)
        {
            animation.Stop();
            return;
        }

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
            moveDirection *= currentSpeed;

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
        else if (crouchReceived)
        {
            //Debug.Log("crouching " + DateTime.Now);
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

        float maxSpeedIncrease = (DateTime.Now - boosted).TotalMilliseconds < boostDuration ? maxSpeed : 0;
        float playerDistanceTime = (opponent.transform.position.z - this.transform.position.z) / maxSpeed;
        //Debug.Log(playerNumber + " " + opponent.transform.position.z + " " + this.transform.position.z + " " + playerDistanceTime);
        if(playerDistanceTime > 0)
        {
            maxSpeedIncrease += Math.Min(12, playerDistanceTime * 4);
        }
        //Debug.Log(maxSpeedIncrease);
        currentSpeed = Math.Min(maxSpeed + maxSpeedIncrease, currentSpeed + Time.deltaTime * acceleration);
        
        if(baaaing)
        { 
            var sphere = this.transform.FindChild("Sphere");
            
            Vector3 scale = Vector3.zero;
            Vector3 maxScale = new Vector3(0.6f ,0.6f ,0.6f);
            float p = (float)(DateTime.Now - baaaTime).TotalMilliseconds / 350f;
            if (p > 1)
            {
                baaaing = false;
                sphere.transform.localScale = scale;
            }
            else
            {
                sphere.transform.localScale = scale + (maxScale * p);
                scale = sphere.transform.localScale;
            }

            SphereCollider collider = sphere.collider as SphereCollider;
            float playerDistance = (opponent.transform.position - this.transform.position).magnitude;
            if (collider.radius * scale.x * this.transform.localScale.x >= playerDistance && this.DecreaseScore())
            {
                opponent.Boost();
                baaaing = false;
                sphere.transform.localScale = Vector3.zero;
            }
        }

        if ((baaaReceived || Input.GetKey(KeyCode.B)) && !baaaing && (DateTime.Now - baaaTime).TotalMilliseconds > 5000)
        {
            baaaReceived = false;
            baaaing = true;
            baaaTime = DateTime.Now;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
    //    if(hit.gameObject.tag != "Terrain")
        //Debug.Log("Hit a " + hit.gameObject.tag);
        if (hit.gameObject.tag == "Tree" || hit.gameObject.tag == "Obstacle")
        {
            currentSpeed = 5;
        }

        if (hit.gameObject.tag == "FinishLine")
        {
            this.stopped = true;
        }

    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject == null)
        {
            return;
        }

        if (hit.gameObject.tag == "FinishLine")
        {
            this.stopped = true;
        }

        if (hit.gameObject.tag == "Bush")
        {
            currentSpeed = Math.Min(7, currentSpeed - 7);
        }

        if ((hit.gameObject.tag == "Apple" && this.playerNumber == 1) ||
            (hit.gameObject.tag == "Pear" && this.playerNumber == 2))
        {
            IncreaseScore();
            if ((DateTime.Now - boosted).TotalMilliseconds < boostDuration)
            {
                opponent.IncreaseScore();
            }

            foreach (Collider collider in Physics.OverlapSphere(hit.gameObject.transform.position, 0.4f))
            {
                if (collider.gameObject.tag == "Apple" || collider.gameObject.tag == "Pear")
                {
                    Destroy(collider.gameObject);
                }
            }
        }

    }

    public void IncreaseScore()
    {
        this.playerScore++;
        this.scoreChanged = true;
    }

    private bool DecreaseScore()
    {
        if (this.playerScore > 0)
        {
            this.playerScore--;
            this.scoreChanged = true;
            return true;
        }

        return false;
    }

    float GetX()
    {
        float factor = turnSpeed / 4f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            return Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        }

        if (rightReceived)
        {
            return factor;
        }

        if (leftReceived)
        {
            return -factor;
        }

        return 0;
    }

    DateTime crouchedTime;
    bool hasCrouched = false;
    bool baaaing = false;
    DateTime baaaTime;

    bool jumpReceived;
    bool crouchReceived;
    bool leftReceived;
    bool rightReceived;
    bool baaaReceived;
    public void SendCommand(string command, params object[] args)
    {
        //Debug.Log("Player " + playerNumber + " received " + command);
        switch (command)
        {
            case "left":
                leftReceived = true;
                rightReceived = false;
                if (args != null && args.Length > 0)
                {
                    turnSpeed = (int)args[0];
                    Debug.Log(turnSpeed);
                }
                break;
            case "right":
                rightReceived = true;
                leftReceived = false;
                if (args != null && args.Length > 0)
                {
                    turnSpeed = (int)args[0];
                    Debug.Log(turnSpeed);
                }
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
            case "baaaaa":
                //Debug.Log("baa received");
                if (!baaaing && (DateTime.Now - baaaTime).TotalMilliseconds > 5000)
                {
                    baaaReceived = true;
                }
                break;
        }
    }



    private bool stopped;
}
