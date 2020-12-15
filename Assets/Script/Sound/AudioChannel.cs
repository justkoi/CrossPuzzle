using UnityEngine;
using System.Collections;
using Global.Constants;
using Audio.Sound;
using System.Collections.Generic;
/// <summary>
/// 오디오 채널입니다. (동시에 몇개의 실제 오디오 오브젝트를 가질 것인지를 컨트롤 합니다.
/// </summary>
/// 
public enum E_AUDIO_CHANNEL_TYPE
{
    BGM_Map,
    SE,
    UISE,
    CV_3D,
    CV_2D,
    BGM_SPECIAL,
}

class E_AUDIO_CHANNEL_TYPEComparer : System.Collections.Generic.IEqualityComparer<E_AUDIO_CHANNEL_TYPE>
{
    bool System.Collections.Generic.IEqualityComparer<E_AUDIO_CHANNEL_TYPE>.Equals(E_AUDIO_CHANNEL_TYPE x, E_AUDIO_CHANNEL_TYPE y)
    {
        return (int)x == (int)y;
    }

    int System.Collections.Generic.IEqualityComparer<E_AUDIO_CHANNEL_TYPE>.GetHashCode(E_AUDIO_CHANNEL_TYPE obj)
    {
        return ((int)obj).GetHashCode();
    }
}
public class AudioChannel : MonoBehaviour
{
    /// <summary>
    /// 이 채널에서 오디오 클립을 어떻게 재생할지 결정하는 타입입니다.
    /// </summary>
    private AudioPlayType m_PlayType;
    /// <summary>
    /// 이 채널에서 오디오 클립을 어떻게 로드할지 결정하는 타입입니다.
    /// </summary>
    private AudioLoadType m_LoadType;

    public AudioFilter m_AudioFilter;

    /// <summary>
    /// 채널내의 활성화된 사운드 오브젝트 (플레이중인...)
    /// </summary>
    public List<AudioObject> m_AudioActiveObjectList;

    /// <summary>
    /// 채널내의 비활성화된 사운드 오브젝트 (꺼져있는...)
    /// </summary>
    public List<AudioObject> m_AudioDeactiveObjectList;

    /// <summary>
    /// 객체Id+CustomKey , 오브젝트
    /// </summary>
    public CustomDictionary<string, AudioObject> m_AudioObjectList;

    /// <summary>
    /// 채널 타입
    /// </summary>
    public E_AUDIO_CHANNEL_TYPE m_eChannelType = E_AUDIO_CHANNEL_TYPE.BGM_Map;

    /// <summary>
    /// 이 채널에서 재생 가능한 최대 동시 재생 사운드 오브젝트 수
    /// </summary>
    public int m_nMaxAudioObject;

    /// <summary>
    /// 인스펙터 창에서 채널의 위치
    /// </summary>
    public Transform m_trTransform;

    public AudioObject AudioObjectPrefab;
    /*
    private AudioObject AudioObjectPrefab
    {
        get
        {
            if (audioObjectPrefab == null)
            {
                audioObjectPrefab = BundlePatchManager.Instance.LocalResourcesLoad<AudioObject>("Prefabs/Common/Sound/AudioObjectPrefab");
                audioObjectPrefab.transform.localPosition = Vector3.zero;
            }
            return audioObjectPrefab;
        }
    }
    */

    /// <summary>
    /// 이 채널이 활성화중인가?
    /// </summary>
    public bool m_bActive = true;

