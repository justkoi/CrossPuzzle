using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum E_AUDIO_GROUP_TYPE
{
    UI,
    UI_Base,
    UI_CV,


    InGame_BGM,
    InGame_Player,
    InGame_PlayerMulti,
    InGame_Monster,
    InGame_Pet,
    InGame,
    Max,
}

class E_AUDIO_GROUP_TYPEComparer : System.Collections.Generic.IEqualityComparer<E_AUDIO_GROUP_TYPE>
{
    bool System.Collections.Generic.IEqualityComparer<E_AUDIO_GROUP_TYPE>.Equals(E_AUDIO_GROUP_TYPE x, E_AUDIO_GROUP_TYPE y)
    {
        return (int)x == (int)y;
    }

    int System.Collections.Generic.IEqualityComparer<E_AUDIO_GROUP_TYPE>.GetHashCode(E_AUDIO_GROUP_TYPE obj)
    {
        return ((int)obj).GetHashCode();
    }
}
public class AudioGroup : MonoBehaviour {
    
    public List<AudioObject> m_AudioObjectList;

    public AudioGroup m_ParentGroup;
    public E_AUDIO_GROUP_TYPE m_eAudioGroupType;
    public float _m_fVolume;
    public float m_fVolume
    {
        get
        {
            return _m_fVolume;
        }
        set
        {
            _m_fVolume = value;
            ApplyChannelVolume();
        }
    }

    public float _m_fFadeVolume;
    public float m_fFadeVolume
    {
        get
        {
            return _m_fFadeVolume;
        }
        set
        {
            _m_fFadeVolume = value;
            ApplyChannelVolume();
        }
    }

    public float m_fVolumeTotal
    {
        get
        {
            float fParentVolumeTotal = 1.0f;
            if (m_ParentGroup != null)
                fParentVolumeTotal = m_ParentGroup.m_fVolumeTotal;

            return m_fVolume * m_fFadeVolume * fParentVolumeTotal;
        }
    }

    public void ApplyChannelVolume()
    {
        for (int i = 0; i < m_AudioObjectList.Count; i++)
        {
            m_AudioObjectList[i].ApplyVolume();
        }
    }

    public MyUIBlendingModel m_Fader;

    /// <summary>
    /// From에서 To까지 페이드 진행
    /// </summary>
    /// <param name="fFrom"></param>
    /// <param name="fTo"></param>
    /// <param name="fTime"></param>
    public void FadeTo(float fFrom, float fTo, float fTime)
    {
        m_Fader.datas[0].myUIBlendingData.floatValue = fFrom;
        m_Fader.datas[1].myUIBlendingData.floatValue = fTo;
        m_Fader.StartBlending(0, 1, fTime);
    }

    public void Init(E_AUDIO_GROUP_TYPE eAudioGroupType)
    {
        m_AudioObjectList = new List<AudioObject>();
        m_eAudioGroupType = eAudioGroupType;
        m_fVolume = 1.0f;
        m_fFadeVolume = 1.0f;
    }

    public void SetParentGroup(AudioGroup ParentGroup)
    {
        m_ParentGroup = ParentGroup;
    }

    public void AddAudioObject(AudioObject TargetObject)
    {
        m_AudioObjectList.Add(TargetObject);
    }

    public void RemoveAudioObject(AudioObject TargetObject)
    {
        m_AudioObjectList.Remove(TargetObject);
    }
}
