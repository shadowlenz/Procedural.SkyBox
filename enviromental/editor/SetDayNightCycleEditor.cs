using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SetDayNightCycle))]
public class SetDayNightCycleEditor : Editor
{
    public DayNightCycle spawnDayNightCycle;

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

        string messageLog;
        if (HasDayNightCycle)
        {
            GUI.color = Color.green;
            messageLog = "Has ( " + DayNightCycle.gameObject.name + " ) to preview";
        }
        else
        {
            GUI.color = new Color(1f,0.5f,0.5f);
            messageLog = "No ( DayNightCycle ) exist to preview";

            if (spawnDayNightCycle != null)
            {
                if (GUILayout.Button("Spawn DayNightCycle to Preview"))
                {
                    DayNightCycle _spawnedDayNight= (DayNightCycle) PrefabUtility.InstantiatePrefab(spawnDayNightCycle);
                    DayNightCycle = _spawnedDayNight;

                    RefreshDayNight();
                }
            }
        }

        GUILayout.Label(messageLog);
        GUI.color = Color.white;
        //

        if (!HasDayNightCycle) return;

        if (GUI.changed)
        {
            RefreshDayNight();

            GUI.changed = false;
        }
    }

    void RefreshDayNight()
    {
        if (_target.activeDayNight)
        {
            DayNightCycle.skyData = _target.SkyData;
            DayNightCycle.Setup();
            DayNightCycle.UpdateSky();
            DayNightCycle.ChangeTimeTo(_target.overrideTime);
        }
    }
}
