using UnityEngine;
using System.Collections;
using System;

public class MyUIInspectorBlendingData : MonoBehaviour
{
    public MyUIBlendingData myUIBlendingData;
}

[System.Serializable]
public struct MyUIBlendingData
{
    public float floatValue;
    public bool useFloatBlending;
    public Vector3 vector3Value;
    public bool useVector3Blending;
    public Color colorValue;
    public bool useColorBlending;

    public static MyUIBlendingData BezierBlend(MyUIBlendingData from, MyUIBlendingData mid, MyUIBlendingData to, float t)
    {
        MyUIBlendingData result = new MyUIBlendingData();

        float t2 = t * t;
        float t3 = t2 * t;

        float a = (1 - 3 * t + 3 * t2 - t3);
        float b = (3 * t - 3 * t2);

        if (from.useFloatBlending || to.useFloatBlending)
        {
            result.floatValue = a * from.floatValue + b * mid.floatValue + t3 * to.floatValue;
            result.useFloatBlending = true;
        }

        if (from.useVector3Blending || to.useVector3Blending)
        {
            result.vector3Value = a * from.vector3Value + b * mid.vector3Value + t3 * to.vector3Value;
            result.useVector3Blending = true;
        }

        if (from.useColorBlending || to.useColorBlending)
        {
            result.colorValue = a * from.colorValue + b * mid.colorValue + t3 * to.colorValue;
            result.useColorBlending = true;
        }

        return result;
    }

    public static MyUIBlendingData Blend(MyUIBlendingData from, MyUIBlendingData to, float t)
    {
        MyUIBlendingData result = new MyUIBlendingData();

        if (from.useFloatBlending || to.useFloatBlending)
        {
            result.floatValue = Mathf.Lerp(from.floatValue, to.floatValue, t);
            result.useFloatBlending = true;
        }

        if (from.useVector3Blending || to.useVector3Blending)
        {
            result.vector3Value = Vector3.Lerp(from.vector3Value, to.vector3Value, t);
            result.useVector3Blending = true;
        }

        if (from.useColorBlending || to.useColorBlending)
        {
            result.colorValue = Color.Lerp(from.colorValue, to.colorValue, t);
            result.useColorBlending = true;
        }

        return result;
    }
}
