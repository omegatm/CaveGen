using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public float height;
    public LayerMask Ground;
    bool grounded;
    public float groundDrag;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, height * .5f * .2f, Ground);
        myInput();
        SpeedControl();
        if (grounded)
        {
            rb.drag= groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void MovePlayer()
    {
        moveDirection=orientation.forward*verticalInput+orientation.right*horizontalInput;
        rb.AddForce(moveDirection.normalized*moveSpeed*10f,ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel= new Vector3(rb.velocity.x,0f,rb.velocity.z);
        if(flatVel.magnitude>moveSpeed)
        {
            Vector3 limitedVel=flatVel.normalized*moveSpeed;
            rb.velocity=new Vector3(limitedVel.x,rb.velocity.y,limitedVel.z);
        }
    }
}
