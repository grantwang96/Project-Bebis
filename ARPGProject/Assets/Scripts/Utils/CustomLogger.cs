using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomLogger
{
    public static void Log(string reportingClass, string message) {
        Debug.Log($"[{reportingClass}] {message}");
    }

    public static void Warn(string reportingClass, string message) {
        Debug.LogWarning($"[{reportingClass}] {message}");
    }

    public static void Error(string reportingClass, string message) {
        Debug.LogError($"[{reportingClass}] {message}");
    }
}
