using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    public bool lockCursor;
    public Transform target;
    public float distanceFromTarget = 3.5f;

    float speedH = 2.0f;
    float speedV = 2.0f;

    Vector2 pitchMinMax = new Vector2(-90,90);
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Use this for initialization
    void Start () {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        transform.position = target.position - transform.forward * distanceFromTarget;
    }
}
