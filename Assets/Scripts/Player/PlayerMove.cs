using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float CurrentSpeed {  get; private set; }
    public float walkSpeed;
    public float runSpeed;

    public Transform skin;
    public Transform reference;
    private Rigidbody rb;
    private InputManager input;
    private Camera cam;

    private Vector2 moveInput;
    private bool isSprint;
    
    private Vector3 orientation;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = InputManager.I;
        cam = Camera.main;

        ReadInput();
    }
    private void Update()
    {
        orientation = reference.right * moveInput.x + reference.forward * moveInput.y;
        SpeedControl();
        SkinRotation();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void SkinRotation()
    {
        if (moveInput != Vector2.zero)
        {
            skin.rotation = Quaternion.Lerp(skin.rotation, Quaternion.LookRotation(orientation), 10 * Time.deltaTime);
        }
    }

    private void ReadInput()
    {
        if (!input) return;

        var control = input.action.Player;
        control.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        control.Move.canceled += ctx => moveInput = Vector2.zero;

        control.Sprint.performed += ctx => isSprint = true;
        control.Sprint.canceled += ctx => isSprint = false;
    }
    private void Move()
    {
        rb.AddForce(orientation * CurrentSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, isSprint ? runSpeed : walkSpeed, 10 * Time.deltaTime);

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (flatVel.magnitude > CurrentSpeed)
        {
            Vector3 limited = flatVel.normalized * CurrentSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

}
