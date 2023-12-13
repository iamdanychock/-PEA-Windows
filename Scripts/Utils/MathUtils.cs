using Godot;
using System;

namespace Com.IsartDigital.WindowWar.Utils
{

    public partial class MathUtils
    {
        public static float Lxrp(float startValue, float endValue, float t)
        {
            t = (float)Mathf.Clamp(t, 0.0, 1.0);
            return Mathf.Lerp(startValue, endValue, t * t);
        }
    }

}
