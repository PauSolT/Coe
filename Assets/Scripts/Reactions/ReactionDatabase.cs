using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Reactions/Reaction Database")]
public class ReactionDatabase : ScriptableObject
{
    public List<ElementalReaction> reactions;

    /// <summary>
    /// Gets the reaction for these 2 elements
    /// </summary>
    /// <param name="a">First element</param>
    /// <param name="b">Second element</param>
    /// <returns>Returns that elemental reaction</returns>
    public ElementalReaction GetReaction(ElementData a, ElementData b)
    {
        foreach (ElementalReaction reaction in reactions)
        {
            if (reaction.Matches(a, b))
            {
                return reaction;
            }
        }
        return null;
    }
}