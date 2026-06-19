using System;
using UnityEngine;

[Serializable]
public class ShieldComponent
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public float Amount { get; set; }
    public Timer ShieldTimer { get; set; }
    public Action OnDestroyedOrExpired { get; private set; }

    public ShieldComponent(string id, float amount, Action onDestroyedOrExpired)
    {
        Id = id;
        Amount = amount;
        OnDestroyedOrExpired = onDestroyedOrExpired;
    }
}