using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MyUIColorBlender : MonoBehaviour {

    [System.Serializable]
    public class ColorBlendingEvent : UnityEvent<Color> { }

    public Color from;
    public Color to;
    public AnimationCurve lerpModel;
    public ColorBlendingEvent colorBlendingEvent;

    private Color curBlendingResult;
    public Color CurBlendingResult { get { return curBlendingResult; } }

    public void Blend(float f)
    {
        if (lerpModel.keys.Length > 0)
            curBlendingResult = Color.Lerp(from, to, lerpModel.Evaluate(f));
        else
            curBlendingResult = Color.Lerp(from, to, f);
        colorBlendingEvent.Invoke(curBlendingResult);
    }

    public void SetFrom(Color value)
    {
        from = value;
    }

    public void SetTo(Color value)
    {
        to = value;
    }
}