    public float _m_fStandardVolume;
    public float m_fStandardVolume
    {
        get
        {
            return _m_fStandardVolume;
        }
        set
        {
            _m_fStandardVolume = value;
            ApplyChannelVolume();
        }
    }

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
            return m_fVolume * m_fFadeVolume * m_fStandardVolume;
        }
    }


    public bool _m_bMute;

    public bool m_bMute
    {
        get
        {

            return _m_bMute;
        }
        set
        {
            _m_bMute = value;
            ApplyChannelMute();
        }

    }

    /// <summary>
    /// 현재 재생중인가?
    /// </summary>
    public bool m_bIsPlaying
    {
        get
        {
            return m_AudioActiveObjectList.Count > 0;
        }
    }

    /// <summary>
    /// 현재 재생중이고 앞으로도 재생중일 것인가?
    /// </summary>
    public bool m_bIsPlayingWithoutFadingOut
    {
        get
        {

            return m_AudioActiveObjectList.FindAll(x => !(x.m_bFadingOut && x.m_eFadeProcess == E_SOUND_FADE_PROCESS.STOP)).Count > 0;
        }
    }
    /// <summary>
    /// 현재 재생중이고 앞으로도 재생중일 리스트의 카운트
    /// </summary>
    public int m_nPlayingAudioCount
    {
        get
        {
            return m_AudioActiveObjectList.FindAll(x => !(x.m_bFadingOut && x.m_eFadeProcess == E_SOUND_FADE_PROCESS.STOP)).Count;
        }
    }

    public MyUIBlendingModel m_Fader;

    /// <summary>
    /// 이 채널의 우선순위
    /// </summary>
    public int m_nPriority;

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

    /// <summary>
    /// 가장 마지막에 플레이된 오디오, 현재 플레이중인 배경음을 구하거나 할때 이 필드를 사용하십시오.
    /// </summary>
    public AudioObject m_LastPlayingAudio
    {
        get
        {
            if (m_AudioActiveObjectList.Count <= 0)
                return null;

            return m_AudioActiveObjectList[m_AudioActiveObjectList.Count - 1];
        }
    }

    public float m_fSpatialBlend;

    public void ApplyChannelVolume()
    {
        for (int i = 0; i < m_AudioActiveObjectList.Count; i++)
        {
            if (m_AudioActiveObjectList[i] == null)
                m_AudioActiveObjectList.Remove(m_AudioActiveObjectList[i]);
            m_AudioActiveObjectList[i].ApplyVolume();
        }

        //ApplyChannelActive();
    }

    public void ApplyChannelActive()
    {
        if (m_bActive && m_fVolumeTotal <= 0.000001f)
        {
            m_bActive = false;
        }
        else if (!m_bActive && m_fVolumeTotal > 0.0f)
        {
            m_bActive = true;
        }
    }

    public void Awake()
    {
        m_AudioFilter = new AudioFilter(0);
        m_nPriority = 256;
    }

    public void IncreasePriority()
    {
        m_nPriority--;
        if (m_nPriority < 0)
            m_nPriority = 0;
    }

    public void DecreasePriority()
    {
        m_nPriority++;
        if (m_nPriority > 256)
            m_nPriority = 256;
    }

    /// <summary>
    /// 이 오디오 채널의 재생 우선순위를 설정합니다. 
    /// 시작 재생 우선순위를 다르게 설정함으로서 각 채널별 우선순위를 결정할 수 있습니다.
    /// 하드웨어가 수용가능한 채널보다 많은 오디오소스가 있는 경우, 유니티는 오디오 소스를 가상화합니다. 가장 낮은 우선순위를(그리고 가청도를) 갖는 오디오소스는 가장 먼저 가상화됩니다. Priority는 정수형 변수로 0에서 256사이의 값을 갖습니다. 0은 가장 높은 우선순위, 256은 가장 낮은 우선순위를 나타냅니다.
    /// </summary>
    /// <param name="nStartPriority"></param>
    public void SetPriority(int nStartPriority)
    {
        m_nPriority = nStartPriority;
    }
    
    public void ApplyChannelMute()
    {
        for (int i = 0; i < m_AudioActiveObjectList.Count; i++)
        {
            if (m_AudioActiveObjectList[i] == null)
                m_AudioActiveObjectList.Remove(m_AudioActiveObjectList[i]);
            m_AudioActiveObjectList[i].ApplyMute();
        }
    }

    public void DeactiveList_Enqueue(AudioObject TargetAudio)
    {
        m_AudioDeactiveObjectList.Add(TargetAudio);
        m_AudioActiveObjectList.Remove(TargetAudio);
        TargetAudio.RemoveAudioGroup();
        TargetAudio.gameObject.SetActive(false);
        TargetAudio.transform.SetParent(this.m_trTransform);
        DecreasePriority();
    }

    public AudioObject DeactiveList_Dequeue()
    {
        if (m_AudioDeactiveObjectList.Count <= 0)
            return null;

        AudioObject TargetAudio = m_AudioDeactiveObjectList[0];
        m_AudioDeactiveObjectList.RemoveAt(0);
        if (TargetAudio == null)
        {
            Debugs.LogError("[사운드-재생] 꺼내온 비활성화중인 오디오 객체가 NULL입니다! 이미 파괴되었을 수 있습니다.");
            return null;
        }
        TargetAudio.gameObject.SetActive(true);
        IncreasePriority();
        return TargetAudio;
    }

    public AudioChannel(E_AUDIO_CHANNEL_TYPE eChannelType, int nMaxChannel, Transform trTransform, float fSpatialBlend = 1.0f)
    {
        Init(eChannelType, nMaxChannel, trTransform, fSpatialBlend);
    }

    /// <summary>
    /// 채널을 초기화합니다.
    /// </summary>
    /// <param name="eChannelType">채널 타입</param>
    /// <param name="nMaxChannel">최대 동시 재생 사운드 오브젝트 수</param>
    public void Init(E_AUDIO_CHANNEL_TYPE eChannelType, int nMaxChannel, Transform trTransform, float fSpatialBlend)
    {
        m_AudioActiveObjectList = new List<AudioObject>();
        m_AudioDeactiveObjectList = new List<AudioObject>();
        m_AudioObjectList = new CustomDictionary<string, AudioObject>();

        m_eChannelType = eChannelType;
        m_nMaxAudioObject = nMaxChannel;
        m_trTransform = trTransform;
        m_fSpatialBlend = fSpatialBlend;

        m_AudioActiveObjectList.Clear();
        m_AudioDeactiveObjectList.Clear();

        FillAudioObject(nMaxChannel);
        m_fVolume = 1.0f;
        m_fFadeVolume = 1.0f;
        m_bMute = false;
        m_bActive = true;

        switch (m_eChannelType)
        {
            case E_AUDIO_CHANNEL_TYPE.BGM_Map:
                m_PlayType = new AudioPlayType_Bgm_Map(this);
                m_LoadType = new AudioLoadType_Bgm(m_PlayType);
                break;
            case E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL:
                m_PlayType = new AudioPlayType_Bgm_Special(this);
                m_LoadType = new AudioLoadType_Bgm(m_PlayType);
                break;
            case E_AUDIO_CHANNEL_TYPE.CV_2D:
            case E_AUDIO_CHANNEL_TYPE.CV_3D:
                m_PlayType = new AudioPlayType_Effect(this);
                m_LoadType = new AudioLoadType_CV(m_PlayType);
                break;
            case E_AUDIO_CHANNEL_TYPE.SE:
            case E_AUDIO_CHANNEL_TYPE.UISE:
                m_PlayType = new AudioPlayType_Effect(this);
                m_LoadType = new AudioLoadType_Effect(m_PlayType);
                break;
        }
    }

    /// <summary>
    /// MaxChannel 수 만큼 오디오 오브젝트를 채웁니다.
    /// </summary>
    /// <param name="nMaxChannel"></param>
    public void FillAudioObject(int nMaxChannel)
    {
        for (int i = 0; i < nMaxChannel; i++)
        {
            AddAudioObject();
        }
    }

    public AudioObject AddAudioObject()
    {
        AudioObject NewAudioObject = CreateAudioObject();
        NewAudioObject.SetParent(m_trTransform);
        DeactiveList_Enqueue(NewAudioObject);
        return NewAudioObject;
    }

    /// <summary>
    /// 오디오 오브젝트를 채널에 생성하고 플레이하면 알아서 꺼내오는식으로 하는것이 좋을듯하다...
    /// </summary>
    public AudioObject CreateAudioObject()
    {
        AudioObject NewAudioObject = GameObject.Instantiate(AudioObjectPrefab);
        return NewAudioObject;
    }

    public AudioObject GetAudioObject()
    {
        if (m_AudioDeactiveObjectList.Count > 0)
        {
            return DeactiveList_Dequeue();
        }
        else
        {
            ///가장 오래된 오브젝트를 즉시 중지시킴
            Stop(m_AudioActiveObjectList[0], true, true);

            return DeactiveList_Dequeue();
        }
    }

    public void RemoveAudioObject()
    {

    }

    public void AddAudioObject(AudioObject TargetObject)
    {
        if (!m_AudioObjectList.ContainsKey(TargetObject.m_Data.m_strFinalKey))
        {
            m_AudioObjectList.Add(TargetObject.m_Data.m_strFinalKey, TargetObject);
#if UNITY_EDITOR
            SoundManager.Instance.EditOnly_g_nActiveAudioObjectCount++;
#endif
        }
    }

    public void RemoveAudioObject(AudioObject TargetObject)
    {
        if (m_AudioObjectList.ContainsKey(TargetObject.m_Data.m_strFinalKey))
        {
            m_AudioObjectList.Remove(TargetObject.m_Data.m_strFinalKey);
#if UNITY_EDITOR
            SoundManager.Instance.EditOnly_g_nActiveAudioObjectCount--;
#endif
        }
    }

    public void Play(SoundPlayData Data)
    {
        if (!m_AudioFilter.CheckFiltering(Data.m_strSoundFileName))
            TryLoad(Data);
    }

    /// <summary>
    /// 먼저 로드를 시도합니다. 성공하면 TryPlay로 이어지고 플레이 됩니다.
    /// </summary>
    /// <param name="Data"></param>
    public void TryLoad(SoundPlayData Data)
    {
        m_LoadType.TryLoad(Data);
    }

    public AudioObject GetRecentPlayingAudio()
    {
        for (int i = m_AudioActiveObjectList.Count - 1; i >= 0; i--)
        {
            if (m_AudioActiveObjectList[i].m_bFadingOut && m_AudioActiveObjectList[i].m_eFadeProcess == E_SOUND_FADE_PROCESS.STOP)
                continue;

            return m_AudioActiveObjectList[i];
        }
        return null;
    }

    /// <summary>
    /// 가장 최근의 오디오를 Fade후 일시정지 시킵니다.
    /// </summary>
    public void PauseLastBGM()
    {
        AudioObject m_CurrBGM = GetRecentPlayingAudio();
        if (m_CurrBGM != null)
            m_CurrBGM.FadeOutAndPause();
    }

    /// <summary>
    /// 가장 최근의 BGM을 중지 시킵니다.
    /// </summary>
    public void PauseBGM()
    {
        int nPlayingAudioCount = m_nPlayingAudioCount;
        if (nPlayingAudioCount == 1)
            SoundManager.Instance.PauseMapBGM();
        else if (nPlayingAudioCount > 1)
        {
            //PauseLastBGM();
        }
    }

    public void Stop(AudioObject TargetObject, bool bForcedStopWithoutFade = false, bool bPassReplay = false)
    {
        if (TargetObject == null)
            return;

        if (TargetObject.m_Data.m_bUseFadeOut && !bForcedStopWithoutFade)
            TargetObject.FadeOutAndStop();
        else
            TargetObject.Stop();

        if (!bPassReplay && m_eChannelType == E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL)
        {
            int nPlayingAudioCount = m_nPlayingAudioCount;
            ///중지후에 플레이 가능한 BGM이 없으면 맵 배경음을 재생.
            if (nPlayingAudioCount == 0)
                SoundManager.Instance.RePlayMapBGM();
            else ///있다면 해당 BGM을 재생
            {
                AudioObject Audio = GetRecentPlayingAudio();
                if (Audio != null)
                {
                    Audio.FadeInAndResume();
                }
            }
        }
    }

    public void Stop(string strFinalKey, bool bForced = false)
    {
        if (m_AudioObjectList.ContainsKey(strFinalKey))
        {
            Stop(m_AudioObjectList[strFinalKey], bForced);
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < m_AudioActiveObjectList.Count; i++)
        {
            Stop(m_AudioActiveObjectList[i]);
        }
    }

    public void StopAll(Transform trPlace)
    {
        for (int i = 0; i < m_AudioActiveObjectList.Count; i++)
        {
            if (m_AudioActiveObjectList[i].m_Data.m_trPlace != trPlace)
                continue;

            Stop(m_AudioActiveObjectList[i]);
        }
    }


    public void Pause(AudioObject TargetObject)
    {
        TargetObject.Pause();
    }

    public void Pause(string strFinalKey)
    {
        if (m_AudioObjectList.ContainsKey(strFinalKey))
        {
            Pause(m_AudioObjectList[strFinalKey]);
        }
    }
    public void Resume(AudioObject TargetObject)
    {
        TargetObject.Resume();
    }


    public void LateUpdate()
    {
        if (m_AudioFilter != null)
            m_AudioFilter.LateUpdate();
    }
}

