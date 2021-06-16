using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSceneEnabler : MonoBehaviour
{
    public DayNightCycle dayNightCycleMaster;
    [Space()]
    public bool activeDayNight=true;

    public bool canOverrideTime = false;
    [Range(0,1)]public float overrideTime;
    [Space()]
    public DayNightCycle.SkyData SkyData = new DayNightCycle.SkyData();
    private void OnEnable()
    {
        StartCoroutine(DelayEnable());
    }

    IEnumerator DelayEnable()
    {
        yield return new WaitForEndOfFrame();

        if (DayNightCycle.instance == null)
        {
            Debug.Log("no skybox exsist. Spawns one");
            Instantiate(dayNightCycleMaster);
        }
        yield return new WaitUntil(() => DayNightCycle.instance != null);

        if (activeDayNight)
        {
            if (canOverrideTime) DayNightCycle.instance.ChangeTimeTo(overrideTime);

            DayNightCycle.instance.skyData = SkyData;
            DayNightCycle.instance.Setup();
        }

        DayNightCycle.instance.active_DayNight = activeDayNight;
    }
}
