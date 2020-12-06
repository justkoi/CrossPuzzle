using UnityEngine;
using Global.Constants;
using System.Collections.Generic;

public enum E_LOOP_TYPE
{
    PlayOnce,
    PlayN,
    Loop
}

/// <summary>
/// 어떻게 사운드를 재생할것인가? 그에 관한 데이터 묶음
/// </summary>
[System.Serializable]
public class SoundPlayData {

    /// <summary>
    /// 어떤 사운드 파일을 재생 할 것인가?
    /// </summary>
    public string m_strSoundFileName;
    /// <summary>
    /// 어디서 재생 할 것인가?
    /// </summary>
    public Transform m_trPlace;

    /// <summary>
    /// 어떤 오디오 그룹에 속 할 것인가?
    /// </summary>
    public E_AUDIO_GROUP_TYPE m_eAudioGroup;

    /// <summary>
    /// 어떤 오디오 채널에서 재생 할 것인가?
    /// </summary>
    public E_AUDIO_CHANNEL_TYPE m_eAudioChannel;

    /// <summary>
    /// 로컬 로드인가?
    /// </summary>
    public bool m_bLocalLoad;

    /// <summary>
    /// 해당 객체의 어떤 사운드인가? (예, 같은 객체에서 같은 총알 사운드를 두개 이상 재생할 필요가 있을때 구분하기 위한 키)
    /// </summary>
    public string m_strKey;

    public string m_strFinalKey;

    /// <summary>
    /// 어떻게 루프 시킬 것인가?
    /// </summary>
    public E_LOOP_TYPE m_eLoopType;

    public bool m_bLoop
    {
        get
        {
            return m_eLoopType == E_LOOP_TYPE.Loop;
        }
    }

    /// <summary>
    /// 남은 루프 횟수
    /// </summary>
    public int m_nLoopCount;

    public bool m_bUseFadeIn;

    public bool m_bUseFadeOut;

    public float m_fFadeInTime;

    public float m_fFadeOutTime;

    public float m_fDelay;
    public float m_fVolume = 1.0f;

    public float m_fTime = 0.0f;

    public E_AUDIO_CLIP_GROUP m_eAudioClipGroup;

    public AudioClipGroup m_AudioClipGroup;

    public AudioClip m_LoadedAudioClip;

    public AudioPlayType m_PlayType;


    public SoundPlayData(SoundPlayData Data)
    {
        Init(Data.m_strSoundFileName, Data.m_eAudioGroup, Data.m_eAudioChannel, Data.m_eAudioClipGroup, Data.m_trPlace, Data.m_bLocalLoad, Data.m_eLoopType, Data.m_strKey, Data.m_fFadeInTime, Data.m_fFadeOutTime,Data.m_fDelay,Data.m_fVolume);
    }

    //public SoundPlayData(int nSoundId, E_AUDIO_GROUP_TYPE eAudioGroup, E_AUDIO_CHANNEL_TYPE eAudioChannel, Transform trPlace = null, bool bLocalLoad = false, E_LOOP_TYPE eLoopType = E_LOOP_TYPE.PlayOnce, string strKey = "Default", float fFadeInTime = 0.0f, float fFadeOutTime = 0.0f, float fDelay = 0.0f, float fVolume = 0.0f,float fTime = 0.0f)
    //{
    //    OldSoundData Data = SoundDataManager.Instance.GetSoundDataByID(nSoundId);
    //    if (Data == null) return;

    //    Init(Data.name, eAudioGroup, eAudioChannel, trPlace, bLocalLoad, eLoopType, strKey, fFadeInTime, fFadeOutTime,fDelay,fVolume, fTime);
    //}

    public SoundPlayData(string strSoundFileName, E_AUDIO_GROUP_TYPE eAudioGroup, E_AUDIO_CHANNEL_TYPE eAudioChannel, E_AUDIO_CLIP_GROUP eAudioClipGroup, Transform trPlace = null, bool bLocalLoad = false, E_LOOP_TYPE eLoopType = E_LOOP_TYPE.PlayOnce, string strKey = "Default", float fFadeInTime = 0.0f, float fFadeOutTime = 0.0f, float fDelay = 0.0f, float fVolume = 1.0f, float fTime = 0.0f)
    {
        Init(strSoundFileName, eAudioGroup, eAudioChannel, eAudioClipGroup, trPlace, bLocalLoad, eLoopType, strKey, fFadeInTime, fFadeOutTime,fDelay,fVolume, fTime);
    }