public abstract class AudioLoadType
{
    protected AudioPlayType m_PlayType;
    public AudioLoadType(AudioPlayType PlayType)
    {
        m_PlayType = PlayType;
    }
    /// <summary>
    /// 먼저 오디오 클립 로드를 시도합니다. 성공하면 TryPlay로 이어지고 플레이를 시도합니다.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="eCachDestroy"></param>
    public abstract void TryLoad(SoundPlayData Data);
}

public sealed class AudioLoadType_Bgm : AudioLoadType
{
    public AudioLoadType_Bgm(AudioPlayType PlayType)
        : base(PlayType)
    {

    }

    public override void TryLoad(SoundPlayData Data)
    {
        SoundManager.Instance.m_Loader.LoadAsyncAudioClip(Data, m_PlayType);
    }
}

public sealed class AudioLoadType_CV : AudioLoadType
{
    public AudioLoadType_CV(AudioPlayType PlayType)
        : base(PlayType)
    {

    }

    public override void TryLoad(SoundPlayData Data)
    {
        SoundManager.Instance.m_Loader.LoadAsyncAudioClip(Data, m_PlayType);
    }
}

public sealed class AudioLoadType_Effect : AudioLoadType
{
    public AudioLoadType_Effect(AudioPlayType PlayType)
        : base(PlayType)
    {

    }

