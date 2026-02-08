using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    [field: SerializeField] List<Timer> timers = new();
    [field: SerializeField] public bool Paused { get; private set; } = false;


    void Update()
    {
        if (Paused) return;

        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];
            timer.CurrentTime -= Time.deltaTime;

            if (timer.CurrentTime < 0 )
            {
                timer.OnTimerEnd?.Invoke();
                timers.Remove(timer);
            }
        }
    }

    void Pause()
    {
        Paused = true;
    }

    void Unpause()
    {
        Paused = false;
    }
    /// <summary>
    /// Create a new timer.
    /// </summary>
    /// <param name="duration">Duration of the timer.</param>
    /// <param name="onEnd">Action that executes when the timer ends.</param>
    /// <returns>Returns the damage dealt to health.</returns>
    public void StartTimer(float duration, Action onEnd)
    {
        timers.Add(new Timer(duration, onEnd));
    }
}
