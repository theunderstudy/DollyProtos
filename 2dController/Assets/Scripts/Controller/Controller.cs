using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float JumpForce;
    public float Acceleration, MaxSpeed = 2;
    const float rayOffset = 0.02f;
    public float GroundedDistance = 0.01f;
    public float MovementStartImpulse=1;
    Rigidbody2D rb;
    Vector2 move;
    float desiredVelocity;
    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public LayerMask GroundLayer;
    bool jump = false;

    public enum PlayerStates { ground, air, wall, dashing }
    public PlayerStates PlayerState = PlayerStates.air;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        

    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (PlayerState == PlayerStates.ground)
            {
                jump = true;
            }
        }
    }

    private void CheckWall()
    {

    }
    private void CheckGround()
    {
        UpdateRaycastOrigins();
        //check for ground collisions
        for (int i = 0; i < 3; i++)
        {
            Vector2 _rayOrigin = Vector2.Lerp(raycastOrigins.BotLeft, raycastOrigins.BotRight, (float)i / 2);
            RaycastHit2D hit = Physics2D.Raycast(_rayOrigin, Vector2.up * -1, GroundedDistance,GroundLayer);
            Debug.DrawRay(
              _rayOrigin
               , Vector2.up * -1 * GroundedDistance
               , Color.red);
            if (hit)
            {
                //we on the ground
                PlayerState = PlayerStates.ground;
                break;
            }
            else
            {
                PlayerState = PlayerStates.air;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
       
        if (jump)
        {
            jump = false;
            Debug.Log("jump");
            Vector3 _jumpVelocity = Vector3.zero;
            _jumpVelocity.y = JumpForce;
            rb.AddForce(_jumpVelocity, ForceMode2D.Impulse);
        }

        DoHorizontalMovement();
        
    }
    private void DoHorizontalMovement()
    {
        move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        desiredVelocity = move.x * MaxSpeed;
        if (move.x !=0)
        {
            //check if the velocity is 0
            if (Mathf.Abs(rb.velocity.x) < 0.001f)
            {
                Debug.Log("push");
                rb.AddForce(new Vector2(MovementStartImpulse * move.x, 0) , ForceMode2D.Impulse);
                Debug.Log(MovementStartImpulse * move.x);
            }
            if (Mathf.Abs(rb.velocity.x) <= Mathf.Abs(desiredVelocity))
            {
                Vector2 _force = new Vector2(move.x * Acceleration, 0) * Time.deltaTime;
                Debug.Log(rb.velocity.x);
                rb.AddForce(_force);
            }
            else
            {
                Debug.Log("chillllll");
            }
        }
        if (Mathf.Abs(move.x) < 1)
        
        {
            //apply dampening force
            Vector2 _damp = rb.velocity * -15;
            _damp.y = 0;
            rb.AddForce(_damp, ForceMode2D.Force);
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        //bounds.Expand(rayOffset * -2);
        raycastOrigins.BotLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.BotRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    private struct RaycastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BotLeft, BotRight;
    }

}
