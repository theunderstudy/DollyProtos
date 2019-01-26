using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Controller2D))]
public class Player : TimeObject
{
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    public ParticleSystem DashTrail;
    public bool Dashing = false, CanDash = false;
    public float DashForce = 10;
    public float DashTime = 0.5f;
    private float CurrentDashTime = 0;
    public BackgroundSquare InstantiateSquare;
    public int FramesBetweenSquares=30, CurrentFrame=0;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        var emission = DashTrail.emission;
        emission.enabled = false;
    }

    protected override void ReverseFrame()
    {
        transform.position = TimeDataList[TimeDataList.Count - 1].Position;
        transform.localScale = TimeDataList[TimeDataList.Count - 1].Scale;
        velocity = TimeDataList[TimeDataList.Count - 1].Velocity;
        TimeDataList.RemoveAt(TimeDataList.Count - 1);
        if (TimeDataList.Count == 0)
        {
            TimeController.CurrentTimeState = TimeController.TimeState.Normal;
        }
        CurrentFrame += 1;
        if (CurrentFrame >= FramesBetweenSquares)
        {
            CurrentFrame = 0;
            Instantiate(InstantiateSquare).transform.position = transform.position;
        }
    }
    protected override void StoreTimeData()
    {
        //make a new time data
        TimeData temp;
        temp.Position = transform.position;
        temp.Scale = transform.localScale;
        temp.Velocity = velocity;
        TimeDataList.Add(temp);
        if (TimeDataList.Count > FramesStored)
        {
            TimeDataList.RemoveAt(0);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void UpdateLoop()
    {
        CalculateVelocity();
        HandleWallSliding();
        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    public void OnDashInputDown()
    {
        if (CanDash)
        {
            if (!Dashing)
            {
                Dashing = true;
                CanDash = false;
                CurrentDashTime = 0;
                velocity.y = 0;
                //velocity x = dash force;
                Vector3 temp = Vector3.one;
                temp.y = 0.6f;
                transform.localScale = temp;
                var emission = DashTrail.emission;
                emission.enabled = true;

            }
        }
    }

    public void PrepForTimeTravel()
    {
        DisableDash();
        Vector3 temp = Vector3.one;
        temp.y = 1;
        transform.localScale = temp;
    }

    public void EndTimeTravel()
    {

    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;

            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
                Instantiate(InstantiateSquare).transform.position = transform.position;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
                Instantiate(InstantiateSquare).transform.position = transform.position;
            }
            return;
        }
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    Instantiate(InstantiateSquare).transform.position = transform.position;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
                Instantiate(InstantiateSquare).transform.position = transform.position;
            }
        }
        else
        {
            //check if we can double jump 
            if (controller.DoubleJumpAllowed)
            {
                controller.DoubleJumpAllowed = false;
                velocity.y = maxJumpVelocity;
                Instantiate(InstantiateSquare).transform.position = transform.position;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }



    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public void DisableDash()
    {
        Dashing = false;
        var emission = DashTrail.emission;
        emission.enabled = false;
        velocity.x = directionalInput.x * moveSpeed;
        Vector3 temp = Vector3.one;
        temp.y = 1;
        transform.localScale = temp;
    }
    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        if (Dashing)
        {
            velocity.x = directionalInput.x * DashForce;
            velocity.y = 0;
            CurrentDashTime += Time.deltaTime;
            if (CurrentDashTime > DashTime)
            {
                DisableDash();
            }
        }
    }
}
