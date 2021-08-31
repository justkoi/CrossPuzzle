using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BlendingModel : MonoBehaviour
{
    [System.Serializable]
    public class FloatBlendingEvent : UnityEvent<float> { }
    [System.Serializable]
    public class Vector3BlendingEvent : UnityEvent<Vector3> { }
    [System.Serializable]
    public class ColorBlendingEvent : UnityEvent<Color> { }

    public InspectorBlendingData[] datas;
    public AnimationCurve lerpModel;

    public FloatBlendingEvent floatBlendingEvent;
    public Vector3BlendingEvent vector3BlendingEvent;
    public ColorBlendingEvent colorBlendingEvent;

    private BlendingData curFrom;
    private BlendingData curTo;
    private BlendingData nowBlendingData;
    private float blendingTime;

    private bool isBlending;
    public bool IsBlending { get { return isBlending; } }

    private float lastTime;
    private BlendingData midData;

    public void InitAsInspected(int index)
    {
        StopBlending();
        if (datas.Length > index && datas[index] != null)
        {
            nowBlendingData = datas[index].BlendingData;
            InvokeToAll(datas[index].BlendingData);
        }
    }

    public void InitAs(BlendingData data)
    {
        StopBlending();
        InvokeToAll(data);
        nowBlendingData = data;
    }

    /// <summary>
    /// 인스펙터에 지정된 블랜딩 데이터를 사용하여 블랜딩합니다.
    /// </summary>
    public void StartBlending(int fromIndex, int toIndex, float blendingTime, System.Action<BlendingModel> onBlendingFinished = null)
    {
        var isContinued = isBlending;
        StopBlending();

        if (fromIndex >= datas.Length || toIndex >= datas.Length || datas[fromIndex] == null || datas[toIndex] == null || blendingTime == 0f)
        {
            if (onBlendingFinished != null)
                onBlendingFinished(this);
        }
        else
        {
            curFrom = datas[fromIndex].BlendingData;
            curTo = datas[toIndex].BlendingData;
            this.blendingTime = blendingTime;
            isBlending = true;
            CoroutineManager.Instance.AddCoroutine(this, BlendingProcess(onBlendingFinished, isContinued, false));
        }
    }

    /// <summary>
    /// 인자로 넘겨진 블랜딩 데이터를 사용하여 블랜딩합니다.
    /// </summary>
    public void StartBlending(BlendingData from, BlendingData to, float blendingTime, System.Action<BlendingModel> onBlendingFinished = null)
    {
        var isContinued = isBlending;
        curFrom = from;
        curTo = to;
        this.blendingTime = blendingTime;

        StopBlending();
        if (blendingTime != 0)
        {
            isBlending = true;
            CoroutineManager.Instance.AddCoroutine(this, BlendingProcess(onBlendingFinished, isContinued, false));
        }
        else
        {
            if (onBlendingFinished != null)
                onBlendingFinished(this);
        }
    }

    /// <summary>
    /// 현재 블랜딩 단계에서 인스펙터에 지정된 블랜딩 데이터를 사용하여 블랜딩합니다.
    /// </summary>
    public void StartBlendingFromNow(int toIndex, float blendingTime, System.Action<BlendingModel> onBlendingFinished = null, bool useBezierLerp = false)
    {
        var isContinued = isBlending;
        StopBlending();

        if (toIndex >= datas.Length || datas[toIndex] == null || blendingTime == 0f)
        {
            if (onBlendingFinished != null)
                onBlendingFinished(this);
        }
        else
        {
            curFrom = nowBlendingData;
            curTo = datas[toIndex].BlendingData;
            this.blendingTime = blendingTime;
            isBlending = true;
            CoroutineManager.Instance.AddCoroutine(this, BlendingProcess(onBlendingFinished, isContinued, isContinued && useBezierLerp)); // 베지어 러프는 이어서 움직이는 경우에만 유효합니다.
        }
    }

    /// <summary>
    /// 현재 블랜딩 단계에서 인자로 넘겨진 블랜딩 데이터를 사용하여 블랜딩합니다.
    /// </summary>
    public void StartBlendingFromNow(BlendingData to, float blendingTime, System.Action<BlendingModel> onBlendingFinished = null, bool useBezierLerp = false)
    {
        var isContinued = isBlending;
        if (useBezierLerp)
            midData = curTo;
        curFrom = nowBlendingData;
        curTo = to;
        this.blendingTime = blendingTime;

        StopBlending();
        if (blendingTime != 0f)
        {
            isBlending = true;
            CoroutineManager.Instance.AddCoroutine(this, BlendingProcess(onBlendingFinished, isContinued, isContinued && useBezierLerp)); // 베지어 러프는 이어서 움직이는 경우에만 유효합니다.
        }
        else
        {
            if (onBlendingFinished != null)
                onBlendingFinished(this);
        }
    }

    public void StopBlending()
    {
        isBlending = false;
        if (CoroutineManager.Instance)
        CoroutineManager.Instance.RemoveAllCoroutines(this);
    }

    private IEnumerator BlendingProcess(System.Action<BlendingModel> onBlendingFinished, bool isContinued, bool useBezierLerp)
    {
        float elapsedTime = 0f;
        float nowTime = Time.realtimeSinceStartup;
        
        if (isContinued)// 이렇게 시간을 넣어주지 않으면 한창 동작 중인 애니메이션의 경우 한 프레임 멈춘다.
        {
            float deltaTime = nowTime - lastTime;
            elapsedTime = deltaTime;
        }
        lastTime = nowTime;

        do
        {
            if (blendingTime == 0f)
                break;
            
            var blendingValue = useBezierLerp ? elapsedTime / blendingTime : lerpModel.Evaluate(elapsedTime / blendingTime);

            if (useBezierLerp)
                nowBlendingData = BlendingData.BezierBlend(curFrom, midData, curTo, blendingValue);
            else
                nowBlendingData = BlendingData.Blend(curFrom, curTo, blendingValue);
            InvokeToAll(nowBlendingData);

            yield return null;

            nowTime = Time.realtimeSinceStartup;
            elapsedTime += nowTime - lastTime;
            lastTime = nowTime;

            if (elapsedTime >= blendingTime)
                elapsedTime = blendingTime;

        } while (elapsedTime < blendingTime);

        nowBlendingData = BlendingData.Blend(curFrom, curTo, lerpModel.Evaluate(1f));
        
        InvokeToAll(nowBlendingData);
        isBlending = false;
        if (onBlendingFinished != null)
            onBlendingFinished(this);
    }

    private void InvokeToAll(BlendingData blendingResult)
    {
        if (blendingResult.useFloatBlending)
            floatBlendingEvent.Invoke(blendingResult.floatValue);
        if (blendingResult.useVector3Blending)
            vector3BlendingEvent.Invoke(blendingResult.vector3Value);
        if (blendingResult.useColorBlending)
            colorBlendingEvent.Invoke(blendingResult.colorValue);
    }

    private void OnDestroy()
    {
        if(CoroutineManager.Instance != null)
            CoroutineManager.Instance.RemoveAllCoroutines(this);
    }
}