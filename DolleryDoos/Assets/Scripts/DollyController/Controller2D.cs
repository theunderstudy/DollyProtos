using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask CollisionMask;
    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    const float rayOffset = 0.02f;
    public float GroundOffset = 1;
    public float SafeDistance=0.2f;
    public int HorizontalRays, VerticalRays;
    public float HorizontalSpeed = 5;
    private bool MovingLeft = false;
    private bool OnGround = false;
    //unsafe Vector3* leftVertex, middleVertex, rightVertex;

    public int LeftVert, RightVert;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void UpdatePosition(Vector3 velocity)
    {
        //update raycast origins
        UpdateRaycastOrigins();
        //  Debug.Log(velocity);
        CheckVerticalCollisions(ref velocity);
        UpdateHorizontalMovement(ref velocity);
        // Debug.Log(velocity);
        transform.Translate(velocity);
    }
    public void UpdateHorizontalMovement(ref Vector3 velocity)
    {
        if (!OnGround)
        {
            return;
        }
        if (MovingLeft)
        {
            velocity.x = -HorizontalSpeed;
            Vector2 _rayOrigin = raycastOrigins.BotLeft;
            RaycastHit2D hit = Physics2D.Raycast(_rayOrigin, Vector2.up * -1, 1, CollisionMask);
            if (!hit)
            {
                //check for land in the center of dolly
                _rayOrigin = Vector2.Lerp(raycastOrigins.BotLeft, raycastOrigins.BotRight, 0.5f);
                hit = Physics2D.Raycast(_rayOrigin, Vector2.up * -1, 1, CollisionMask);
                if (!hit)
                    MovingLeft = false;
            }
        }
        else
        {
            velocity.x = HorizontalSpeed;
            Vector2 _rayOrigin = raycastOrigins.BotRight;
            RaycastHit2D hit = Physics2D.Raycast(_rayOrigin, Vector2.up * -1, 1, CollisionMask);
            if (!hit)
            {
                _rayOrigin = Vector2.Lerp(raycastOrigins.BotLeft, raycastOrigins.BotRight, 0.5f);
                hit = Physics2D.Raycast(_rayOrigin, Vector2.up * -1, 1, CollisionMask);
                if (!hit)
                    MovingLeft = true;
            }
        }
        velocity.x *= Time.deltaTime;
        float _xDirection = Mathf.Sign(velocity.x);
        float _rayLength = Mathf.Abs(velocity.x) + rayOffset;
        for (int i = 0; i < VerticalRays; i++)
        {
            Vector2 _rayOrigin = Vector2.Lerp(
                (_xDirection == -1) ? raycastOrigins.BotLeft : raycastOrigins.BotRight,
                 (_xDirection == -1) ? raycastOrigins.TopLeft : raycastOrigins.TopRight,
                  (float)i / (HorizontalRays - 1)
                );
            RaycastHit2D hit = Physics2D.Raycast(_rayOrigin, Vector2.right * _xDirection, _rayLength, CollisionMask);
            if (hit)
            {
                velocity.y = (hit.distance - rayOffset) * _xDirection;
                _rayLength = hit.distance;
                OnGround = true;
            }
            Debug.DrawRay(
               _rayOrigin
                , Vector3.right * _xDirection
                , Color.red);
        }
    }
    private void CheckVerticalCollisions(ref Vector3 velocity)
    {
        float _yDirection = Mathf.Sign(velocity.y);
        float _rayLength = Mathf.Abs(velocity.y) + rayOffset;
        // Debug.Log(_rayLength);
        //fire bottom rays
        OnGround = false;
        for (int i = 0; i < HorizontalRays; i++)
        {
            Vector2 _rayOrigin = Vector2.Lerp(
                (_yDirection == -1) ? raycastOrigins.BotLeft : raycastOrigins.TopLeft,
                 (_yDirection == -1) ? raycastOrigins.BotRight : raycastOrigins.TopRight,
                  (float)i / (HorizontalRays - 1)
                );
            RaycastHit2D hit = Physics2D.Raycast(_rayOrigin, Vector2.up * _yDirection, _rayLength, CollisionMask);
            if (hit)
            {
                //check how far from the ground we are
                if (hit.distance <= SafeDistance)
                {
                    velocity.y = ((hit.distance - rayOffset) * _yDirection) + GroundOffset;
                }              
                _rayLength = hit.distance;
                OnGround = true; 
                //get the nearest vertex
                
            }
            Debug.DrawRay(
               _rayOrigin
                , Vector3.up * _yDirection
                , Color.red);
        }
    }
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(rayOffset * -2);
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
