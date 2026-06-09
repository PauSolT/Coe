using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


//Flags makes it so that Unity's Inspector treats this enum
//as a colllection of individual on/off switches
[Flags]
public enum LogCategory 
{ 
    None = 0,
    Default = 1 << 0,
    System = 1 << 1,
    Movement = 1 << 2,
    Health = 1 << 3,
    Elements = 1 << 4,
    Input = 1 << 5,
    All = ~0
}

[CreateAssetMenu(fileName = "LogSettings", menuName = "Logging/Log Settings")]
public class LogSettings : ScriptableObject
{
    [Serializable]
    public struct CategoryColor
    {
        public LogCategory category;
        public Color color;
    }

    [SerializeField] private bool isEnabled = true;
    [SerializeField] private LogCategory activeCategories = LogCategory.All;

    [Header("Category Colors")]
    [SerializeField] private List<CategoryColor> categoryColors = new();

    public void Apply()
    {
        Log.IsEnabled = isEnabled;
        Log.ActiveCategories = activeCategories;

        Log.ClearColors();
        foreach (CategoryColor item in categoryColors)
        {
            Log.SetColor(item.category, item.color);
        }
    }

    private void OnValidate()
    {
        SyncCategoryColorsList();
        Apply();
    }

    /// <summary>
    /// Checks all of the enum categories, and updates them with their colors
    /// </summary>
    private void SyncCategoryColorsList()
    {
        if (categoryColors == null)
        {
            categoryColors = new List<CategoryColor>();
        }

        // Get all defined enum values
        Array enumValues = Enum.GetValues(typeof(LogCategory));
        HashSet<LogCategory> existingCategories = new();

        // Clean up the list: remove any None, All, or invalid combined flags
        for (int i = categoryColors.Count - 1; i >= 0; i--)
        {
            LogCategory cat = categoryColors[i].category;
            if (cat == LogCategory.None || cat == LogCategory.All || !IsSingleFlag(cat))
            {
                categoryColors.RemoveAt(i);
                continue;
            }
            existingCategories.Add(cat);
        }

        // Add any new enum values that aren't already in our list
        foreach (LogCategory val in enumValues)
        {
            if (val == LogCategory.None || val == LogCategory.All) continue;
            if (!IsSingleFlag(val)) continue;

            if (!existingCategories.Contains(val))
            {
                categoryColors.Add(new CategoryColor
                {
                    category = val,
                    color = Color.white
                });
            }
        }
    }

    /// <summary>
    /// Helper to determine if an enum value represents a single bit (power of two)
    /// </summary>
    /// <param name="val">Value of log category</param>
    /// <returns>True if the value of that category is a power of 2</returns>
    private bool IsSingleFlag(LogCategory val)
    {
        int intVal = (int)val;
        return intVal > 0 && (intVal & (intVal - 1)) == 0;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        LogSettings settings = Resources.Load<LogSettings>("LogSettings");
        if (settings != null)
        {
            settings.Apply();
        }
        else
        {
            UnityEngine.Debug.LogWarning("[Log] No 'LogSettings' asset found in a 'Resources' folder. Using default log settings.");
        }
    }
}