using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class SpringMesh : MonoBehaviour
{
    public List<Vertex> VertList;
    public MeshVertex[] MeshVerts;
    public int[] Triangles;
    public int xVerts, yVerts;
    private Mesh mesh;
    private PolygonCollider2D polyCollider;
    private List<Vector2> colliderPath = new List<Vector2>();
    public Material Mat;
    public Vector3 TopLeft, TopRight, BotRight, BotLeft;
    public float MaxDistanceBetweenVerts, MinDistanceBetweenVerts;
    public Vertex VertPrefab;
    public Vertex SelectedVert;
    public List<Vertex> JoinedVerts;
    private Vector3 MouseStartPos, MouseCurrentPos;
    private bool Interacting = false;
    public float SpringForce, ForceDamp;
    private void Awake()
    {
        //generate mesh
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        polyCollider = GetComponent<PolygonCollider2D>();
        PopulateVerts();
        PopulateTriangles();
    }

    public void PopulateVerts()
    {
        VertList.Clear();
        //uv array
        Vector2[] uv = new Vector2[(yVerts + 1) * (xVerts + 1)];
        Camera camera = Camera.main;
        Color _vertColor;
        Color[] colors = new Color[(yVerts + 1) * (xVerts + 1)];

        //color the triangle


        //x,y
        Vector3 _screenBotLeft = camera.ViewportToWorldPoint(BotLeft);
        Vector3 _screenBotRight = camera.ViewportToWorldPoint(BotRight);
        Vector3 _screenMidLeft = camera.ViewportToWorldPoint(TopLeft);
        Vector3 _screenMidRight = camera.ViewportToWorldPoint(TopRight);
        int vertIndex = 0;
        for (int ypos = 0; ypos <= yVerts; ypos++)
        {
            for (int xpos = 0; xpos <= xVerts; xpos++)
            {
                Vertex temp = Instantiate(VertPrefab);
                temp.Initialize(MinDistanceBetweenVerts, MaxDistanceBetweenVerts, VertList.Count, xpos, ypos, SpringForce, ForceDamp);
                Vector3 _pos;
                _pos = Vector3.Lerp(_screenBotLeft, _screenBotRight, (float)xpos / xVerts);
                _pos.y = Mathf.Lerp(_screenBotLeft.y, _screenMidLeft.y, (float)ypos / yVerts);
                _pos.z = 0;
                temp.transform.position = _pos;
                temp.StartPos = _pos;
                uv[vertIndex] = new Vector2((float)xpos / xVerts, (float)ypos / yVerts);
                //color
                _vertColor = Color.Lerp(Color.black, Color.white, Random.Range(0.2f, 0.8f));
                temp.VertColor = _vertColor;
                colors[vertIndex] = _vertColor;

                vertIndex += 1;
                //check if point is an anchor
                if (xpos == 0 || xpos == xVerts)
                {
                    temp.Anchor = true;
                }
                else
                    temp.Anchor = false;
                VertList.Add(temp);


                temp.ConnectedVerts[2] = GetVertex(temp.Index - xVerts);
                temp.ConnectedVerts[3] = GetVertex(temp.Index - xVerts - 1);
                temp.ConnectedVerts[4] = GetVertex(temp.Index - 1);
                for (int i = 0; i < 6; i++)
                {
                    if (temp.ConnectedVerts[i])
                    {
                        temp.ConnectedVerts[i].ConnectedVerts[i < 3 ? i + 3 : i - 3] = temp;
                    }
                }
            }
        }
        Vector3[] verts = new Vector3[VertList.Count];
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = VertList[i].transform.position;
        }
        mesh.vertices = verts;
        mesh.colors = colors;
        mesh.uv = uv;
    }
    public Vertex GetVertex(int index)
    {
        if (index < VertList.Count && index > -1)
        {
            return VertList[index];
        }
        return null;
    }
    public void PopulateTriangles()
    {
        Vector3[] verts = new Vector3[VertList.Count];
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = VertList[i].transform.position;
        }
        mesh.vertices = verts;
        Triangles = new int[xVerts * yVerts * 6];
        int _triangleIndex = 0;
        int _vertexIndex = 0;
        //reset the collider
        polyCollider.pathCount = 0;
        //triangle colors
        for (int y = 0; y < yVerts; y++)
        {
            for (int x = 0; x < xVerts; x++)
            {
                //populate connected verts
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
                colliderPath.Add(verts[_vertexIndex]);
                colliderPath.Add(verts[_vertexIndex + xVerts + 1]);
                colliderPath.Add(verts[_vertexIndex + 1]);

                colliderPath.Add(verts[_vertexIndex + xVerts + 1]);
                colliderPath.Add(verts[_vertexIndex + xVerts + 2]);
                colliderPath.Add(verts[_vertexIndex + 1]);
                polyCollider.pathCount++;
                polyCollider.SetPath(polyCollider.pathCount - 1, colliderPath.ToArray());
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
    private void OnMouseDown()
    {
        Interacting = true;
        MouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //get the nearest vert
        float dist = 1000000;
        for (int i = 0; i < VertList.Count; i++)
        {
            if (Vector3.Distance(MouseStartPos, VertList[i].transform.position) < dist)
            {
                dist = Vector3.Distance(MouseStartPos, VertList[i].transform.position);
                SelectedVert = VertList[i];
            }
        }
        JoinedVerts.Clear();
        PopulateJoinedVerts(SelectedVert);
    }
    private void PopulateJoinedVerts(Vertex vert)
    {
        if (JoinedVerts.Contains(vert))
        {
            return;
        }
        JoinedVerts.Add(vert);
        if (vert.ConnectedVerts[0])
        {
            PopulateJoinedVerts(vert.ConnectedVerts[0]);
        }
        if (vert.ConnectedVerts[3])
        {
            PopulateJoinedVerts(vert.ConnectedVerts[3]);
        }
    }
    private void OnMouseUp()
    {
        Interacting = false;
    }
    private void FixedUpdate()
    {
        if (Interacting)
        {
            MouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MouseCurrentPos.z = 0;
            PullMesh();
        }
        else
        {
            for (int i = 0; i < VertList.Count; i++)
            {
                VertList[i].UpdateVertexPos();
            }
            PopulateTriangles();
        }
    }
    private void PullMesh()
    {
        //verts should have a max distance that they can be pulled apart from each other
        //if clicking on the mesh, get the vertex(s) closest to the mouse
        //disable spring logic of the vertexs
        //move the selected vertex by the mouse
        //all other vertexs going back to the anchor should distribute the total distance moved across all verts
        //this should deminish closer to the anchor   
        SelectedVert.transform.position = MouseCurrentPos;
        //update the position of the joined verts
        Vector3 _deltaPos = SelectedVert.transform.position - SelectedVert.StartPos;
        for (int i = 0; i < JoinedVerts.Count; i++)
        {
            JoinedVerts[i].transform.position = JoinedVerts[i].StartPos + _deltaPos;
        }
        //lerp positions of all non anchored verts between selected vert and nearest anchor 
        UpdateRowOfVerts();
        PopulateTriangles();

    }

    private void UpdateRowOfVerts()
    {
        //lerp the height of vertexs based on number of verts left/right of the selected vert 
        for (int i = 0; i < VertList.Count; i++)
        {
            if (VertList[i].Anchor)
            {
                continue;
            }
            Vertex _cent = SelectedVert;
            for (int x = 0; x < JoinedVerts.Count; x++)
            {
                if (JoinedVerts[x].yPos == VertList[i].yPos)
                {
                    _cent = JoinedVerts[x];
                }
            }
            if (VertList[i].xPos < SelectedVert.xPos)
            {
                //find an anchor on the left
                Vector3 temp = Vector3.Lerp(VertList[VertList[i].yPos * (xVerts + 1)].StartPos,
                    _cent.transform.position,
                    (float)VertList[i].xPos / _cent.xPos);
                VertList[i].transform.position = temp;
            }
            else if (VertList[i].xPos > _cent.xPos)
            {
                Vertex _anchor = VertList[(xVerts * (VertList[i].yPos + 1)) + VertList[i].yPos];
                Vector3 temp = Vector3.Lerp(
                    _cent.transform.position
                    , _anchor.StartPos
                    ,Mathf.InverseLerp(_cent.xPos , _anchor.xPos , VertList[i].xPos));
                VertList[i].transform.position = temp;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mesh == null)
        {
            return;
        }
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(mesh.vertices[i], 0.1f);
        }
    }

}
