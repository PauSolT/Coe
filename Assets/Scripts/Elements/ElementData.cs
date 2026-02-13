using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ElementData), menuName = "SO/" + nameof(ElementData), order = 2)]
public class ElementData : ScriptableObject
{
    public string elementName;
    public string description;
    public List<AbilityData> abilityData;

}
