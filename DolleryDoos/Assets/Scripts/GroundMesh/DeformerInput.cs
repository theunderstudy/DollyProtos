using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformerInput : MonoBehaviour
{
    public float force = 10f;
    public float forceOffset = 0.1f;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
          //  HandleInput();
        }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(inputRay.origin, inputRay.direction);
        //RaycastHit hit;
        if (hit)
        {
            Deformer deformer = hit.collider.GetComponent<Deformer>();
            if (deformer)
            {
                Vector2 point = hit.point;
                deformer.AddDeformingForce(point, force);
            }
        }
    }
}
