using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using static LogSettings;

public static class Log 
{
    public static bool IsEnabled { get; set; } = true;
    public static LogCategory ActiveCategories { get; set; } = LogCategory.All;

    private static readonly Dictionary<LogCategory, string> categoryColors = new();

    public static void ClearColors()
    {
        categoryColors.Clear();
    }

    private static string GetColorHex(LogCategory category)
    {
        if (categoryColors.TryGetValue(category, out string hex))
        {
            return hex;
        }
        return "#FFFFFF";
    }

    /// <summary>
    /// Allows overriding colors from the inspector
    /// </summary>
    public static void SetColor(LogCategory category, Color color)
    {
        string hex = "#" + ColorUtility.ToHtmlStringRGB(color);
        categoryColors[category] = hex;
    }

    /// <summary>
    /// Custom logger
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="category">TThe category which this log belongs to</param>
    /// <param name="context">The owner who is logging this message. Used so that unity links it with the object</param>
    /// <param name="filePath">Filled by C# to know which class called the logger</param>
    [HideInCallstack] // Makes it so Unity skips the Log file, and when the log is double clicked, it goes to the called file
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Info(string message,
        LogCategory category = LogCategory.Default,
        Object context = null,
        [CallerFilePath] string filePath = "")
    {
        if (!IsEnabled) return;

        // Checks if the specific category bit is set in ActiveCategories
        if ((ActiveCategories & category) == 0)
        {
            return;
        }

        string className = Path.GetFileNameWithoutExtension(filePath);
        string colorHex = GetColorHex(category);

        string formattedMessage = $"<color={colorHex}><b>[INFO][{category}]</b></color> [{className}]: {message}";
        UnityEngine.Debug.Log(formattedMessage, context);
    }
}
