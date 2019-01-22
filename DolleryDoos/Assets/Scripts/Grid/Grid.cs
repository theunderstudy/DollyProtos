using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int XSize = 10, YSize = 5;
    private Vector3[] vertices;

    private Mesh mesh;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        vertices = new Vector3[(XSize + 1) * (YSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= YSize; y++)
        {
            for (int x = 0; x <= XSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / XSize, (float)y / YSize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        yield return wait;
        //triangles
        int[] triangles = new int[XSize * YSize * 6];
        for (int triangleIndex = 0, vertexIndex = 0, y = 0; y < YSize; y++, vertexIndex++)
        {
            for (int x = 0; x < XSize; x++, triangleIndex += 6, vertexIndex++)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 4] = triangles[triangleIndex + 1] = vertexIndex + XSize + 1;
                triangles[triangleIndex + 5] = vertexIndex + XSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.tangents = tangents;
    }
    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