    public override void TryLoad(SoundPlayData Data)
    {
        SoundManager.Instance.m_Loader.LoadAsyncAudioClip(Data, m_PlayType);
    }
}
/// <summary>
/// 이 채널에서 어떤 방식으로 플레이 할 지 정의합니다.
/// </summary>
public abstract class AudioPlayType
{
    protected AudioChannel m_Channel;

    public AudioPlayType(AudioChannel Channel)
    {
        m_Channel = Channel;
    }
    /// <summary>
    /// 플레이를 시도합니다.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="Clip"></param>
    /// <param name="fDelayTime">로드까지 걸린시간 그 사운드의 지연시간에 빼줘야함</param>
    public abstract void TryPlay(SoundPlayData Data);

    public virtual bool CanPlay(SoundPlayData Data)
    {
        if (Data.m_LoadedAudioClip == null)
        {
            Debugs.LogError("[사운드-재생] 클립이 null입니다!");
            return false;
        }

        if (Data.m_trPlace == null)
        {
            Debugs.LogError("[사운드-재생] 재생할 장소가 null입니다!");
            return false;
        }
        return true;
    }

    public virtual void ExceptionProcess(AudioObject NewAudioObject)
    {
        ///버그에 대한 예외처리 및 디버그 코드 작성
        if (NewAudioObject == null)
        {
            Debugs.LogError("[사운드-재생] 새로 가져온 오브젝트가 null입니다! 어딘가에서 객체 파괴시 그 객체 하위에서 재생중인 사운드를 돌려주지 않았을 수 있습니다! 일단 새로운 객체를 재생성합니다.");
            NewAudioObject = m_Channel.AddAudioObject();
            m_Channel.m_AudioDeactiveObjectList.Remove(NewAudioObject);
            NewAudioObject.gameObject.SetActive(true);
            for (int i = 0; i < m_Channel.m_AudioActiveObjectList.Count; i++)
            {
                if (m_Channel.m_AudioActiveObjectList[i] == null)
                {
                    Debugs.LogError("[사운드-재생] 현재 활성화된 객체중 이미 NULL이 되버린 객체를 발견했습니다! 인덱스:", i);
                    m_Channel.m_AudioActiveObjectList.Remove(m_Channel.m_AudioActiveObjectList[i]);
                }
            }
        }
    }
}

