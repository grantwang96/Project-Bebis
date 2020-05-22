using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtraMath
{
    public static float Lerp(float current, float target, float rate) {
        float value = Mathf.Abs(current);
        float absTarget = Mathf.Abs(target);
        float absRate = Mathf.Abs(rate);
        if(Mathf.Approximately(current, target) || (value - absTarget < absRate)) {
            return target;
        }
        value -= absRate;
        value = current >= target ? value : -value;
        return value;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    
    public static float RoundValueClampOne(float value) {
        float returnVal = Mathf.Abs(value) >= 0.5f ? 1f : 0f;
        if (value < 0f) {
            returnVal *= -1;
        }
        return returnVal;
    }

    public static float Magnitude(float value, float magnitude) {
        float returnVal = magnitude;
        if(value < 0f) {
            returnVal *= -1;
        }
        return returnVal;
    }
}
