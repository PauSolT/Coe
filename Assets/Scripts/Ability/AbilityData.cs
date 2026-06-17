using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AbilityData), menuName = "SO/" + nameof(AbilityData), order = 1)]
public class AbilityData : ScriptableObject
{
    [Header("Common")]
    [Tooltip("Name of the ability")]
    public string abilityName;
    [Tooltip("Description of the ability")]
    public string description;
    [Tooltip("Power of the ability. The scale is from 1(1%) to 100(100%)")]
    public float power;
    [Tooltip("Secondary Power of the ability. The scale is from 1(1%) to 100(100%). For abilities that have two effects")]
    public float secondaryPower;
    [Tooltip("Cooldown of the ability in seconds")]
    public float cooldown;
    [Tooltip("Duration of the ability in seconds")]
    public float duration;
    [Tooltip("What to instantiate when executing the ability")]
    public GameObject prefab;

    [Header("DoT")]
    [Tooltip("Frequency of ticks for DoT damage in seconds")]
    public float tickFrequency;

    [Header("Empoered states")]
    [Tooltip("Empowered power of the ability. The scale is from 1(1%) to 1(100%). It adds to the original power")]
    public float empoweredPower;
    [Tooltip("Empowered secondary power of the ability. The scale is from 1(1%) to 1(100%). It adds to the original power")]
    public float empoweredSecondaryPower;
    [Tooltip("Empowered duration of the ability in seconds. It adds to the original seconds")]
    public float empoweredDuration;

    public override string ToString()
    {
        // :P0 converts a 0-1 float to a Percentage string with 0 decimal places (0.85 -> 85%)
        // :0.## ensures we don't show unnecessary zeros on the cooldown (2.50 -> 2.5)
        string desc = $"COMMON\n {abilityName} | Power: {power:P0} | CD: {cooldown:0.##}s | Duration: {duration:0.##}s | Prefab: {(prefab != null ? prefab.name : "null")}";
        desc += $"\n{description}";
        desc += $"\nDoT\n Frequency: {tickFrequency}";
        return desc;
    }
}
