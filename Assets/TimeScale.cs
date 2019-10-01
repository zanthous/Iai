using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeScale : MonoBehaviour
{
    [SerializeField] private float _TimeScale;
    [SerializeField] private PostProcessProfile normal;
    [SerializeField] private PostProcessProfile grayscale;
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private AnimationCurve timeScaleCurve;

    public static System.Action timeSlow;

    private ColorGrading colorGrading;
    private bool slow = false;

    private void Awake()
    {
        timeSlow += DoTimeSlowAndGrayscale;
    }
    private void OnDestroy()
    {
        timeSlow -= DoTimeSlowAndGrayscale;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Time.timeScale = _TimeScale * Settings.GlobalTimeScale;
    //    Time.fixedDeltaTime = _TimeScale * 0.02f;
    //}

    private void Start()
    {
        Debug.Log("Timescale start");
        Time.timeScale = 1.0f * Settings.GlobalTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void DoTimeSlowAndGrayscale()
    {
        slow = !slow;
        if(slow)
        {
            volume.profile = grayscale;
            StartCoroutine(FakeAnim());
        }
        else
        {
            volume.profile = normal;
            GameManager.EndLevel.Invoke();
        }
        //volume.profile.isDirty = true;
    }

    private IEnumerator FakeAnim()
    {
        //Method 1: time slow and apply force and then show slice
        if(true)
        { 
            Time.timeScale = .006f * Settings.GlobalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            //Wait for 1 second 
            float timer = 0.0f;
            while(timer < 1.0f)
            {
                timer += Time.unscaledDeltaTime;
                yield return 0;
            }
            timer = 0.0f;
            while(timer < 0.5f)
            {
                Time.timeScale = timeScaleCurve.Evaluate(timer) * Settings.GlobalTimeScale;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                timer += Time.unscaledDeltaTime;
                yield return 0;
            }
            Time.timeScale = 1.0f * Settings.GlobalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            timeSlow.Invoke();
        }
        else
        {
            //method 2: time freeze show slice then apply force
            Time.timeScale = 0.0f;
            //Wait for 1 second 
            float timer = 0.0f;
            while(timer < 1.0f)
            {
                timer += Time.unscaledDeltaTime;
                yield return 0;
            }
            Time.timeScale = 1.0f * Settings.GlobalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            timeSlow.Invoke();
        }
    }
}
