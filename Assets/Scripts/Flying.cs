using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : MonoBehaviour
{
    public static Flying instance;
    public float speedF;
    public float speedH;
    public float speedV;

    private float yaw = 41;
    private float pitch = -63;

    private Rigidbody cameraRB;

    [SerializeField]
    Transform defaultTransform;

    bool activateMouse = true;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        defaultTransform.position = new Vector3(-21f,35f,-20f);

        cameraRB = GetComponent<Rigidbody>();
        
        transform.LookAt(Boundries.instance.terrain_.GetComponent<BoxCollider>().center);
    }
    public void ResetCamPosition()
    {
        defaultTransform.position = new Vector3(-21f,35f,-20f);
        transform.eulerAngles = new Vector3(-(63), 41, 0);
    }
    public void LateActivation()
    {
         activateMouse = false;
         ResetCamPosition();
         pitch = -63;
         yaw = 41;
         activateMouse = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(activateMouse == true)
        {
            yaw += speedH * Input.GetAxis("Mouse X") * Time.deltaTime;
            pitch += speedV * Input.GetAxis("Mouse Y") * Time.deltaTime;

            // Prevent camera from being inverted
            if (pitch > 120f)
            {
                pitch = 120f;
            }
            else if (pitch < -120f)
            {
                pitch = -120f;
            }

            // Camera view angle
            transform.eulerAngles = new Vector3(-pitch, yaw, 0);
            
            // Camera movement
            if (Input.GetKey(KeyCode.W))
            {
                cameraRB.AddRelativeForce(Vector3.forward * speedF * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                cameraRB.AddRelativeForce(Vector3.back * speedF * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                cameraRB.AddRelativeForce(Vector3.left * speedF * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                cameraRB.AddRelativeForce(Vector3.right * speedF * Time.deltaTime);
            }
        }
       
    }
}
