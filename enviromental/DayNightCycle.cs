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

        print("changed skymat");
        if (Application.isPlaying)
        {
            Material ClonedMat = Instantiate(skyData.skyBoxMat);//new Material(skyData.skyBoxMat);
            RenderSettings.skybox = ClonedMat;
        }
        else
        { 
            RenderSettings.skybox = skyData.skyBoxMat; 
        }

        DynamicGI.UpdateEnvironment();
    }

    public void ChangeTimeTo(float range)
    {
        timeOfDay = range;
    }

    // Update is called once per frame
    public void Update () {
        if (Application.isPlaying && DebugGame.instance != null && DebugGame.instance.inDebug) return;

        UpdateSky();
    }
    public void TimeOfDayBar()
    {
        if (timeOfDay >= 1) timeOfDay = timeOfDay - 1;
        else if (timeOfDay <= 0) timeOfDay = timeOfDay + 1;
        if (Application.isPlaying) timeOfDay += Time.deltaTime * (skyData.speed / 360);

        //day night cal split
        if (timeOfDay < 0.5f)
        {
            dayLightTime = timeOfDay * 2;
            nightLightTime = 0;
        }
        else
        {
            nightLightTime = (timeOfDay * 2) - 1;
            dayLightTime = 0;
        }
    }

    Vector3 SunRot = Vector3.zero;
    public void UpdateSky()
    {
        TimeOfDayBar();

        //rotate transform
        SunRot.x = timeOfDay * 360;
        light.transform.localEulerAngles = SunRot;

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


    [Header("mat properties")]
    public string _topColor = "_SkyColor1";
    public string _midColor = "_SkyColor2";
    public string _bottomColor = "_SkyColor3";
    public string _nightOpacity = "_NightOpacity";
    public string _sunScale = "_SunScale";
    public void SkyColor()
    {
        SmoothNight();
        //change direcrtional color on light over time
        light.color = skyData.dayColorOverTime.Evaluate(dayLightTime);
        moonLight.color = skyData.nightColorOverTime.Evaluate(nightLightTime);

        if (dayLightTime > 0)
        {
            //_topColor
            if (RenderSettings.skybox.HasProperty(_topColor)) RenderSettings.skybox.SetColor(_topColor, (skyData.dayTopColorOverTime.Evaluate(dayLightTime)));
            //_midColor
            if (RenderSettings.skybox.HasProperty(_midColor)) RenderSettings.skybox.SetColor(_midColor, skyData.dayColorOverTime.Evaluate(dayLightTime));


            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.dayColorOverTime.Evaluate(dayLightTime), skyData.fogToDayColor);
        }
        else
        {
            //_topColor
            if (RenderSettings.skybox.HasProperty(_topColor)) RenderSettings.skybox.SetColor(_topColor, (skyData.nightTopColorOverTime.Evaluate(nightLightTime)));
            //_midColor
            if (RenderSettings.skybox.HasProperty(_midColor)) RenderSettings.skybox.SetColor(_midColor, skyData.nightColorOverTime.Evaluate(nightLightTime));

            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.nightColorOverTime.Evaluate(nightLightTime), skyData.fogToDayColor);
        }


        RenderSettings.fogColor = _dayFogOverTime;
        //_bottomColor
        if (RenderSettings.skybox.HasProperty(_bottomColor)) RenderSettings.skybox.SetColor(_bottomColor, _dayFogOverTime);

        //nightsky
        if (RenderSettings.skybox.HasProperty(_nightOpacity)) RenderSettings.skybox.SetFloat(_nightOpacity, smoothFadeNight);
        if (RenderSettings.skybox.HasProperty(_sunScale)) RenderSettings.skybox.SetFloat(_sunScale, skyData.sunSizeOverTime.Evaluate(timeOfDay));

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
