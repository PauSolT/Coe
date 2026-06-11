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

            if (timer.IsCancelled)
            {
                timers.Remove(timer);
                continue;
            }

            if (timer.IsPaused)
            {
                continue;
            }

            timer.UpdateTimer(Time.deltaTime);

            if (timer.IsFinished )
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
    /// <returns>Returns the timer.</returns>
    public Timer StartTimer(float duration, Action onEnd)
    {
        Timer timer = new Timer(duration, onEnd);
        timers.Add(timer);
        return timer;
    }

    /// <summary>
    /// Create a new tick timer
    /// </summary>
    /// <param name="duration">Duration of the timer.</param>
    /// <param name="tickFrequency">Frequency of the ticks</param>
    /// <param name="onTick">Action that executes every tick</param>
    /// <param name="onEnd">Action that executes when the timer ends.</param>
    /// <returns>Returns the timer.</returns>
    public Timer StartTickTimer(float duration, float tickFrequency, Action onTick, Action onEnd = null)
    {
        Timer timer = new Timer(duration, onEnd, tickFrequency, onTick);
        timers.Add(timer);
        return timer;
    }
}