public sealed class AudioPlayType_Bgm_Map : AudioPlayType
{
    public AudioPlayType_Bgm_Map(AudioChannel Channel)
        : base(Channel)
    {

    }

    public override void TryPlay(SoundPlayData Data)
    {
        if (!CanPlay(Data))
            return;

        ///이미 플레이중인 같은 클립이 있는지 검사합니다.
        AudioObject AlreadyPlayingAudio = m_Channel.m_AudioActiveObjectList.Find(x => x.m_Data.m_strSoundFileName == Data.m_strSoundFileName);
        bool bAlreadyPlaying = false;// AlreadyPlayingAudio != null; //같은 클ㄹ립 없다고 간주하게 수정함.
        ///이미 플레이중인 같은 클립의 오디오가 있다면 새로운 오디오는 그 오디오를 가져오고 아니라면 풀에서 꺼내옵니다.
        AudioObject NewAudioObject = bAlreadyPlaying ? AlreadyPlayingAudio : m_Channel.GetAudioObject();
        #region Exception
        ///버그에 대한 예외처리 및 디버그 코드 작성
        ExceptionProcess(NewAudioObject);
        #endregion

        ///오디오 정보를 세팅합니다.
        NewAudioObject.SetAudioChannel(m_Channel);
        NewAudioObject.SetAudioGroup(NewAudioObject.m_Data.m_eAudioGroup);
        NewAudioObject.SetPlayData(Data);
        NewAudioObject.SetParent(NewAudioObject.m_Data.m_trPlace);
        NewAudioObject.SetPriority(m_Channel.m_nPriority);
        NewAudioObject.transform.localPosition = Vector3.zero;
        NewAudioObject.OnStop = null;

        m_Channel.AddAudioObject(NewAudioObject);

        NewAudioObject.OnStop = () =>
        {
            m_Channel.DeactiveList_Enqueue(NewAudioObject);
            m_Channel.RemoveAudioObject(NewAudioObject);
        };

        ///같은 BGM을 플레이하는경우
        if (bAlreadyPlaying && NewAudioObject.m_bFadingOut)
        {
            ///가장 최근에 플레이한 BGM으로 교체하기위해 앞에서 빼고 뒤에 넣어줌
            m_Channel.m_AudioActiveObjectList.Remove(NewAudioObject);
            m_Channel.m_AudioActiveObjectList.Add(NewAudioObject);
        }
        else if (!bAlreadyPlaying)
        {
            NewAudioObject.FadeVolume = 0;
        }

        AudioObject TargetObject = m_Channel.m_AudioActiveObjectList.Count > 0 ? m_Channel.m_AudioActiveObjectList[0] : null;
        if ((bAlreadyPlaying && NewAudioObject.m_bFadingOut) || !bAlreadyPlaying)
        {
            ///가장 최근에 플레이한 오브젝트가 일시정지하려는 중이 아니면 중지시킵니다.
            if (TargetObject != null)
            {
                m_Channel.Stop(TargetObject);
            }

            ///대상 BGM을 루프로 재생합니다.
            if (!bAlreadyPlaying)
                NewAudioObject.Play(Data.m_LoadedAudioClip, NewAudioObject.m_Data.m_fDelay, 1, true, NewAudioObject.m_Data.m_fTime);
            if (SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL).m_nPlayingAudioCount > 0)
            {
                NewAudioObject.FadeVolume = 0;
                NewAudioObject.FadeOutAndPause();
            }
            else if (NewAudioObject.m_Data.m_bUseFadeIn)
            {
                NewAudioObject.FadeIn();
            }
        }

