using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeObject : MonoBehaviour
{
    [System.Serializable]
    public struct TimeData
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Scale;
    }

    public List<TimeData> TimeDataList;
    public int FramesStored = 500;

    protected virtual void StoreTimeData()
    {
        //make a new time data
        TimeData temp;
        temp.Position = transform.position;
        temp.Velocity = Vector3.zero;
        temp.Scale = transform.localScale;
        TimeDataList.Add(temp);
        if (TimeDataList.Count > FramesStored)
        {
            TimeDataList.RemoveAt(0);
        }
    }
    private void Update()
    {
        switch (TimeController.CurrentTimeState)
        {
            case TimeController.TimeState.Normal:
                StoreTimeData();
                break;
            case TimeController.TimeState.Back:
                ReverseFrame();
               
                return;
            case TimeController.TimeState.Paused:
                return;
            default:
                break;
        }

        UpdateLoop();
    }

    protected virtual void ReverseFrame()
    {

    }

    protected virtual void UpdateLoop()
    {

    }

}
