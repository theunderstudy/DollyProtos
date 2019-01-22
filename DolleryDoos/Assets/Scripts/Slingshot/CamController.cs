using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public enum CamState {Starting, Follow }
    public CamState CurrentState = CamState.Starting;
    public Vector3 velocity;

    public GameObject Projectile;
	// Update is called once per frame
	void Update () {
        switch (CurrentState)
        {
            case CamState.Starting:
                break;
            case CamState.Follow:
                if (Camera.main.ScreenToViewportPoint(Projectile.transform.position).y>0)
                {
                    //add the difference to the camera pos
                    Vector3 temp = transform.position;
                    temp.y = Projectile.transform.position.y;
                    transform.position = temp;
                }
                break;
            default:
                break;
        }
    }
}
