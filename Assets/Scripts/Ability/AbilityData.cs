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
    [Tooltip("Power of the ability. The scale is from 0(0%) to 1(100%)")]
    public float power;
    [Tooltip("Cooldown of the ability in seconds")]
    public float cooldown;
    [Tooltip("Duration of the ability in seconds")]
    public float duration;
    [Tooltip("What to instantiate when executing the ability")]
    public GameObject prefab;

    [Header("DoT")]
    [Tooltip("Frequency of ticks")]
    public float tickFrequency;

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
