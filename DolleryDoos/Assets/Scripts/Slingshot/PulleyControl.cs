using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulleyControl : MonoBehaviour
{
    bool Dragging = false;
    private Vector3 _mouseStart;
    private void OnMouseDown()
    {
        Dragging = true;
        _mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void Update()
    {
        if (Dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _mouseStart;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Dragging = false;
            FindObjectOfType<CamController>().CurrentState = CamController.CamState.Follow;
        }
    }
}
