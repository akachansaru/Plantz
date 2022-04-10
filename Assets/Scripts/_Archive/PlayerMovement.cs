﻿//using UnityEngine;

//// From How to Walk Around a Sphere (Walk on Planet) in Unity 3D by Learn Everything Fast on Youtube
//[RequireComponent(typeof(GravityBody))]
//public class PlayerMovement : MonoBehaviour
//{
//    // public vars
//    public float mouseSensitivityX = 1;
//    public float mouseSensitivityY = 1;
//    public float walkSpeed = 6;
//    public float jumpForce = 220;
//    public LayerMask groundedMask;

//    // System vars
//    bool grounded;
//    Vector3 moveAmount;
//    Vector3 smoothMoveVelocity;
//    float verticalLookRotation;
//    Transform cameraTransform;
//    Rigidbody playerRigidbody;


//    void Awake()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//        cameraTransform = Camera.main.transform;
//        playerRigidbody = GetComponent<Rigidbody>();
//    }

//    void Update()
//    {

//        // Look rotation:
//        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
//        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
//        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
//        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

//        // Calculate movement:
//        float inputX = Input.GetAxisRaw("Horizontal");
//        float inputY = Input.GetAxisRaw("Vertical");

//        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
//        Vector3 targetMoveAmount = moveDir * walkSpeed;
//        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

//        // Jump
//        if (Input.GetButtonDown("Jump"))
//        {
//            if (grounded)
//            {
//                playerRigidbody.AddForce(transform.up * jumpForce);
//            }
//        }

//        // Grounded check
//        Ray ray = new Ray(transform.position, -transform.up);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
//        {
//            grounded = true;
//        }
//        else
//        {
//            grounded = false;
//        }
//    }

//    void FixedUpdate()
//    {
//        // Apply movement to rigidbody
//        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
//        playerRigidbody.MovePosition(playerRigidbody.position + localMove);
//    }
//}
