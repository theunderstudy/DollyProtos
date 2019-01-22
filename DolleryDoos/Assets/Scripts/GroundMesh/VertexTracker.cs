using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexTracker : MonoBehaviour
{
    public int[] VertexIndexs = new int[3];
    public GroundMesh mainmesh;
    bool interaction = false;
    private Vector3 _mouseLastFrame, _mouseThisFrame,_mouseDelta;
    // Update is called once per frame
    void Update()
    { 
        if (interaction)
        { 
            //store mouse current pos
            _mouseThisFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            _mouseDelta = _mouseThisFrame - _mouseLastFrame;
            //get new lastframe
            _mouseLastFrame = _mouseThisFrame;
            //add delta to pos
            transform.position += _mouseDelta;
            //add delta to indexs 
            for (int i = 0; i < VertexIndexs.Length; i++)
            {
                mainmesh.Verts[VertexIndexs[i]] += _mouseDelta;
            }
            //redraw triangles
            mainmesh.CalculateTriangles();
        }
    }
    private void OnMouseDown()
    {
        interaction = true;
        //store current position of mouse
        _mouseLastFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp()
    {
        interaction = false;
    }
}
