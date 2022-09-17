using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        if (Application.isPlaying) return;

        GUILayout.Space(20);
        GUILayout.BeginVertical(EditorStyles.helpBox);

        bool HasDayNightCycle = DayNightCycle != null;

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

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        GUILayout.Label(messageLog);
        GUI.color = Color.white;
        //========================================================
        EditorGUI.BeginChangeCheck();

        SerializedProperty overrideTime_p = serializedObject.FindProperty(nameof(_target.overrideTime));
        EditorGUILayout.PropertyField(overrideTime_p,new GUIContent("Preview Time"));
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            RefreshDayNight();
        }
        //==============================================

        GUILayout.EndVertical();
    }

    void RefreshDayNight()
    {
        if (_target.activeDayNight)
        {
         
            DayNightCycle.Setup(_target.SkyData);
            DayNightCycle.SkyOnUpdate();
            DayNightCycle.ChangeTimeTo(_target.overrideTime);
        }
    }
}
