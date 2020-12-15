using System;
using UnityEngine;
using System.Collections;
using Audio.Sound;
using Global.Constants;

public enum E_SOUND_FADE_PROCESS
{
    STOP,
    PAUSE,
}

/// <summary>
/// 오디오 오브젝트 설정.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public sealed class AudioObject : MonoBehaviour
{
    //초기데이터. Prefab 미리 설정시 적용.
    [SerializeField]
    public AudioData audioData = new AudioData();
    /// <summary>
    /// 오디오 그룹
    /// </summary>
    private AudioGroup audioGroup;
    private AudioSource audioSource;
    public E_SOUND_FADE_PROCESS m_eFadeProcess;
    //private float _m_fFaderVolume = 1.0f;
    //public float m_fFaderVolume
    //{
    //    get
    //    {
    //        return _m_fFaderVolume;
    //    }
    //    set
    //    {
    //        _m_fFaderVolume = value;
    //        ApplyVolume(_m_fFaderVolume);
    //    }
    //}

    //private double playTime = -1;
    //private double playStartTimeLocal = -1;
    //private double playStartTimeSystem = -1;

    private double audioTime = 0f;

    private bool isPlay = false;

    public AudioGroup m_AudioGroup;
    public AudioChannel m_AudioChannel;

    private float audioVolume = 1.0f;
    public float Volume { get { return audioVolume; } set {
            audioVolume = value;
            ApplyVolume(); } }

    private float fadeVolume = 1.0f;
    public float FadeVolume { get { return fadeVolume; } set { fadeVolume = value; ApplyVolume(); } }
    public float VolumeTotal
    {
        get { return FadeVolume * Volume * m_AudioChannel.m_fVolumeTotal * m_AudioGroup.m_fVolumeTotal; }// * audioGroup.Volume; }
    }

    public bool IsPlaying { get { return isPlay || isPlay; } }
    /// <summary>
    /// -1:Left100%, 0, 1:Right100%;
    /// </summary>
    public float Pen { get { return audioSource.panStereo; } set { audioSource.panStereo = value; } }

    public float Pitch { get { return audioSource.pitch; } set { audioSource.pitch = value; } }

    public float MinDistance { get { return audioSource.minDistance; } set { audioSource.minDistance = value; } }
    public float MaxDistance { get { return audioSource.maxDistance; } set { audioSource.maxDistance = value; } }
    /// <summary>
    /// 0.0f(2D) ~ 1.0f(3D)
    /// </summary>
    public float SpatialBlend { get { return audioSource.spatialBlend; } set { audioSource.spatialBlend = value; } }

    public int Priority { get { return audioSource.priority; } set { audioSource.priority = value; } }

    private bool audioMute = false;
    public bool Mute { get { return audioMute; } set { audioMute = value; ApplyMute(); } }
    public bool MuteTotal { get { return Mute | m_AudioChannel.m_bMute; } }//| audioGroup.Mute;

    public float m_fTime
    {
        get
        {
            return audioSource.time;
        }
        set
        {
            audioSource.time = value;
        }
    }

    private bool isPause;
    /// <summary>
    /// 일시중지 설정.
    /// </summary>
    public bool IsPause
    {
        get { return isPause; }
        set { if (isPause == value) return; if (isPause) Pause(); else Resume(); }
    }

    public AudioClip curAudioClip { get { return audioSource.clip; } }

    private bool isApplicationPause;
    
    public Action OnComplete, OnStop, OnFaidOutComplete;

    public SoundPlayData m_Data;

    public MyUIBlendingModel m_Fader;

    //public E_SOUND_PLAY_CONDITION m_ePlayCondition;

    public void SetAudioChannel(AudioChannel AudioChannel)
    {
        m_AudioChannel = AudioChannel;
        if (AudioChannel.m_fSpatialBlend < 0.999999f)
            SpatialBlend = AudioChannel.m_fSpatialBlend;
    }

    public void SetAudioGroup(E_AUDIO_GROUP_TYPE eAudioGroup)
    {
        if (SoundManager.Instance == null)
            return;

        m_AudioGroup = SoundManager.Instance.GetAudioGroup(eAudioGroup);
        if(m_AudioGroup != null)
            m_AudioGroup.AddAudioObject(this);
    }

    public void SetPlayData(SoundPlayData Data)
    {
        m_Data = Data;
        this.Volume = Data.m_fVolume;
    }

    public void SetPriority(int nPriority)
    {
        Priority = nPriority;
    }
    
    /// <summary>
    /// 사운드 플레이
    /// </summary>
    /// <param name="audioclip"></param>
    /// <param name="delay"></param>
    /// <param name="volume"></param>
    /// <param name="loop"></param>
    public void Play(AudioClip audioclip, float delay = 0f, float volume = 1.0f, bool loop = false, float fTime = 0.0f)
    {
        if(audioclip == null)
        {
            Debugs.LogWarning("[AudioObject] Play() Clip is Null !!");
            return;
        }

        Resume();

        audioSource.clip = audioclip;
        audioSource.loop = loop;

        ApplyVolume(volume);

        audioSource.PlayDelayed(delay);

        isPlay = true;
        //playTime = audioTime;
        //playStartTimeLocal = audioTime + delay;
        //playStartTimeSystem = SoundManager.systemTime + delay;

        m_fTime = fTime;
    }

    public void PlayOneShot(AudioClip clip, float volume = 1.0f)
    {
        audioSource.PlayOneShot(clip, VolumeTotal * volume);
        isPlay = true;
    }

    public void RemoveAudioGroup()
    {
        if (m_AudioGroup != null) m_AudioGroup.RemoveAudioObject(this);
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        if (curAudioClip != null)
        {
            AudioClipGroup.ErasePlayingClip(curAudioClip);

            if (m_Data.m_eAudioClipGroup == E_AUDIO_CLIP_GROUP.Stop)
                AudioClipGroup.Stop.OnStop(curAudioClip);
            else if (m_Data.m_eAudioClipGroup == E_AUDIO_CLIP_GROUP.Bgm_Map)
                AudioClipGroup.Bgm_Map.OnStop(curAudioClip);
            else if (m_Data.m_eAudioClipGroup == E_AUDIO_CLIP_GROUP.Bgm_Special)
                AudioClipGroup.Bgm_Special.OnStop(curAudioClip);
        }

        audioSource.Stop();

        Reset();
        OnStop.Execute();

    }
    /// <summary>
    /// 정지(페이드아웃)
    /// </summary>
    /// <param name="fadeOutLength"></param>
    public void StopWithFadeOut()
    {
        FadeOut();
    }
    /// <summary>
    /// 일시정지
    /// </summary>
    public void Pause()
    {
        if (isPause)
            return;
        audioSource.Pause();
        isPause = true;
    }
    /// <summary>
    /// 이어서 재생.
    /// </summary>
    public void Resume()
    {
        if (false == isPause)
            return;
        audioSource.UnPause();
        isPause = false;
    }
    /// <summary>
    /// 볼륨 적용.
    /// </summary>
    /// <param name="volume"></param>
    public void ApplyVolume(float volume = 1.0f)
    {
        audioSource.volume = VolumeTotal * volume;
    }
    /// <summary>
    /// 뮤트 적용.
    /// </summary>
    public void ApplyMute()
    {
        audioSource.mute = MuteTotal;
    }

    /// <summary>
    /// 페이드 인 진행
    /// </summary>
    public void FadeIn()
    {
        FadeTo(FadeVolume, 1.0f, m_Data.m_fFadeInTime);
        m_bFadingIn = true;
        m_bFadingOut = false;
    }

    /// <summary>
    /// 페이드 아웃 진행
    /// </summary>
    public void FadeOut()
    {
        FadeTo(FadeVolume, 0.0f, m_Data.m_fFadeOutTime);
        m_bFadingOut = true;
        m_bFadingIn = false;
    }

    /// <summary>
    /// 현재 페이드 볼륨에서 페이드 진행
    /// </summary>
    /// <param name="fTo"></param>
    /// <param name="fTime"></param>
    public void FadeTo(float fTo, float fTime)
    {
        FadeTo(FadeVolume, fTo, fTime);
    }

    public bool m_bFadingIn = false;
    public bool m_bFadingOut = false;

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
        m_Fader.StartBlending(0, 1, fTime, OnFadingFinished);
    }

    public void FadeOutAndStop()
    {
        FadeTo(FadeVolume, 0.0f, m_Data.m_fFadeOutTime);
        m_bFadingOut = true;
        m_bFadingIn = false;
        m_eFadeProcess = E_SOUND_FADE_PROCESS.STOP;
    }
    public void FadeOutAndPause()
    {
        FadeTo(FadeVolume, 0.0f, m_Data.m_fFadeOutTime);
        m_bFadingOut = true;
        m_bFadingIn = false;
        m_eFadeProcess = E_SOUND_FADE_PROCESS.PAUSE;
    }

    public void FadeInAndResume()
    {
        Resume();
        FadeTo(FadeVolume, 1.0f, m_Data.m_fFadeInTime);
        m_bFadingIn = true;
        m_bFadingOut = false;
        m_eFadeProcess = E_SOUND_FADE_PROCESS.PAUSE;
    }

    public void OnFadingFinished(MyUIBlendingModel Data)
    {
        switch (m_eFadeProcess)
        {
            case E_SOUND_FADE_PROCESS.STOP:
                if (m_bFadingOut)
                {
                    Stop();
                    m_bFadingOut = false;
                }
                else if (m_bFadingIn)
                {
                    m_bFadingIn = false;
                }
                break;
            case E_SOUND_FADE_PROCESS.PAUSE:
                if (m_bFadingOut)
                {
                    Pause();
                    m_bFadingOut = false;
                }
                else if (m_bFadingIn)
                {
                    m_bFadingIn = false;
                }
                break;
        }
    }

    /// <summary>
    /// 오디오 Reset초기화
    /// </summary>
    public void Reset()
    {
        if(audioSource)
            audioSource.clip = null;
        isPlay = false;
        audioTime = 0;
        //playStartTimeLocal = -1;
        isPause = false;
        isApplicationPause = false;
        SetAudioData(audioData);
    }

    /// <summary>
    /// 오디오 데이터를 설정한다.
    /// </summary>
    /// <param name="audioData"></param>
    public void SetAudioData(AudioData audioData)
    {
        audioVolume = audioData.Volume;
        Priority = audioData.Priority;
        MinDistance = audioData.MinDistance;
        MaxDistance = audioData.MaxDistance;
        m_bFadingIn = false;
        m_bFadingOut = false;
    }

    private void Update()
    {
        if (false == isPlay || isPause || isApplicationPause)
            return;
        
        if (false == audioSource.isPlaying)
        {
            OnComplete.Execute();

            Stop();
            return;
        }
        audioTime += SoundManager.SystemDeltaTime;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Awake()
    {
        SoundManager.AddAudioObject(this);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        SetAudioData(audioData);
    }

    private void OnDestroy()
    {
        Reset();
        OnStop = null;
        OnComplete = null;
        OnFaidOutComplete = null;

        SoundManager.RemoveAudioObject(this);
    }

    private void OnApplicationPause(bool pause)
    {
        isApplicationPause = pause;
    }
}
