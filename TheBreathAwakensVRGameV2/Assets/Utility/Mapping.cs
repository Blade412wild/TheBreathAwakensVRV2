using System;

public static class Mapping
{
    public static float MapValueClamped(float currentValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        if (oldMax == oldMin)
            throw new ArgumentException("oldMax and oldMin cannot be the same.");

        float t = (currentValue - oldMin) / (oldMax - oldMin);

        t = Math.Clamp(t, 0f, 1f);

        return newMin + t * (newMax - newMin);
    }
}