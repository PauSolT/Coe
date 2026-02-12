using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Log 
{
    public static bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Custom logger
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="context">The owner who is logging this message. Used so that unity links it with the object</param>
    /// <param name="filePath">Filled by C# to know which class called the logger</param>
    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    public static void Info(string message,
        Object context = null,
        [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        string formattedMessage = $"[INFO][{className}]: {message}";
        UnityEngine.Debug.Log(formattedMessage, context);
    }
}