        ///이미 플레이 중이지 않았다면 활성화된 리스트에 추가합니다.
        if (!bAlreadyPlaying)
            m_Channel.m_AudioActiveObjectList.Add(NewAudioObject);

        m_Channel.m_AudioFilter.RecordFilterInfo(new AudioFilterInfo(Data.m_strSoundFileName));

        AudioClipGroup.RecordPlayingClip(Data.m_LoadedAudioClip);
    }
}


public sealed class AudioPlayType_Bgm_Special : AudioPlayType
{
    public AudioPlayType_Bgm_Special(AudioChannel Channel)
        : base(Channel)
    {

    }

    public override void TryPlay(SoundPlayData Data)
    {
        if (!CanPlay(Data))
            return;

        ///이미 플레이중인 같은 클립이 있는지 검사합니다.
        AudioObject AlreadyPlayingAudio = m_Channel.m_AudioActiveObjectList.Find(x => x.m_Data.m_strSoundFileName == Data.m_strSoundFileName);
        bool bAlreadyPlaying = AlreadyPlayingAudio != null;
        ///이미 플레이중인 같은 클립의 오디오가 있다면 새로운 오디오는 그 오디오를 가져오고 아니라면 풀에서 꺼내옵니다.
        AudioObject NewAudioObject = bAlreadyPlaying ? AlreadyPlayingAudio : m_Channel.GetAudioObject();
        #region Exception
        ///버그에 대한 예외처리 및 디버그 코드 작성
        ExceptionProcess(NewAudioObject);
        #endregion

        ///오디오 정보를 세팅합니다.
        NewAudioObject.SetAudioChannel(m_Channel);
        NewAudioObject.SetAudioGroup(NewAudioObject.m_Data.m_eAudioGroup);
        NewAudioObject.SetPlayData(Data);
        NewAudioObject.SetParent(NewAudioObject.m_Data.m_trPlace);
        NewAudioObject.SetPriority(m_Channel.m_nPriority);
        NewAudioObject.transform.localPosition = Vector3.zero;
        NewAudioObject.OnStop = null;

        m_Channel.AddAudioObject(NewAudioObject);

        NewAudioObject.OnStop = () =>
        {
            m_Channel.DeactiveList_Enqueue(NewAudioObject);
            m_Channel.RemoveAudioObject(NewAudioObject);
        };

        ///같은 BGM을 플레이하는경우
        if (bAlreadyPlaying && NewAudioObject.m_bFadingOut)
        {
            ///가장 최근에 플레이한 BGM으로 교체하기위해 앞에서 빼고 뒤에 넣어줌
            m_Channel.m_AudioActiveObjectList.Remove(NewAudioObject);
            m_Channel.m_AudioActiveObjectList.Add(NewAudioObject);
        }
        else if (!bAlreadyPlaying)
        {
            NewAudioObject.FadeVolume = 0;
        }

        AudioObject TargetObject = m_Channel.m_AudioActiveObjectList.Count > 0 ? m_Channel.m_AudioActiveObjectList[m_Channel.m_AudioActiveObjectList.Count - 1] : null;
        if ((bAlreadyPlaying && NewAudioObject.m_bFadingOut) || !bAlreadyPlaying)
        {
            ///가장 최근에 플레이한 오브젝트가 일시정지하려는 중이 아니면 중지시킵니다.
            if (TargetObject != null)
            {
                if (!TargetObject.IsPause)
                {
                    if (!TargetObject.m_bFadingOut)
                    {
                        TargetObject.FadeOutAndPause();
                    }
                }
            }

            ///대상 BGM을 루프로 재생합니다.
            if (!bAlreadyPlaying)
                NewAudioObject.Play(Data.m_LoadedAudioClip, NewAudioObject.m_Data.m_fDelay, 1, true, NewAudioObject.m_Data.m_fTime);
            if (NewAudioObject.m_Data.m_bUseFadeIn)
            {
                NewAudioObject.FadeIn();
            }
        }


        ///이미 플레이 중이지 않았다면 활성화된 리스트에 추가합니다.
        if (!bAlreadyPlaying)
            m_Channel.m_AudioActiveObjectList.Add(NewAudioObject);

        m_Channel.m_AudioFilter.RecordFilterInfo(new AudioFilterInfo(Data.m_strSoundFileName));
        m_Channel.PauseBGM();

        AudioClipGroup.RecordPlayingClip(Data.m_LoadedAudioClip);
    }
}

