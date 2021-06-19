using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SetDayNightCycle))]
public class SetDayNightCycleEditor : Editor
{
    SetDayNightCycle _target;
    DayNightCycle DayNightCycle;
    private void OnEnable()
    {
        _target = (SetDayNightCycle)target;

        DayNightCycle = (DayNightCycle)FindObjectOfType(typeof(DayNightCycle));
    }
    public override void OnInspectorGUI()
    {
        _target = (SetDayNightCycle)target;
        DrawDefaultInspector();

        //
        GUILayout.Space(20);
        bool HasDayNightCycle = DayNightCycle != null && !Application.isPlaying;

        GUI.color = Color.red;
        string messageLog = "No ( DayNightCycle ) to preview";
        if (HasDayNightCycle)
        {
            GUI.color = Color.green;
            messageLog = "Has ( " + DayNightCycle.gameObject.name + " ) to preview";
        }
        GUILayout.Label(messageLog);
        GUI.color = Color.white;
        //

        if (!HasDayNightCycle) return;

        if (GUI.changed)
        {
            if (_target.activeDayNight)
            {
                DayNightCycle.skyData = _target.SkyData;
                DayNightCycle.Setup();
                DayNightCycle.UpdateSky();
                if (_target.canOverrideTime) DayNightCycle.ChangeTimeTo(_target.overrideTime);
            }
            GUI.changed = false;
        }
    }
}
