using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class DollyController : MonoBehaviour
{
    public float DollyGravity = -10;
    private Vector3 dollyVelocity;

    Controller2D controller;
    private void Start()
    {
        controller = GetComponent<Controller2D>();
    }
    void Update()
    {
        dollyVelocity.y = DollyGravity;
        controller.UpdatePosition(dollyVelocity *Time.deltaTime);
    }
}
