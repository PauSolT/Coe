using System;

[Serializable]
public class ShieldComponent
{
    public string Id { get; private set; }
    public float Amount { get; set; }
    public Timer ShieldTimer { get; set; }

    public Action OnDestroyedOrExpired { get; private set; }

    public ShieldComponent(string id, float amount, Action onDestroyedOrExpired)
    {
        Id = id;
        Amount = amount;
        OnDestroyedOrExpired = onDestroyedOrExpired;
    }
}