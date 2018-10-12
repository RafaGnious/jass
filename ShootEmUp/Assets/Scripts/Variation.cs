using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type used to have variables variating in function of another variable
/// </summary>
[Serializable]
public struct Equation
{
    [Tooltip("The a in 'a(x+c)^p + b'")]
    public float Slope;

    [Tooltip("The b in 'a(x+c)^p + b'")]
    public float Incline;

    [Tooltip("The p in 'a(x+c)^p + b'")]
    public float Power;

    [Tooltip("The c in 'a(x+c)^p + b'")]
    public float XOffset;

    [Tooltip("Should the values be floored?")]
    public bool Floor;

    [Tooltip("If checked whenever value is negative it will return 0")]
    public bool PositiveOrZero;

    public float GetValue(float x)
    {
        
        float returning = Slope * Mathf.Pow(x + XOffset, Power) + Incline;
        if (Floor) returning =  Mathf.FloorToInt(returning);
        if (PositiveOrZero) return Mathf.Max(returning, 0f);
        return returning;
    }
    

}

