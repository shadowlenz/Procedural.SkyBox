﻿//Written by: Eugene Chu
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
    Vector3 oldVec = Vector3.one;
    Vector3 newVec = Vector3.zero;
    public Material skyMat;

    static float time;
    public override void OnInspectorGUI()
    {
        DayNightCycle _target = (DayNightCycle)target;

        Debug.Log(StageUtility.GetMainStage());
        if (!_target.gameObject.scene.IsValid() || StageUtility.GetMainStage() == null) return;


        if ( RenderSettings.skybox == null )

        {
            GUI.color = new Color(1, 0.5f, 0.5f);
            if (!Application.isPlaying && GUILayout.Button("Setup"))
            {


                    if (RenderSettings.skybox == null) RenderSettings.skybox = skyMat;
                    else
                    {
                        Shader _shader = Shader.Find("Skybox/ProceduralGradient");
                        //Shader _shader = Shader.Find("Skybox/Skybox-Procedural");
                        RenderSettings.skybox.shader = _shader;
                    }
                

                _target.Setup();
            }


            return;
        }

        ///====================================================
        ///


        if (_target.nightLightTime > 0 && _target.nightLightTime < 1)
        {
            if (_target.nightLightTime > 0 && _target.nightLightTime < 0.5f)
            {
                GUILayout.Label("night", EditorStyles.boldLabel);
            }
            else if (_target.nightLightTime >= 0.5f && _target.nightLightTime < 0.8f)
            {
                GUILayout.Label("Late night", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("Dawn", EditorStyles.boldLabel);
            }
        }
        else
        {
            if (_target.dayLightTime >= 0 && _target.dayLightTime < 0.3f)
            {
                GUILayout.Label("Morning", EditorStyles.boldLabel);
            }
            else if (_target.dayLightTime >= 0.3f && _target.dayLightTime < 0.7f)
            {
                GUILayout.Label("Noon", EditorStyles.boldLabel);
            }
            else if (_target.dayLightTime >= 0.7f && _target.dayLightTime <= 1)
            {
                GUILayout.Label("Evening", EditorStyles.boldLabel);
            }
        }

        /////////
        DrawDefaultInspector();

        if (GUILayout.Button("Skip Time"))
        {
            _target.Debug();
        }

        EditorGUI.BeginChangeCheck();
        time = (GUILayout.HorizontalSlider(time, 0, 1));

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_target, "tweak time");
            if (_target.light != null) Undo.RecordObject(_target.light, "tweak time");
            if (_target.moonLight != null) Undo.RecordObject(_target.moonLight, "tweak time");

            _target.ChangeTimeTo(time);
            _target.UpdateSky();

        }
        else
        {
            time = _target.timeOfDay;
        }


        GUILayout.Space(50);

        if (_target.light != null && _target.moonLightGo != null && _target.moonLight != null && !Application.isPlaying)
        {

            if (_target.transform.hasChanged)
            {
                if (GUI.changed)
                {
                    //_target.SkyColor();
                    _target.UpdateSky();

                    GUI.changed = false;
                }

            }

        }

     }

    protected virtual void OnSceneGUI()
    {
        DayNightCycle _target = (DayNightCycle)target;
        EditorGUI.BeginChangeCheck();

        if (_target.light == null || _target.moonLightGo == null || _target.moonLight == null)
        {
            return;
        }


            if (Event.current.type == EventType.Repaint)
            {
                Transform transform = _target.transform;

                Handles.color = _target.light.color;
                Handles.ArrowHandleCap(
                    0,
                    transform.position + transform.forward * 3,
                    transform.rotation,
                     Vector3.Distance(Camera.current.transform.position, _target.transform.position) / 5,
                    EventType.Repaint
                    );

            }
       
        EditorGUI.EndChangeCheck();
    }



}
