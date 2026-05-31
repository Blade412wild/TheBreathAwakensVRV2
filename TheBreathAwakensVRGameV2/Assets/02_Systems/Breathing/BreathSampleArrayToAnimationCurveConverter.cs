using System.Collections.Generic;
using UnityEngine;

public static class BreathSampleArrayToAnimationCurveConverter
{
    public static AnimationCurve ConvertSampleToAnimationCurve(List<BreathCycle> breathingCycles)
    {
        AnimationCurve animationCurve = new AnimationCurve();

        int maxKeyFrames = GetMaxPossibleKeyframes(breathingCycles);
        Keyframe[] keyframes = new Keyframe[maxKeyFrames];

        int counter = 0;

        foreach (BreathCycle cycle in breathingCycles)
        {

            for (int i = 0; i < cycle.Points.Count; i++)
            {
                SensorDataPoint point = cycle.Points[i];

                if (point.State == BreathingState.exhaling)
                {
                    point.Value *= -1;
                }

                keyframes[counter] = SensorPointToKeyFrame(point);
                counter++;
            }
        }

        animationCurve.keys = keyframes;

        return animationCurve;


    }

    private static Keyframe SensorPointToKeyFrame(SensorDataPoint dataPoint)
    {
        Keyframe keyframe = new Keyframe();
        keyframe.value = dataPoint.Value;
        keyframe.time = dataPoint.Time;
        return keyframe;
    }

    private static int GetMaxPossibleKeyframes(List<BreathCycle> breathingCycles)
    {
        int maxPossibleKeyframes = 0;

        foreach (var cycle in breathingCycles)
        {
            maxPossibleKeyframes += cycle.Points.Count;
        }

        return maxPossibleKeyframes;
    }

}
