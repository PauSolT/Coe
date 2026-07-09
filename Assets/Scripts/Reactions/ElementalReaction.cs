using UnityEngine;

[CreateAssetMenu(menuName = "SO/Reactions/Reaction")]
public class ElementalReaction : ScriptableObject
{
    public ElementData elementA;
    public ElementData elementB;
    public string reactionName;

    [Header("Effects")]
    public GameObject reactionVfxPrefab;
    public float damageMultiplier = 2.0f;

    /// <summary>
    /// Checks if the incoming element and current element match this reaction
    /// </summary>
    /// <param name="a">First element</param>
    /// <param name="b">Second element</param>
    /// <returns>True if those two elements have a reaction</returns>
    public bool Matches(ElementData a, ElementData b)
    {
        return (elementA == a && elementB == b) || (elementA == b && elementB == a);
    }

    /// <summary>
    /// Executes the logic of the reaction
    /// </summary>
    /// <param name="target">The target of the reaction</param>
    public virtual void TriggerReaction(GameObject target)
    {
        Log.Info($"Triggered {reactionName} on {target.name}!", LogCategory.Reactions);

        if (reactionVfxPrefab != null)
        {
            Instantiate(reactionVfxPrefab, target.transform.position, Quaternion.identity);
        }
    }
}