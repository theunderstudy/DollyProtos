using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVertex : MonoBehaviour
{
    public GroundMesh Mainmesh;

    public void UpdatePosition(MeshVertex instigatingVertex)
    {
        //get nearby vert
        for (int i = 0; i < Mainmesh.MeshVerts.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, Mainmesh.MeshVerts[i].transform.position);
            //if a distance from this vertex is closer than the minimum distance
            if (dist < Mainmesh.MinimumDistance)
            {
                if (instigatingVertex == Mainmesh.MeshVerts[i])
                {
                    continue;
                }
                if (instigatingVertex == this)
                {
                    continue;
                }
                //push the vertex away from this vertex
                Vector3 newPos = Mainmesh.MeshVerts[i].transform.position - transform.position;
                newPos *= 0.2f;
                Mainmesh.MeshVerts[i].transform.position += newPos;
            }
        }
    }
}
