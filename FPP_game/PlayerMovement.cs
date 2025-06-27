using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed=12f;
    public float gravity=-9.81f*2;
    public float jumpHeight=3f;
    public Transform groundCheck;
    public float groundDistance=0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGround;
    bool isMoving;

    private Vector3 lastPosition=new Vector3(0f,0f,0f);

    // Start is called before the first frame update
    void Start()
    {
        controller=GetComponent<CharacterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        isGround=Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);

        if(isGround && velocity.y<0)
        {
            velocity.y=-2f;
        }

        float x=Input.GetAxis("Horizontal");
        float z=Input.GetAxis("Vertical");

        Vector3 move=transform.right * x +transform.forward*z;

        controller.Move(move*speed*Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            velocity.y=Mathf.Sqrt(jumpHeight*-2f*gravity);
        }
        velocity.y +=gravity*Time.deltaTime;

        controller.Move(velocity*Time.deltaTime);

        if(lastPosition !=gameObject.transform.position && isGround == true)
        {
            isMoving=true;

        }
        else{
            isMoving=false;
        }
        lastPosition=gameObject.transform.position;
    }
}
