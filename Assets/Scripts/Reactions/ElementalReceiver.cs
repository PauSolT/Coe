using System.Collections.Generic;
using UnityEngine;

public class ElementalReceiver : MonoBehaviour
{
    [SerializeField] private ReactionDatabase reactionDatabase;

    // Tracks the elements currently active on this entity
    private List<ActiveElement> activeElements = new List<ActiveElement>();

    private class ActiveElement
    {
        public ElementData element;
        public Timer effectTimer;

        public ActiveElement(ElementData element, Timer effectTimer)
        {
            this.element = element;
            this.effectTimer = effectTimer;
        }
    }

    public void ApplyElement(ElementData incomingElement)
    {
        // Check if incoming element reacts with any currently active elements
        for (int i = activeElements.Count - 1; i >= 0; i--)
        {
            ElementData existingElement = activeElements[i].element;

            if (existingElement == incomingElement)
            {
                continue;
            }

            ElementalReaction reaction = reactionDatabase.GetReaction(existingElement, incomingElement);

            if (reaction != null)
            {
                reaction.TriggerReaction(gameObject);

                // Clean up the consumed element
                activeElements[i].effectTimer.Cancel();
                activeElements.RemoveAt(i);

                return;
            }
        }

        // If no reaction occurs, apply the element as a temporary status effect
        // Check if the element is already applied to refresh its duration
        ActiveElement existing = activeElements.Find(e => e.element == incomingElement);
        if (existing != null)
        {
            existing.effectTimer.Cancel();
            existing.effectTimer = DecayElement(incomingElement);
        }
        else
        {
            Timer decay = DecayElement(incomingElement);
            activeElements.Add(new ActiveElement(incomingElement, decay));
            Log.Info($"{incomingElement.elementName} applied to {gameObject.name}", LogCategory.Reactions);
        }
    }

    private Timer DecayElement(ElementData element)
    {
        return new Timer(element.effectDuration, () =>
        {
            activeElements.RemoveAll(e => e.element == element);
            Log.Info($"{element.elementName} faded from {gameObject.name}", LogCategory.Reactions);
        });
    }
}