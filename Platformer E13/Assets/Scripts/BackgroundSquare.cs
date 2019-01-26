using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BackgroundSquare : MonoBehaviour
{

    public float AliveTimeMax = 3, AliveTimeMin = 2;
    public float Alivetime = 0, CurrentAliveTime = 0;
    public float RandomMoveMax = 0.2f;
    Vector2 startpos;
    public float MaxTimeBetweenMove = 0.6f , MinTimeBetweenMpve = 0.2f;
    public float TimeBetweenMove = 0, CurrentTimeBetweenMove = 0;
    private void Start()
    {
        Alivetime = Random.Range(AliveTimeMin , AliveTimeMax);
        TimeBetweenMove = Random.Range(MinTimeBetweenMpve ,MaxTimeBetweenMove);
        startpos = transform.position;
    }
    // Update is called once per frame
    void Update ()
    {
        CurrentAliveTime += Time.deltaTime;
        if (CurrentAliveTime >= Alivetime)
        {
            transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(gameObject));
        }
        CurrentTimeBetweenMove += Time.deltaTime;
        if (CurrentTimeBetweenMove >= TimeBetweenMove)
        {
            CurrentTimeBetweenMove = 0;
            Vector2 temp = startpos;
            temp.x += Random.Range(-RandomMoveMax , RandomMoveMax);
            temp.y += Random.Range(-RandomMoveMax, RandomMoveMax);
            transform.position = temp;

        }
	}
}
