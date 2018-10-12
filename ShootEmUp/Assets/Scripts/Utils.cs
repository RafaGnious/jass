using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    ///<summary>
    ///Returns the Transform's higher Parent in the hierarchy
    ///</summary> 
    public static Transform GetMaxParent(this Transform transform)
    {
        Transform returning = transform;
        while (returning.parent != null) returning = returning.parent;
        return returning;
    }
}
