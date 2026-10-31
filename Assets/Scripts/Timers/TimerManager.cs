using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : BaseSingleton<TimerManager>
{

    [field: SerializeField] List<Timer> timers = new();
    [field: SerializeField] public bool Paused { get; private set; } = false;

    public event Action OnTimerUpdated;

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

            if (timer.IsFinished)
            {
                timer.OnTimerEnd?.Invoke();
                timers.Remove(timer);
            }
        }

        OnTimerUpdated?.Invoke();
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
    public Timer StartTimer(float duration, Action onEnd, Action<float> update = null)
    {
        Timer timer = new Timer(duration, onEnd, onUpdate: update);
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
    public Timer StartTickTimer(float duration, float tickFrequency, Action onTick, Action onEnd = null, Action<float> update = null)
    {
        Timer timer = new Timer(duration, onEnd, tickFrequency, onTick, update);
        timers.Add(timer);
        return timer;
    }
}
