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
    Vector3 oldVec = Vector3.one;
    Vector3 newVec = Vector3.zero;
    public Material skyMat;

    static float time;
    public override void OnInspectorGUI()
    {
        DayNightCycle _target = (DayNightCycle)target;

        Debug.Log(StageUtility.GetMainStage());
        if (!_target.gameObject.scene.IsValid() || StageUtility.GetMainStage() == null) return;

        if (RenderSettings.skybox == null && !_target.useOwnShader)
        {
            EditorGUILayout.HelpBox("Please add missing sky material under 'lighting setting. Use 'Skybox/ProceduralGradient' shader", MessageType.Error);
            _target.useOwnShader = EditorGUILayout.Toggle("Use Own Shader", _target.useOwnShader);
            return;
        }

        if (_target.light == null || _target.moonLightGo == null || _target.moonLight == null ||
             (RenderSettings.skybox == null || (RenderSettings.skybox != null && RenderSettings.skybox.shader.name != "Skybox/ProceduralGradient"))
            //(RenderSettings.skybox == null || (RenderSettings.skybox != null && RenderSettings.skybox.shader.name != "Skybox/Skybox-Procedural"))
            )

        {
            GUI.color = new Color(1, 0.5f, 0.5f);
            if (!Application.isPlaying && GUILayout.Button("Setup"))
            {

                if (!_target.useOwnShader)
                {
                    if (RenderSettings.skybox == null) RenderSettings.skybox = skyMat;
                    else
                    {
                        Shader _shader = Shader.Find("Skybox/ProceduralGradient");
                        //Shader _shader = Shader.Find("Skybox/Skybox-Procedural");
                        RenderSettings.skybox.shader = _shader;
                    }
                }

                _target.Setup();
            }

            GUI.color = Color.white;
            _target.useOwnShader = EditorGUILayout.Toggle("Use Own Shader", _target.useOwnShader);
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
        }
        else
        {
            time = _target.timeOfDay;
        }


        GUILayout.Space(50);

        if (_target.light != null && _target.moonLightGo != null && _target.moonLight != null && !Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();
            if (_target.transform.hasChanged)
            {
                if (GUI.changed)
                {


                    _target.SkyColor();
                }
                oldVec = _target.light.transform.eulerAngles;
                if (oldVec != newVec)
                {

                    _target.Update();

                    oldVec = _target.light.transform.eulerAngles;
                    newVec = oldVec;

                }
            }
            EditorGUI.EndChangeCheck();
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