    /*
    public SoundPlayData(SoundData Data, E_AUDIO_GROUP_TYPE eAudioGroup, E_AUDIO_CHANNEL_TYPE eAudioChannel, E_AUDIO_CLIP_GROUP eAudioClipGroup, Transform trPlace = null, bool bLocalLoad = false, string strKey = "Default", float fVolume = 1.0f)
    {
        Init(Data.m_strFileName, eAudioGroup, eAudioChannel, eAudioClipGroup, trPlace, bLocalLoad, Data.m_eLoopType, strKey, Data.m_fFadeInTime, Data.m_fFadeOutTime,Data.m_fDelay,Data.m_fVolume * fVolume);
    }
    */

    static int globalSoundKey = 0;
    public void Init()
    {
        SoundPlayData Data = this;
        Init(Data.m_strSoundFileName, Data.m_eAudioGroup, Data.m_eAudioChannel, Data.m_eAudioClipGroup, Data.m_trPlace, Data.m_bLocalLoad, Data.m_eLoopType, Data.m_strKey, Data.m_fFadeInTime, Data.m_fFadeOutTime, Data.m_fDelay, Data.m_fVolume, Data.m_fTime);
    }

    public void Init(string strSoundFileName, E_AUDIO_GROUP_TYPE eAudioGroup, E_AUDIO_CHANNEL_TYPE eAudioChannel, E_AUDIO_CLIP_GROUP eAudioClipGroup, Transform trPlace = null, bool bLocalLoad = false, E_LOOP_TYPE eLoopType = E_LOOP_TYPE.PlayOnce, string strKey = "Default", float fFadeInTime = 0.0f, float fFadeOutTime = 0.0f, float fDelay = 0.0f, float fVolume = 0.0f, float fTime = 0.0f)
    {
        m_strSoundFileName = strSoundFileName;
        m_eAudioGroup = eAudioGroup;
        m_eAudioChannel = eAudioChannel;
        m_bLocalLoad = bLocalLoad;
        m_eLoopType = eLoopType;
        m_trPlace = trPlace;

        m_eAudioClipGroup = eAudioClipGroup;

        switch (m_eAudioClipGroup)
        {
            case E_AUDIO_CLIP_GROUP.Global:
                m_AudioClipGroup = AudioClipGroup.Global;
                break;
            case E_AUDIO_CLIP_GROUP.Map:
                m_AudioClipGroup = AudioClipGroup.Map;
                break;
            case E_AUDIO_CLIP_GROUP.Bgm_Map:
                m_AudioClipGroup = AudioClipGroup.Bgm_Map;
                break;
            case E_AUDIO_CLIP_GROUP.Bgm_Special:
                m_AudioClipGroup = AudioClipGroup.Bgm_Special;
                break;
            case E_AUDIO_CLIP_GROUP.UI:
                m_AudioClipGroup = AudioClipGroup.UI;
                break;
            default:
                m_AudioClipGroup = AudioClipGroup.Map;
                break;
        }

        m_strKey = strKey;

        if (m_trPlace == null)
        {
            m_trPlace = SoundManager.Instance.GetAudioChannelRoot(eAudioChannel);
            m_strKey = (globalSoundKey++).ToString();
        }


        m_fFadeInTime = fFadeInTime;
        m_bUseFadeIn = !(m_fFadeInTime <= 0.0001f);
        m_fFadeOutTime = fFadeOutTime;
        m_bUseFadeOut = !(m_fFadeOutTime <= 0.0001f);

        m_fDelay = fDelay;
        m_fVolume = fVolume;
        m_fTime = fTime;
        PublishKey();
    }

    public void PublishKey()
    {
        if (m_trPlace == null)
        {
            Debugs.LogError("[사운드-키발급] 발급의 대상이 되는 객체가 없습니다! 채널을 먼저 생성하십시오.");
            return;
        }

        m_strFinalKey = string.Concat(m_trPlace.GetInstanceID(), m_strKey);
    }

    public void SetAudioClipGroup(AudioClipGroup AudioClipGroup)
    {
        m_AudioClipGroup = AudioClipGroup;
    }

    /// <summary>
    /// 요청 딜레이 시간 - 로드에 걸린 딜레이 시간 계산
    /// </summary>
    /// <param name="fDelay"></param>
    private void UpdateDelay(float fDelay)
    {
        m_fDelay -= fDelay;
    }

    /// <summary>
    /// 로드된 클립 게시
    /// </summary>
    /// <param name="TargetClip"></param>
    private void PublishAudioClip(AudioClip TargetClip)
    {
        m_LoadedAudioClip = TargetClip;
    }

    /// <summary>
    /// 로드 완료 시 값 설정
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="TargetClip"></param>
    /// <param name="fDelay"></param>
    public void LoadComplete(AudioClip TargetClip, float fDelay)
    {
        UpdateDelay(fDelay);
        PublishAudioClip(TargetClip);
    }

    public void TryPlay()
    {
        m_PlayType.TryPlay(this);
    }
}

[System.Serializable]
public class SoundPlayDataGroup
{
    public List<SoundPlayData> m_DataList = new List<SoundPlayData>();
}