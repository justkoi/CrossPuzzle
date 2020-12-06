using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MyUIVector3Blender : MonoBehaviour {

    [System.Serializable]
    public class Vector3BlendingEvent : UnityEvent<Vector3> { }

    public Vector3 from;
    public Vector3 to;
    public AnimationCurve lerpModel;
    public Vector3BlendingEvent vector3BlendingEvent;

    private Vector3 curBlendingResult;
    public Vector3 CurBlendingResult { get { return curBlendingResult; } }

    public void Blend(float f)
    {
        if(lerpModel.keys.Length > 0)
            curBlendingResult = Vector3.Lerp(from, to, lerpModel.Evaluate(f));
        else
            curBlendingResult = Vector3.Lerp(from, to, f);
        vector3BlendingEvent.Invoke(curBlendingResult);
    }

    public void SetFrom(Vector3 value)
    {
        from = value;
    }

    public void SetTo(Vector3 value)
    {
        to = value;
    }
}
