using UnityEngine;

/// <summary>
/// Interface to make something targetable
/// </summary>
public interface ITargetable
{
    bool IsTargetable { get; }
    GameObject GetGameObject();
}
