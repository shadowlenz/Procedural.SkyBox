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

    public Light sunLight;
    public Light moonLight;
    public GameObject moonLightGo;

    [Range(0,1)]
    public float smoothLerp =0.5f;
    public float GetSmoothLerp { get { if (Application.isPlaying || smoothLerp <= 0) return smoothLerp *Time.deltaTime; else return 1; } }

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
        public Gradient dayMidColorOverTime;

        [Space()]
        public Gradient nightTopColorOverTime;
        public Gradient nightMidColorOverTime;
 
        [Space()]
        public Gradient dayFogOverTime;
        [Range(0, 1)]
        public float fogToDayColor;

    }
    public SkyData skyData = new SkyData();


    [Header("debug")]
    [Range (0,1)]
	public float dayLightTime;
	[Range (0,1)]
	public float nightLightTime;
    [Space]
    [Range(0, 1)]
    public float timeOfDay = 0;

    //[HideInInspector()]
    [Range(0, 1)]
    public float fadeToNight;
   // [HideInInspector()]
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
    Material ClonedMat;
    Material prevSourceMat;
    public void Setup()
    {
        RenderSettings.sun = sunLight;
        if (moonLightGo == null) moonLightGo.transform.Rotate(180, 0, 0);

        print("changed skymat");
        if (Application.isPlaying)
        {
            if (skyData.skyBoxMat != prevSourceMat)
            {
                ClonedMat = Instantiate(skyData.skyBoxMat);//new Material(skyData.skyBoxMat);
                prevSourceMat = skyData.skyBoxMat;
            }
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
        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * (skyData.speed / 360);
        }
        //repeat
        if (timeOfDay > 1) timeOfDay = timeOfDay - 1;
        else if (timeOfDay < 0) timeOfDay = timeOfDay + 1;

        //day night cal split
        if (timeOfDay < 0.5f)
        {
            dayLightTime = timeOfDay * 2;

            // cause both dayLightTime & nightLightTime are at 0
            if (dayLightTime == 0) nightLightTime = 1;
            else nightLightTime = 0;
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
        sunLight.transform.rotation =Quaternion.Slerp(sunLight.transform.rotation, Quaternion.Euler(SunRot), GetSmoothLerp);

        //visuals
        if (active_DayNight)
        {
            float _fadeIntensity = Mathf.Lerp(0, 1, fadeToNight);
            float _fadeIntensity_invert = Mathf.Lerp(1, 0, fadeToNight);

            //fade sun light
            bool AtDay = (fadeToNight < 1);
            sunLight.enabled = AtDay;

            float L_SunLightIntensity = Mathf.Lerp(sunLight.intensity, skyData.lightIntensity, GetSmoothLerp) * _fadeIntensity_invert;
            sunLight.intensity = L_SunLightIntensity;

            //fade moon light
            bool AtNight = (fadeToNight > 1);
            moonLight.enabled = AtNight;

            float L_MoonLightIntensity = Mathf.Lerp(moonLight.intensity, skyData.moonLightIntensity, GetSmoothLerp) * _fadeIntensity;
            moonLight.intensity = L_MoonLightIntensity;
            //

            SkyColor();
            moonLightGo.SetActive(true);
        }
        else
        {
            sunLight.enabled = false;
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

        sunLight.color = Color.Lerp(sunLight.color, skyData.dayMidColorOverTime.Evaluate(dayLightTime), GetSmoothLerp);
        moonLight.color = Color.Lerp(moonLight.color, skyData.nightMidColorOverTime.Evaluate(nightLightTime), GetSmoothLerp);
        Color _dayFogOverTime;

        if (dayLightTime > 0)
        {
            //_topColor
            if (RenderSettings.skybox.HasProperty(_topColor))
            {
                Color L_topColor = Color.Lerp(RenderSettings.skybox.GetColor(_topColor), (skyData.dayTopColorOverTime.Evaluate(dayLightTime)), GetSmoothLerp);
                RenderSettings.skybox.SetColor(_topColor, L_topColor);
            }
            //_midColor
            if (RenderSettings.skybox.HasProperty(_midColor))
            {
                Color L_midColor = Color.Lerp(RenderSettings.skybox.GetColor(_midColor), (skyData.dayMidColorOverTime.Evaluate(dayLightTime)), GetSmoothLerp);
                RenderSettings.skybox.SetColor(_midColor, L_midColor);
            }

            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.dayMidColorOverTime.Evaluate(dayLightTime), skyData.fogToDayColor);
        }
        else
        {
            //_topColor
            if (RenderSettings.skybox.HasProperty(_topColor))
            {
                Color L_topColor = Color.Lerp(RenderSettings.skybox.GetColor(_topColor), (skyData.nightTopColorOverTime.Evaluate(nightLightTime)), GetSmoothLerp);
                RenderSettings.skybox.SetColor(_topColor, L_topColor);
            }
            //_midColor
            if (RenderSettings.skybox.HasProperty(_midColor))
            {
                Color L_midColor = Color.Lerp(RenderSettings.skybox.GetColor(_midColor), (skyData.nightMidColorOverTime.Evaluate(nightLightTime)), GetSmoothLerp);
                RenderSettings.skybox.SetColor(_midColor, L_midColor);
            }
            _dayFogOverTime = Color.Lerp(skyData.dayFogOverTime.Evaluate(dayLightTime), skyData.nightMidColorOverTime.Evaluate(nightLightTime), skyData.fogToDayColor);
        }

        if (RenderSettings.skybox.HasProperty(_bottomColor))
        {
            //_bottomColor
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _dayFogOverTime, GetSmoothLerp);
            RenderSettings.skybox.SetColor(_bottomColor, RenderSettings.fogColor);
        }
        //nightsky
        if (RenderSettings.skybox.HasProperty(_nightOpacity))
        {
            float L_nightOpacity = Mathf.Lerp(RenderSettings.skybox.GetFloat(_nightOpacity), smoothFadeNight, GetSmoothLerp);
            RenderSettings.skybox.SetFloat(_nightOpacity, L_nightOpacity);
        }
        if (RenderSettings.skybox.HasProperty(_sunScale))
        {
            //sunscale
            float L_sunScale = Mathf.Lerp(RenderSettings.skybox.GetFloat(_sunScale), skyData.sunSizeOverTime.Evaluate(timeOfDay), GetSmoothLerp);
            RenderSettings.skybox.SetFloat(_sunScale, L_sunScale);
        }

    }
    public void SmoothNight()
    {
        //smooth the day night lerp
        if (nightLightTime >= 0.95f)
        {
            fadeToNight = Mathf.Lerp(1, 0, (nightLightTime - 0.95f) / 0.05f);
        }
        else if (nightLightTime <= 0.05f)
        {
            fadeToNight = Mathf.Lerp(0, 1, nightLightTime / 0.05f);
        }
        else
        {
            fadeToNight = 1;
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
