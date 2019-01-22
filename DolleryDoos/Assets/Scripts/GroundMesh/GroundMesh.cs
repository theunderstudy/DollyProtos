using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class GroundMesh : MonoBehaviour
{
    //lets start with 4 points
    //bot left, right
    //mid left, right
    public Vector3[] Verts;
    public MeshVertex[] MeshVerts;
    public int[] Triangles;
    public int yVerts, xVerts;
    private Mesh mesh;
    public Material TriangleMat;

    public float noise = 0.5f;

    public float MinimumDistance = 2;

    public GameObject Dolly;

    public float someDampeningForce = 1;
    public float MaxHorizontalDistance = 2;
    public float MaxVerticalDistance = 2;
    public float clampmin, clampmax;
    List<Vector2> colliderPath = new List<Vector2>();
    PolygonCollider2D polygonCollider;
    public Vector3 TopLeftCorner, TopRightCorner, BotLeftCorner, BotRightCorner;
    private void Awake()
    {
        //generate the mesh
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        PopulateVerts();
        CalculateTriangles();
        Vector3 temp = Dolly.transform.position;
        temp.z = this.Verts[0].z;
        Dolly.transform.position = temp;
    }

    bool interaction = false;
    private Vector3 _mouseLastFrame, _mouseThisFrame, _mouseDelta;
    private int _nearVertexIndex;
    private void Update()
    {
        //check if dolly is inside the collider
        if (Input.GetMouseButtonDown(0))
        {
            ///get the vertex closest to the mouse pointer
            //get mouse pos
            _mouseLastFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //get index for closest vertex
            float currentDistance = 555555555;
            for (int i = 0; i < Verts.Length; i++)
            {
                if (Mathf.Abs(Vector3.Distance(_mouseLastFrame, Verts[i])) < currentDistance)
                {
                    currentDistance = Mathf.Abs(Vector3.Distance(_mouseLastFrame, Verts[i]));
                    _nearVertexIndex = i;
                }
            }
            interaction = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            interaction = false;
        }
        if (interaction)
        {
            //store mouse current pos
            _mouseThisFrame = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mouseDelta = _mouseThisFrame - _mouseLastFrame;
            //get new lastframe
            _mouseLastFrame = _mouseThisFrame;
            ////update position of the closest vertex
            //Verts[_nearVertexIndex] += _mouseDelta;

            ///calculate a movement ector
            ///move all verts on the horizontal axis the delta x
            ///shrink the delta by the verticle distance
            ///
            Vector3 temp = Vector3.zero;
            for (int i = 0; i < Verts.Length; i++)
            {
                temp.x = Mathf.Lerp(_mouseDelta.x, 0, Mathf.InverseLerp(0, MaxHorizontalDistance, Vector2.Distance(Verts[i], _mouseThisFrame)));
                temp.y = Mathf.Lerp(_mouseDelta.y, 0, Mathf.InverseLerp(0, MaxVerticalDistance, Vector2.Distance(Verts[i], _mouseThisFrame)));

                temp /= Mathf.Clamp(clampmin , clampmax, Mathf.Log10(Mathf.Abs(Verts[i].y - _mouseLastFrame.y) ));
              
               
                //reduce pull by how far away the point is on the main axis
                //temp.x = Mathf.Lerp(_mouseDelta.x, 0, Mathf.Abs(Verts[i].y - _mouseLastFrame.y) ) * Vector3.Distance(Verts[i], _mouseThisFrame);
                //temp.y = Mathf.Lerp(_mouseDelta.y, 0, Mathf.Abs(Verts[i].x - _mouseLastFrame.x) )   ;
                Verts[i] += temp;
            }
            ///move all verts on the verticle axis by the deltay
            ///shrink the delta y by the horizontal distance from selected spot and the current vert
            ///

            //redraw triangles
            CalculateTriangles();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            for (int i = 0; i < Triangles.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                GameObject GO = new GameObject("Triangle " + (i / 3));
                VertexTracker vertexTrack = GO.AddComponent<VertexTracker>();
                vertexTrack.mainmesh = this;
                for (int n = 0; n < 3; n++)
                {
                    int index = Triangles[i + n];
                    vertexTrack.VertexIndexs[n] = Triangles[i + n];
                    newVerts[n] = Verts[index];
                    newUvs[n] = mesh.uv[index];
                    newNormals[n] = mesh.normals[index];

                }
                Mesh _mesh = new Mesh();
                _mesh.vertices = newVerts;
                _mesh.normals = newNormals;
                _mesh.uv = newUvs;
                _mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;

                GO.AddComponent<MeshRenderer>().material = TriangleMat;
                GO.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, Random.Range(0.2f, 0.8f));
                GO.AddComponent<MeshFilter>().mesh = _mesh;
                Color temp = GO.GetComponent<MeshRenderer>().material.color;
                temp.a = 0;
                GO.GetComponent<MeshRenderer>().material.color = temp;
                GO.AddComponent<PolygonCollider2D>();
                // GO.GetComponent<EdgeCollider2D>().points = newVerts;
                Rigidbody2D rb = GO.AddComponent<Rigidbody2D>();
                rb.mass = Random.Range(0.5f, 1.0f);
                //  rb.AddExplosionForce(10, Vector3.up, 30, 0, ForceMode.Impulse);
                // Destroy(GO, 1 + Random.Range(5.0f, 10.0f));
                gameObject.SetActive(false);
            }
        }
    }
    private void CalculateVelocity(int index, Vector3 point, float force)
    {

    }
    private void PopulateVerts()
    {
        Verts = new Vector3[(yVerts + 1) * (xVerts + 1)];
        Color32 currentColor = new Color32();
        Color[] colors = new Color[Verts.Length];
        //uv array
        Vector2[] uv = new Vector2[Verts.Length];

        Camera camera = Camera.main;
        //x,y
        Vector3 _screenBotLeft = camera.ViewportToWorldPoint(BotLeftCorner);
        Vector3 _screenBotRight = camera.ViewportToWorldPoint(BotRightCorner);
        Vector3 _screenMidLeft = camera.ViewportToWorldPoint(TopLeftCorner);
        Vector3 _screenMidRight = camera.ViewportToWorldPoint(TopRightCorner);
        int vertIndex = 0;
        for (int yPos = 0; yPos <= yVerts; yPos++)
        {
            //generate points along the x axis
            for (int xPos = 0; xPos <= xVerts; xPos++)
            {
                //place verts along the x index
                Verts[vertIndex] = Vector3.Lerp(_screenBotLeft, _screenBotRight, (float)xPos / xVerts);
                //set the height for the point
                Verts[vertIndex].y = Mathf.Lerp(_screenBotLeft.y, _screenMidLeft.y, (float)yPos / yVerts);
                Verts[vertIndex] += new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), 0);
                Verts[vertIndex].z = 0;
                //set the uv coords
                uv[vertIndex] = new Vector2((float)xPos / xVerts, (float)yPos / yVerts);
                currentColor = Color.Lerp(Color.red, Color.white, Random.Range(0.2f, 0.8f));
                //new Color(
                //                 Random.Range(0.0f, 1.0f),
                //                 Random.Range(0.0f, 1.0f),
                //                 Random.Range(0.0f, 1.0f),
                //                 1.0f);
                colors[vertIndex] = currentColor;
                //increment the vert index
                vertIndex += 1;
            }
        }
        mesh.vertices = Verts;
        mesh.uv = uv;
        mesh.colors = colors;
    }
    public void CalculateTriangles()
    {
        mesh.vertices = Verts;
        Triangles = new int[xVerts * yVerts * 6];
        int _triangleIndex = 0;
        int _vertexIndex = 0;
        //reset the collider
        polygonCollider.pathCount = 0;
        for (int y = 0; y < yVerts; y++)
        {
            //to form a square, we need two triangles
            for (int i = 0; i < xVerts; i++)
            {
                //first triangle of square
                Triangles[_triangleIndex] = _vertexIndex;//start in the bot left
                Triangles[_triangleIndex + 1] = _vertexIndex + xVerts + 1;//go up a row for the top left
                Triangles[_triangleIndex + 2] = _vertexIndex + 1;//bot right point
                                                                 //the second triangle
                Triangles[_triangleIndex + 3] = _vertexIndex + xVerts + 1;//reuse the vertex in the top left of the square
                Triangles[_triangleIndex + 4] = _vertexIndex + xVerts + 2;//get the vertex top right of the square
                Triangles[_triangleIndex + 5] = _vertexIndex + 1;//reuse the bot right point
                                                                 ///add the poly collider
                colliderPath.Clear();
                colliderPath.Add(Verts[_vertexIndex]);
                colliderPath.Add(Verts[_vertexIndex + xVerts + 1]);
                colliderPath.Add(Verts[_vertexIndex + 1]);

                colliderPath.Add(Verts[_vertexIndex + xVerts + 1]);
                colliderPath.Add(Verts[_vertexIndex + xVerts + 2]);
                colliderPath.Add(Verts[_vertexIndex + 1]);
                polygonCollider.pathCount++;
                polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                //increment the triangle index by a full square
                _triangleIndex += 6;

                //increment the vertex index one column over
                _vertexIndex += 1;
            }
            //increment the vertex index
            _vertexIndex += 1;
        }
        mesh.triangles = Triangles;
        mesh.RecalculateNormals();
    }


    private void OnDrawGizmos()
    {
        if (Verts == null)
        {
            return;
        }
        for (int i = 0; i < Verts.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(Verts[i], 0.1f);
        }
    }

}
