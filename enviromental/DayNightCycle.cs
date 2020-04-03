//Written by: Eugene Chu
//Twitter @LenZ_Chu
//Free to use. Please mention or credit my name in projects if possible!

//drag this to a gameobject and click 'setup' once.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    public static DayNightCycle instance;
    public bool useOwnShader;

    public Light light;
    public float lightIntensity = 1.3f;
    public float moonLightIntensity = 1;

    public float speed = 1f;
	[Space ()]
	public AnimationCurve sunSizeOverTime;
	[Space ()]
    public Gradient dayTopColorOverTime;
    public Gradient dayColorOverTime;
    [Space()]
    public Gradient nightTopColorOverTime;
    public Gradient nightColorOverTime;
    [Space()]
    public Gradient dayFogOverTime;

    [Range(0,1)]
    public float fogToDayColor;
    Color _dayFogOverTime;


    [Header("debug")]
    public float degreesSkip = 45;

    [Range(0, 1)]
    public float timeOfDay = 0;
    [Range (0,1)]
	public float dayLightTime;
	[Range (0,1)]
	public float nightLightTime;

    [HideInInspector()]
    public GameObject moonLightGo;
    [HideInInspector()]
    public Light moonLight;

    [HideInInspector()]
    [Range(0, 1)]
    public float fadeNight;
    [HideInInspector()]
    [Range(0, 1)]
    public float smoothFadeNight;
    [HideInInspector()]
    public Material sky; //sky from RenderSetting

    // Use this for initialization
    void Start () {
        instance = this;
        Setup();
    }

    public void Setup()
    {
        if (light == null)
        {
            if (GetComponent<Light>() == null) light = gameObject.AddComponent<Light>();
            else light = gameObject.GetComponent<Light>();
        }

        //automatically creates the moon with light source
        if (moonLightGo == null)
        {
            moonLightGo = new GameObject("moonLight"); // if there are no gameobj

            moonLightGo.transform.SetParent(transform, false);
            moonLightGo.transform.Rotate(180, 0, 0);
        }
        if (moonLight == null)
        {
            moonLight = moonLightGo.AddComponent<Light>(); //create light if none exsist

            moonLight.type = LightType.Directional;
            moonLight.shadows = LightShadows.None;
            //grab sky material from render settings
        }

            sky = RenderSettings.skybox = new Material(RenderSettings.skybox);

    }
    public void TimeOfDayBar()
    {
        if (nightLightTime > 0 && dayLightTime > 0 && dayLightTime < 1) nightLightTime = 0;
        float isNight = 0;
        if (nightLightTime > 0) isNight = 1;
        timeOfDay = (dayLightTime + nightLightTime + isNight) / 2;
    }
    public void ChangeTimeTo(float range)
    {
        light.transform.localEulerAngles =new Vector3(range*360, 0, 0);
    }

    // Update is called once per frame
    public void Update () {
        if (DebugGame.instance != null && DebugGame.instance.inDebug) return;
        //rotate light 360 over time
        light.transform.Rotate (new Vector3 ( (Time.deltaTime * speed), 0,0));
		//calculate debug
		Vector3 currentRot = GetYawPitch.GetPitchYawRollDeg (light.transform.rotation  );
		dayLightTime = Mathf.Clamp01 ( currentRot.x/180);
        float _nightLightTime = Mathf.Clamp01(-currentRot.x / 180);
        nightLightTime = Mathf.Lerp(1, 0, _nightLightTime);
        ///
        TimeOfDayBar();

        //shuts lights as it goes over the horizon
        if ( dayLightTime <= 0 || dayLightTime >= 1)
        {
            //day to night time========================================= ☽︎
            if (fadeNight >= 1)
            {
                light.enabled = false;
            }
            else
            {
                light.enabled = true;
            }
            light.intensity = Mathf.Lerp(lightIntensity, 0, fadeNight);

            moonLight.enabled = true;
            moonLight.intensity = Mathf.Lerp(0, moonLightIntensity, fadeNight);
        } else {
            //night to day time=========================================== ☼
            
            if (fadeNight <= 0)
            {
                moonLight.enabled = false;
            }
            else
            {
                moonLight.enabled = true;
            }
            moonLight.intensity = Mathf.Lerp(0, moonLightIntensity, fadeNight);

            light.enabled = true;
            light.intensity = Mathf.Lerp(lightIntensity, 0, fadeNight);

        }

        SkyColor();
        
    }


    public void Debug()
    {

            light.transform.Rotate(new Vector3(degreesSkip, 0, 0));
    }

    public void SkyColor()
    {
        SmoothNight();
        //change direcrtional color on light over time
        light.color = dayColorOverTime.Evaluate(dayLightTime);
        moonLight.color = nightColorOverTime.Evaluate(nightLightTime);

        //sky.SetColor("_SkyColor1", Color.Lerp(dayColorOverTime.Evaluate(dayLightTime), dayFogOverTime.Evaluate(dayLightTime), smoothFadeNight));
        if (dayLightTime > 0)
        {
            if (!useOwnShader)
            {
                sky.SetColor("_SkyColor1", (dayTopColorOverTime.Evaluate(dayLightTime)));
                sky.SetColor("_SkyColor2", dayColorOverTime.Evaluate(dayLightTime));
            }

            _dayFogOverTime = Color.Lerp(dayFogOverTime.Evaluate(dayLightTime), dayColorOverTime.Evaluate(dayLightTime), fogToDayColor);
        }
        else
        {
            if (!useOwnShader)
            {
                sky.SetColor("_SkyColor1", (nightTopColorOverTime.Evaluate(nightLightTime)));
                sky.SetColor("_SkyColor2", nightColorOverTime.Evaluate(nightLightTime));
            }

            _dayFogOverTime = Color.Lerp(dayFogOverTime.Evaluate(dayLightTime), nightColorOverTime.Evaluate(nightLightTime), fogToDayColor);
        }


        RenderSettings.fogColor = _dayFogOverTime;
       // RenderSettings.ambientLight = _dayFogOverTime/3;
        if (!useOwnShader)
        {
            sky.SetColor("_SkyColor3", _dayFogOverTime);

            //nightsky
            sky.SetFloat("_NightOpacity", smoothFadeNight);
            sky.SetFloat("_SunScale", sunSizeOverTime.Evaluate(dayLightTime));
        }

    }
    public void SmoothNight()
    {
        //smooth the day night lerp
        if (nightLightTime >= 0.95f)
        {
            fadeNight = Mathf.Lerp(1, 0, (nightLightTime - 0.95f) / 0.05f);
        }
        else if (nightLightTime <= 0.05f)
        {
            fadeNight = Mathf.Lerp(0, 1, nightLightTime / 0.05f);
        }
        else
        {
            fadeNight = 1;
        }

        //smooth the day night lerp
        if (nightLightTime >= 0.6f  )
        {
            smoothFadeNight = Mathf.Lerp(1, 0,( nightLightTime-0.6f)/0.4f) ;
        }
        else if (nightLightTime <= 0.4f)
        {
            smoothFadeNight =  Mathf.Lerp(0,1, nightLightTime / 0.4f);
        }
        else
        {
            smoothFadeNight = 1;
        }

    }
}
