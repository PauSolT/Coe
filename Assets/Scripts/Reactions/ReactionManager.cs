using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance { get; private set; }

    [SerializeField] private ReactionDatabase reactionDatabase;
    public ReactionDatabase Database => reactionDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        } 
    }
}