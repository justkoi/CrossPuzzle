using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public EffectData[] effectList;
    public Transform trHolder;

    public EffectData PlayEffect(E_EFFECT_TYPE eEffectType, float x,float y)
    {
        EffectData effect = Instantiate<EffectData>(effectList[(int)eEffectType]);
        effect.transform.parent = trHolder;
        effect.transform.SetLocalPositionXY(x, y);
        return effect;
    }

    public void PlayLineEffect(Vector2 A, Vector2 B)
    {
        float xDis = (A.x - B.x);
        float yDis = (A.y - B.y);
        float fDistance = Mathf.Sqrt((xDis* xDis) + (yDis*yDis));
        float fAngle = Mathf.Atan2(yDis, xDis) * Mathf.Rad2Deg;
        EffectData effectData = PlayEffect(E_EFFECT_TYPE.Line, A.x - (xDis/2), A.y - (yDis/2));
        effectData.transform.localScale = new Vector3(fDistance / 200.0f, effectData.transform.localScale.y, effectData.transform.localScale.z);
        effectData.transform.localEulerAngles = new Vector3(effectData.transform.localEulerAngles.x, effectData.transform.localEulerAngles.y, fAngle);
    }
}

public enum E_EFFECT_TYPE
{
    Pop_Red,
    Pop_Yellow,
    Pop_Blue,
    Pop_Green,
    Pop_Purple,
    Pop_White,
    Dot,
    Line,
    Msg_Dot,
    Msg_Line,
    Msg_Triangle,
    Msg_Square,
    Msg_Pentagon,
    Score,
    Lightning,
}
