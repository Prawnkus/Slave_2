using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    CharacterController CC;
    Camera cam;
    float forwardVelocity = 15;
    float backwardsVelocity = 4;
    float jumpHeight = 8;
    bool hasJumped;

    float verticalVelocity;
    float gravity = -30;
    [Range(0,1)]
    public float airControlPercent;

    float translation;
    float strafe;

    private RaycastHit? grounding;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        CC = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        //Horizontal movement
        if (Input.GetAxis("Vertical") >= 0)
        {
            translation = Input.GetAxis("Vertical") * forwardVelocity;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            translation = Input.GetAxis("Vertical") * backwardsVelocity;
        }
        strafe = Input.GetAxis("Horizontal") * forwardVelocity;

        //Vertical movement
        groundCheck();
        if (Input.GetButtonDown("Jump") && grounding.HasValue)
        {
            Jump();
        }
        else if (grounding.HasValue && verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
        else if (!grounding.HasValue)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        //rotate with cam on Y axis
        Vector3 newrot = new Vector3(0, cam.transform.rotation.eulerAngles.y, 0);
        transform.eulerAngles = newrot;

        Vector3 moveVector = transform.rotation * new Vector3(strafe,verticalVelocity,translation) * Time.deltaTime;
        moveVector = Vector3.ClampMagnitude(moveVector, forwardVelocity);
        CC.Move(moveVector);


    }

    void groundCheck()
    {
        RaycastHit hit;
        Vector3 bottom = transform.position - new Vector3(0, CC.height/2,0);
        if (Physics.Raycast(bottom, Vector3.down, out hit, 0.5f))
        {
            grounding = hit;
            if (!Input.GetButtonDown("Jump") && verticalVelocity <= 0)
            {
                CC.Move(new Vector3(0, -hit.distance, 0));
            }
        }
        else {
            grounding = null;
        }
    }

    void Jump()
    {
        float jumpVelocity = Mathf.Sqrt(-2*gravity * jumpHeight);
        verticalVelocity = jumpVelocity;
    }
}
