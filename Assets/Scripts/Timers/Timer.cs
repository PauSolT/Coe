using System;
using UnityEngine;

[Serializable]
public class Timer 
{
    public float TotalTime { get; }
    public float CurrentTime { get; set; }
    public float TickFrequency { get; private set; }

    private float nextTickTime;
    public Action OnTimerEnd;
    public Action OnTick;

    public bool IsCancelled { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsFinished => CurrentTime <= 0;

    public Timer(float totalTime, Action onTimerEnd, float tickFrequency = 0f, Action onTick = null)
    {
        TotalTime = totalTime;
        CurrentTime = totalTime;
        OnTimerEnd = onTimerEnd;
        OnTick = onTick;
        OnTimerEnd = onTimerEnd;
        TickFrequency = tickFrequency;

        //If this is a tick timer, calculate next tick time
        if (TickFrequency > 0)
            nextTickTime = totalTime - tickFrequency;
    }

    /// <summary>
    /// Logic that updates the timer. Works with normal timers and timers that have ticks
    /// </summary>
    /// <param name="deltaTime">The delta time of the game</param>
    public void UpdateTimer(float deltaTime)
    {
        CurrentTime -= deltaTime;

        if (TickFrequency > 0 && OnTick != null)
        {
            if (CurrentTime <= nextTickTime)
            {
                OnTick?.Invoke();
                nextTickTime -= TickFrequency;
            }
        }
    }

    public void PauseTimer() { IsPaused = true; }
    public void ResumeTimer() { IsPaused = false; }

    public void Cancel() { IsCancelled = true; }

    /// <summary>
    /// Returns the progress of the timer from 1 to 0
    /// </summary>
    /// Works like this because timer goes from X to 0
    public float ProgressDescending
    {
        get { return TotalTime > 0 ? Mathf.Clamp01(CurrentTime / TotalTime) : 0f; }
    }

    /// <summary>
    /// Returns the progress of the timer from 1 to 0
    /// </summary>
    /// Works like this because timer goes from X to 0
    public float ProgressAscending
    {
        get { return TotalTime > 0 ? Mathf.Clamp01(1f - (CurrentTime / TotalTime)) : 1f; }
    }

    /// <summary>
    /// Returns the exact time that has passed
    /// </summary>
    /// Works like this because timer goes from X to 0
    public float ElapsedTime
    {
        get { return TotalTime - CurrentTime; }
    }
}
