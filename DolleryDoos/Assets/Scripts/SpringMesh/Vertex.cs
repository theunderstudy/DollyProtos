using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{

    public void Initialize(float min, float max, int index, int x, int y, float spring, float damp)
    {
        MinDistance = min;
        MaxDistance = max;
        Index = index;
        xPos = x;
        yPos = y;
        SpringForce = spring;
        Damping = damp;
    }
    public bool Anchor;
    public int Index;
    public int xPos, yPos;
    public Color VertColor;

    public Vector3 StartPos;
    public Vector3 DesiredPos;
    public Vector3 CurrentVelocity;
    public float MaxDistance, MinDistance;
    public float SpringForce = 10f;
    public float Damping = 5f;
    public Rigidbody2D rigidbody2D;
    //0 above
    //1 right
    //2 botright
    //3 bot
    //4 left
    //5 topleft
    public Vertex[] ConnectedVerts = new Vertex[6];

    public void UpdateVertexPos()
    {
        Vector3 _displacement = transform.position - StartPos;
        CurrentVelocity -= _displacement * SpringForce * Time.fixedDeltaTime;
        CurrentVelocity *= 1f - Damping * Time.fixedDeltaTime;
        CurrentVelocity.z = 0;
        transform.position += CurrentVelocity * Time.fixedDeltaTime;        
    }
}
