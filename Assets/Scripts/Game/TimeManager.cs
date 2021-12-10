using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    bool _isPlaying = true;
    bool isRunning = false;

    public static bool isPlaying { get => instance._isPlaying; private set => instance._isPlaying = value; }

    float toTimeScale = 1f;
    float shadeDuration = 0f;

    float atPauseTimeScale = 0f;

    private static TimeManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this;
    }

    void Update()
    {
        if (isPlaying)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, toTimeScale, (1f / shadeDuration) * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }

    public static void Pause()
    {
        isPlaying = false;
        instance.atPauseTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public static void Play()
    {
        isPlaying = true;
        Time.timeScale = instance.atPauseTimeScale;
    }

    public static void SlowDown(float scale, float duration)
    {
        if (!instance.isRunning) instance.StartCoroutine(instance._SlowDown(scale, duration, 0f, 0f));
    }

    public static void SlowDown(float scale, float duration, float preShading)
    {
        if (!instance.isRunning) instance.StartCoroutine(instance._SlowDown(scale, duration, preShading, 0f));
    }
    

    public static void SlowDown(float scale, float duration, float preShading, float postShading)
    {
        if (!instance.isRunning) instance.StartCoroutine(instance._SlowDown(scale, duration, preShading, postShading));
    }
    
    public IEnumerator _SlowDown(float scale, float duration, float preShading, float postShading)
    {
        isRunning = true;

        toTimeScale = scale;
        shadeDuration = preShading;

        float wait = preShading;
        float waited = 0f;

        while (waited < wait)
        {
            if (instance._isPlaying) waited += Time.unscaledDeltaTime;
            yield return null;
        }

        shadeDuration = 0f;

        wait = duration;
        waited = 0f;
        while (waited < wait)
        {
            if (instance._isPlaying) waited += Time.unscaledDeltaTime;
            yield return null;
        }

        toTimeScale = 1f;
        shadeDuration = postShading;

        wait = postShading;
        waited = 0f;

        while (waited < wait)
        {
            if (instance._isPlaying) waited += Time.unscaledDeltaTime;
            yield return null;
        }

        shadeDuration = 0f;

        isRunning = false;
    }
}
