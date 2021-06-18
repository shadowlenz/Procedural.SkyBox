//Written by: Eugene Chu
//Twitter @LenZ_Chu
//Free to use. Please mention or credit my name in projects if possible!

//drag this to a gameobject and click 'setup' once.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
[CustomEditor(typeof(DayNightCycle))]
public class DayNightEditor : Editor
{
    public Material skyMat;

    private void OnEnable()
    {
        DayNightCycle _target = (DayNightCycle)target;

        bool InMainStage = StageUtility.GetCurrentStage() == StageUtility.GetMainStage() && _target.gameObject.scene.IsValid();
        if (InMainStage)
        {
            _target.UpdateSky();
        }
    }
    public override void OnInspectorGUI()
    {
        DayNightCycle _target = (DayNightCycle)target;

        bool InMainStage = StageUtility.GetCurrentStage() == StageUtility.GetMainStage();
        if (!_target.gameObject.scene.IsValid() || StageUtility.GetMainStage() == null) return;

        if (InMainStage)
        {
            if (!Application.isPlaying && (RenderSettings.skybox == null || RenderSettings.skybox != _target.skyData.skyBoxMat))

            {
                GUI.color = new Color(1, 0.5f, 0.5f);
                if (GUILayout.Button("Setup"))
                {
                    if (_target.skyData.skyBoxMat != null) RenderSettings.skybox = _target.skyData.skyBoxMat;
                    else
                    {
                        RenderSettings.skybox = skyMat;

                        SerializedProperty P_skydata = serializedObject.FindProperty(nameof(_target.skyData));
                        SerializedProperty P_skydata_skyMat = P_skydata.FindPropertyRelative(nameof(_target.skyData.skyBoxMat));
                        P_skydata_skyMat.objectReferenceValue = skyMat;

                        serializedObject.ApplyModifiedProperties();
                    }

                    RenderSettings.sun = _target.sunLight;

                    _target.Setup();
                }
                GUI.color = Color.white;
            }
        }

        ///====================================================

        DrawDefaultInspector();
        GUILayout.Space(20);

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label(GetTimeLabel(), EditorStyles.boldLabel);

        SerializedProperty m_timeOfDay = serializedObject.FindProperty(nameof(DayNightCycle.timeOfDay));
        EditorGUILayout.PropertyField(m_timeOfDay, new GUIContent(""));
        serializedObject.ApplyModifiedProperties();

        GUILayout.EndVertical();


        GUILayout.Space(50);

        if (_target.sunLight != null && _target.moonLightGo != null && _target.moonLight != null && !Application.isPlaying)
        {

            if (GUI.changed)
            {
                Undo.RecordObject(_target, "tweak time");
                if (_target.sunLight != null) Undo.RecordObject(_target.sunLight, "tweak time");
                if (_target.moonLight != null) Undo.RecordObject(_target.moonLight, "tweak time");


                _target.UpdateSky();



                GUI.changed = false;
            }

        }

    }

    string GetTimeLabel()
    {
        DayNightCycle _target = (DayNightCycle)target;

        string LabelReturn = string.Empty;
        if (_target.nightLightTime > 0 && _target.nightLightTime < 1)
        {
            if (_target.nightLightTime > 0 && _target.nightLightTime < 0.5f)
            {
                LabelReturn = "night";
            }
            else if (_target.nightLightTime >= 0.5f && _target.nightLightTime < 0.8f)
            {
                LabelReturn = "Late night";
            }
            else
            {
                LabelReturn = "Dawn";
            }
        }
        else
        {
            if (_target.dayLightTime >= 0 && _target.dayLightTime < 0.3f)
            {
                LabelReturn = "Morning";
            }
            else if (_target.dayLightTime >= 0.3f && _target.dayLightTime < 0.7f)
            {
                LabelReturn = "Noon";
            }
            else if (_target.dayLightTime >= 0.7f && _target.dayLightTime <= 1)
            {
                LabelReturn = "Evening";
            }
        }
        return LabelReturn;
    }

    protected virtual void OnSceneGUI()
    {
        DayNightCycle _target = (DayNightCycle)target;

        if (!_target.gameObject.scene.IsValid() || StageUtility.GetMainStage() == null) return;
        if (_target.sunLight == null || Camera.current == null)
        {
            return;
        }


        if (Event.current.type == EventType.Repaint)
        {
            Handles.color = _target.sunLight.color;
            Handles.ArrowHandleCap(
                0,
                _target.sunLight.transform.position + _target.sunLight.transform.forward,
                _target.sunLight.transform.rotation,
                    Vector3.Distance(Camera.current.transform.position, _target.transform.position) / 5,
                EventType.Repaint
                );

        }
    }


}
