using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//triggers events based on DayNight time
public class DayNight_TimeEvent : MonoBehaviour
{
    [Header("0: sunrise | 0.5: sunset | 1: sunrise")]
    [Tooltip("between 0-1 will trigger a state change")]
    public Vector2 minMaxTimeTrigger = new Vector2(0,0.5f);
    [Tooltip("trigger on or off event within the min and max")]
    public bool invertLogic;
    [Space()]
    [Space()]
    public UnityEvent onTime;
    public UnityEvent offTime;

    [Header("debug")]
    public TimeState timeState = TimeState.None;
    public enum TimeState { None = 0, On = 1, Off = 2 }
    public float CurrentTime;

    private void Update()
    {
        if (DayNightCycle.instance == null)
        {
            timeState = TimeState.None;
            return;
        }

        CurrentTime = DayNightCycle.instance.timeOfDay;
        if (CurrentTime >= minMaxTimeTrigger.x && CurrentTime <= minMaxTimeTrigger.y)
        {
            if (!invertLogic) Do_OnTime();
            else Do_OffTime();
        }
        else
        {
            if (!invertLogic) Do_OffTime();
            else Do_OnTime();
        }

    }


    void Do_OnTime()
    {
        if (timeState != TimeState.On)
        {
            ///trigger once
            onTime.Invoke();

            timeState = TimeState.On;
        }
    }
    void Do_OffTime()
    {
        if (timeState != TimeState.Off)
        {
            ///trigger once
            offTime.Invoke();

            timeState = TimeState.Off;
        }
    }
}