public sealed class AudioPlayType_Effect : AudioPlayType
{
    public AudioPlayType_Effect(AudioChannel Channel)
        : base(Channel)
    {

    }

    public override void TryPlay(SoundPlayData Data)
    {
        if (!CanPlay(Data))
            return;

        if (m_Channel.m_AudioObjectList.ContainsKey(Data.m_strFinalKey))
        {
            m_Channel.Stop(m_Channel.m_AudioObjectList[Data.m_strFinalKey]);
        }

        ///이미 플레이중인 같은 클립의 오디오가 있다면 새로운 오디오는 그 오디오를 가져오고 아니라면 풀에서 꺼내옵니다.
        AudioObject NewAudioObject = m_Channel.GetAudioObject();


        #region Exception
        ExceptionProcess(NewAudioObject);
        #endregion

        ///오디오 정보를 세팅합니다.
        NewAudioObject.SetAudioChannel(m_Channel);
        NewAudioObject.SetAudioGroup(NewAudioObject.m_Data.m_eAudioGroup);
        NewAudioObject.SetPlayData(Data);
        NewAudioObject.SetParent(NewAudioObject.m_Data.m_trPlace);
        NewAudioObject.SetPriority(m_Channel.m_nPriority);
        NewAudioObject.transform.localPosition = Vector3.zero;
        NewAudioObject.OnStop = null;

        m_Channel.AddAudioObject(NewAudioObject);

        //AudioObject TargetObject = m_Channel.m_AudioActiveObjectList.Count > 0 ? m_Channel.m_AudioActiveObjectList[m_Channel.m_AudioActiveObjectList.Count - 1] : null;

        NewAudioObject.Play(Data.m_LoadedAudioClip, NewAudioObject.m_Data.m_fDelay, 1, NewAudioObject.m_Data.m_bLoop, NewAudioObject.m_Data.m_fTime);

        NewAudioObject.OnStop = () =>
        {
            m_Channel.DeactiveList_Enqueue(NewAudioObject);
            m_Channel.RemoveAudioObject(NewAudioObject);
        };

        m_Channel.m_AudioActiveObjectList.Add(NewAudioObject);

        m_Channel.m_AudioFilter.RecordFilterInfo(new AudioFilterInfo(Data.m_strSoundFileName));

        AudioClipGroup.RecordPlayingClip(Data.m_LoadedAudioClip);
    }
}