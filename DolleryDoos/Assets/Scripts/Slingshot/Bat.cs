using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public CamController cam;
    public float UpForce = 1.5f;
    private Rigidbody2D rbreezy;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && cam.CurrentState == CamController.CamState.Follow)
        {
            Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            temp.z = 0;
            
            transform.position = temp;
        }
    }

  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "proj")
        {
            if (!rbreezy)
            {
                rbreezy = collision.gameObject.GetComponent<Rigidbody2D>();
            }
            else
            {
                rbreezy.AddForce(Vector2.up * UpForce , ForceMode2D.Impulse);
                return;
                Vector2 _velocity = rbreezy.velocity;

                if (_velocity.y < 0)
                {
                    _velocity.y = 0;
                }
                
                _velocity *= -1;
                _velocity.y = UpForce;
                rbreezy.velocity = _velocity;
            }
        }
    }
}
