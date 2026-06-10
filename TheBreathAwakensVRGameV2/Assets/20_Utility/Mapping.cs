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

    public static float MapValue(float currentValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        if (oldMax == oldMin)
            throw new ArgumentException("oldMax and oldMin cannot be the same.");

        float mappedValue = newMin + ((currentValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin);

        if (mappedValue >= newMax)
        {
            mappedValue = newMax;
        }

        if (mappedValue <= newMin)
        {
            mappedValue = newMin;
        }

        return mappedValue;
    }
}