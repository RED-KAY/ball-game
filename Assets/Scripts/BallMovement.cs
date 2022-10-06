using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Rigidbody body;

    // Used for movement control. Player input is translated in to movement relative to this Transform. In this project it is set to the Camera, meaning ball's forward in Camera's forward
    [SerializeField]
    Transform playerInputSpace = default; 

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [Range(0f, 100f)]
    float maxAcceleration = 10f;

    Vector3 velocity; // current velocity of the ball
    Vector3 desiredVelocity; // target velocity

    bool desiredJump;
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;
    [HideInInspector] public bool onGround;
    [Range(0, 5)]
    int maxAirJumps = 0;
    [HideInInspector] public int jumpPhase;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector3.ClampMagnitude(playerInput, 1f); // We do this in order to prevent extra boost in speed in diagnol directions when playing on Keyboard.

        // We eliminate the Y component & normalize the vectors to get the true desired directions and then multiply to get desired velocity.
        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            desiredVelocity =
                (forward * playerInput.y + right * playerInput.x) * maxSpeed;
        }
        else
        {
            desiredVelocity =
                new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }

        desiredJump |= Input.GetButtonDown("Jump"); // this operator prevents unnecessary calls to the Input function.
    }

    private void FixedUpdate()
    {
        UpdateState();
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z =
            Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        onGround = false;  // setting this to false every end of FixedUpdate call and handling set values in Collision functions is a safe & secure way of resetting this boolean.
    }

    void UpdateState()
    {
        velocity = body.velocity;
        if (onGround)
        {
            jumpPhase = 0;
        }
    }

    void Jump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {   
             jumpPhase += 1;

            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f); // Prevents getting a boost in Y-axis from velocity applied previously.
            }
            velocity.y += jumpSpeed;
        }
    }

    // Below code needs to be done to prevent the ball from detecting surfaces like walls as ground.
    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f; // This does ignore few angled slopes but works for our purpose.
        }
    }

}
