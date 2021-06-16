//Written by: Eugene Chu
//Twitter @LenZ_Chu
//Free to use. Please mention or credit my name in projects if possible!

//drag this to a gameobject and click 'setup' once.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    public static DayNightCycle instance;

    public bool active_DayNight = true;
    public bool useOwnShader;

    Material skyBoxMat_Clone;

    public Light light;

    Color _dayFogOverTime;

    [System.Serializable]
    public struct SkyData
    {
        public Material skyBoxMat;
        public float lightIntensity;// = 1.3f;
        public float moonLightIntensity;// = 1;
        public float speed;//= 1f;
        [Space()]
        public AnimationCurve sunSizeOverTime;
        [Space()]
        public Gradient dayTopColorOverTime;
        public Gradient dayColorOverTime;
        [Space()]
        public Gradient nightTopColorOverTime;
        public Gradient nightColorOverTime;
        [Space()]
        public Gradient dayFogOverTime;
        [Range(0, 1)]
        public float fogToDayColor;

    }
    public SkyData skyData = new SkyData();

    [Header("debug")]
    public float degreesSkip = 45;

    [Range(0, 1)]
    public float timeOfDay = 0;
    [Range (0,1)]
	public float dayLightTime;
	[Range (0,1)]
	public float nightLightTime;

    public GameObject moonLightGo;
    public Light moonLight;

    [HideInInspector()]
    [Range(0, 1)]
    public float fadeNight;
    [HideInInspector()]
    [Range(0, 1)]
    public float smoothFadeNight;

    // Use this for initialization
    void Awake () {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Setup();
        }
        else
        {
            //already exist
            Destroy(this.gameObject);
            return;
        }
    }

    public void Setup()
    {
        RenderSettings.sun = light;
        if (moonLightGo == null) moonLightGo.transform.Rotate(180, 0, 0);

        SkyMatSetup();
    }
   void SkyMatSetup()
    {
        print("changed skymat");
        skyBoxMat_Clone =  new Material(skyData.skyBoxMat);
        RenderSettings.skybox = skyBoxMat_Clone;
        DynamicGI.UpdateEnvironment();
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
        light.transform.Rotate (new Vector3 ( (Time.deltaTime * skyData.speed), 0,0));
		//calculate debug
		Vector3 currentRot = GetYawPitch.GetPitchYawRollDeg (light.transform.rotation  );
		dayLightTime = Mathf.Clamp01 ( currentRot.x/180);
        float _nightLightTime = Mathf.Clamp01(-currentRot.x / 180);
        nightLightTime = Mathf.Lerp(1, 0, _nightLightTime);
        ///
        TimeOfDayBar();

        //visuals
        if (active_DayNight)
        {
            //shuts lights as it goes over the horizon
            if (dayLightTime <= 0 || dayLightTime >= 1)
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
                light.intensity = Mathf.Lerp(skyData.lightIntensity, 0, fadeNight);

                moonLight.enabled = true;
                moonLight.intensity = Mathf.Lerp(0, skyData.moonLightIntensity, fadeNight);
            }
            else
            {
                //night to day time=========================================== ☼

                if (fadeNight <= 0)
                {
                    moonLight.enabled = false;
                }
                else
                {
                    moonLight.enabled = true;
                }
                moonLight.intensity = Mathf.Lerp(0, skyData.moonLightIntensity, fadeNight);

                light.enabled = true;
                light.intensity = Mathf.Lerp(skyData.lightIntensity, 0, fadeNight);

            }

            SkyColor();

            moonLightGo.SetActive(true);
        }
        else
        {
            light.enabled = false;
            moonLight.enabled = false;
            moonLightGo.SetActive(false);
        }
        
    }


    public void Debug()
    {
        light.transform.Rotate(new Vector3(degreesSkip, 0, 0));
    }

    public void SkyColor()
    {
        SmoothNight();
        //change direcrtional color on light over time
        light.color = skyData.dayColorOverTime.Evaluate(dayLightTime);
        moonLight.color = skyData.nightColorOverTime.Evaluate(nightLightTime);

        //sky.SetColor("_SkyColor1", Color.Lerp(dayColorOverTime.Evaluate(dayLightTime), dayFogOverTime.Evaluate(dayLightTime), smoothFadeNight));
        if (dayLightTime > 0)
        {
            if (!useOwnShader)
            {
                RenderSettings.skybox.SetColor("_SkyColor1", (skyData.dayTopColorOverTime.Evaluate(dayLightTime)));
                RenderSettings.skybox.SetColor("_SkyColor2", skyData.dayColorOverTime.Evaluate(dayLightTime));
            }

            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.dayColorOverTime.Evaluate(dayLightTime), skyData.fogToDayColor);
        }
        else
        {
            if (!useOwnShader)
            {
                RenderSettings.skybox.SetColor("_SkyColor1", (skyData.nightTopColorOverTime.Evaluate(nightLightTime)));
                RenderSettings.skybox.SetColor("_SkyColor2", skyData.nightColorOverTime.Evaluate(nightLightTime));
            }

            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.nightColorOverTime.Evaluate(nightLightTime), skyData.fogToDayColor);
        }


        RenderSettings.fogColor = _dayFogOverTime;
       // RenderSettings.ambientLight = _dayFogOverTime/3;
        if (!useOwnShader)
        {
            RenderSettings.skybox.SetColor("_SkyColor3", _dayFogOverTime);

            //nightsky
            RenderSettings.skybox.SetFloat("_NightOpacity", smoothFadeNight);
            RenderSettings.skybox.SetFloat("_SunScale", skyData.sunSizeOverTime.Evaluate(dayLightTime));
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
