using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool IsPlaying { get; private set; }

    private bool _isRunning = false;
    private float _toTimeScale = 1f;
    private float _shadeDuration = 0f;

    private float _atPauseTimeScale = 0f;

    private static TimeManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);

        _instance = this;
    }

    void Update()
    {
        if (IsPlaying)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, _toTimeScale, (1f / _shadeDuration) * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }

    public static void Pause()
    {
        IsPlaying = false;
        _instance._atPauseTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public static void Play()
    {
        IsPlaying = true;
        Time.timeScale = _instance._atPauseTimeScale;
    }

    public static void SlowDown(float scale, float duration)
    {
        if (!_instance._isRunning)
            _instance.StartCoroutine(_instance._SlowDown(scale, duration, 0f, 0f));
    }

    public static void SlowDown(float scale, float duration, float preShading)
    {
        if (!_instance._isRunning)
            _instance.StartCoroutine(_instance._SlowDown(scale, duration, preShading, 0f));
    }

    public static void SlowDown(float scale, float duration, float preShading, float postShading)
    {
        if (!_instance._isRunning)
            _instance.StartCoroutine(_instance._SlowDown(scale, duration, preShading, postShading));
    }

    public IEnumerator _SlowDown(float scale, float duration, float preShading, float postShading)
    {
        _isRunning = true;

        _toTimeScale = scale;
        _shadeDuration = preShading;

        float wait = preShading;
        float waited = 0f;

        while (waited < wait)
        {
            if (IsPlaying)
                waited += Time.unscaledDeltaTime;

            yield return null;
        }

        _shadeDuration = 0f;

        wait = duration;
        waited = 0f;
        while (waited < wait)
        {
            if (IsPlaying)
                waited += Time.unscaledDeltaTime;

            yield return null;
        }

        _toTimeScale = 1f;
        _shadeDuration = postShading;

        wait = postShading;
        waited = 0f;

        while (waited < wait)
        {
            if (IsPlaying)
                waited += Time.unscaledDeltaTime;

            yield return null;
        }

        _shadeDuration = 0f;

        _isRunning = false;
    }
}
