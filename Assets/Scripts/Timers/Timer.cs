using System;

[Serializable]
public class Timer 
{
    public float TotalTime { get; }
    public float CurrentTime { get; set; }
    public Action OnTimerEnd;

    public Timer(float totalTime, Action onTimerEnd)
    {
        TotalTime = totalTime;
        CurrentTime = totalTime;
        OnTimerEnd = onTimerEnd;
    }
}
