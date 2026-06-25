using UnityEngine;

/// <summary>
/// Class to mark the root of the ability
/// </summary>
public class AbilityPrefabRoot : MonoBehaviour
{
    /// <summary>
    /// Destroys the root of the ability
    /// </summary>
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
