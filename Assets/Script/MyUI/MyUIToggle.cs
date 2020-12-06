using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MyUIToggle : MonoBehaviour {
    
    public enum ToggleMode { Normal, Blending }

    [System.Serializable]
    public class OnBeforeToggleStart : UnityEvent<MyUIToggle> { }
    [System.Serializable]
    public class OnAfterToggleFinish : UnityEvent<MyUIToggle> { }

    public ToggleMode mode;
    public MyUIBlendingModel[] blendingModels = new MyUIBlendingModel[0];
    public float blendingTime;
    [SerializeField]
    private bool isOn;
    public OnBeforeToggleStart onBeforeToggleStart;
    public OnAfterToggleFinish onAfterToggleFinish;

    private System.Action<MyUIToggle> instantBlendingFinishCallback;
    private int curListeningModelCount;
    public bool IsToggling { get { return curListeningModelCount > 0; } }

    public bool IsOn
    {
        get { return isOn; }
        set
        {
            if(isOn != value)
                SetValueWithBlending(value);
        }
    }

    public void Init()
    {
        curListeningModelCount = 0;

        for (int i = 0; i < blendingModels.Length; i++)
        {
            if (blendingModels[i] == null)
                continue;
            blendingModels[i].InitAsInspected(isOn ? 1 : 0);
        }
        onAfterToggleFinish.Invoke(this);
    }

    public void SwitchToggleValue()
    {
        SetValueWithBlending(!isOn);
    }

    public void SetValueWithoutBlending(bool value)
    {
        isOn = value;
        for (int i = 0; i < blendingModels.Length; i++)
        {
            if (blendingModels[i] == null)
                continue;
            blendingModels[i].InitAsInspected(isOn ? 1 : 0);
        }
        onAfterToggleFinish.Invoke(this);
    }

    public void SetValueWithBlending(bool value, System.Action<MyUIToggle> instantBlendingFinishCallback = null)
    {
        isOn = value;
        onBeforeToggleStart.Invoke(this);
        this.instantBlendingFinishCallback = instantBlendingFinishCallback;

#if UNITY_EDITOR
        if (!Application.isPlaying && CoroutineManager.Instance.IsRunningOnEditor == false)
        {
            for (int i = 0; i < blendingModels.Length; i++)
            {
                if (blendingModels[i] == null)
                    continue;
                blendingModels[i].InitAsInspected(isOn ? 1 : 0);
            }
            onAfterToggleFinish.Invoke(this);
            return;
        }
#endif

        if (mode == ToggleMode.Blending && blendingTime != 0)
        {
            curListeningModelCount = 0;
            for (int i = 0; i < blendingModels.Length; i++)
            {
                if (blendingModels[i] == null)
                    continue;

                curListeningModelCount++; // 모든 블랜더에게서 onAfterToggleFinish 컬백이 실행되서는 안되기에 이렇게 갯수를 카운트해줍니다.
                if (blendingModels[i].IsBlending)
                    blendingModels[i].StartBlendingFromNow(isOn ? 1 : 0, blendingTime, OnBlendingFinished);
                else
                    blendingModels[i].StartBlending(isOn ? 0 : 1, isOn ? 1 : 0, blendingTime, OnBlendingFinished);
            }
        }
        else
        {
            for (int i = 0; i < blendingModels.Length; i++)
            {
                if (blendingModels[i] == null)
                    continue;
                blendingModels[i].InitAsInspected(isOn ? 1 : 0);
            }
            onAfterToggleFinish.Invoke(this);
        }
    }

    private void OnBlendingFinished(MyUIBlendingModel model)
    {
        curListeningModelCount--;
        if (curListeningModelCount > 0)
            return;
        
        onAfterToggleFinish.Invoke(this);
        if(instantBlendingFinishCallback != null)
        {
            var curCallback = instantBlendingFinishCallback;
            instantBlendingFinishCallback(this); // 이 콜백이 실행되는 동안 instantBlendingFinishCallback 변경되는 경우 무시합니다.
            if (curCallback == instantBlendingFinishCallback)
                instantBlendingFinishCallback = null;
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < blendingModels.Length; i++)
        {
            if (blendingModels[i].IsBlending)
                return;
        }

        for (int i = 0; i < blendingModels.Length; i++)
        {
            if (IsOn)
                blendingModels[i].InitAsInspected(1);
            else
                blendingModels[i].InitAsInspected(0);
        }
    }
}