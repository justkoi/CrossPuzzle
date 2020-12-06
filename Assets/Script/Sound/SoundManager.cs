//#define INDEPENDENT

using System;
using System.Collections;
using System.Collections.Generic;
using Global.Constants;
using UnityEngine;
/// <summary>
/// Import 설정
/// ForceToMono 체크v
/// [ 이펙트 ]
/// PreloadAudioData 체크v, CompressionForMat PCM(메모리 많이,프레임 줄임) 설정. 메모리를 많이 잡아 먹는 경우 Vorbis설정후 퀄리티를 수정해준다.
/// [ 배경음(mp3) ]
/// LoadType : Streamming으로 설정한다. ForceToMono 언체크(스테레오)하는데 메모리를 많이 차지하면 체크한다.(강제 모노)=>Stream모드 끊기는 현상이 있을경우에는 Compressed In Memory를 사용한다.
/// LoadInBackground를 체크한다.(엄격하게 재생타이밍 맞출 필요가 없는경우.)
/// </summary>
public class SoundManager : Singleton<SoundManager>
{

    #region 에디터 인터페이스
#if UNITY_EDITOR
    /// <summary>
    /// 활성화 된 오디오 오브젝트의 수
    /// </summary>
    public int EditOnly_g_nActiveAudioObjectCount = 0;
#endif
    #endregion

    public bool g_bPrintLog = false;

    public AudioClipStorage m_Storage = new AudioClipStorage();
    public AudioClipLoader m_Loader = new AudioClipLoader();
    private Dictionary<E_AUDIO_CHANNEL_TYPE, AudioChannel> m_AudioChannelList = new Dictionary<E_AUDIO_CHANNEL_TYPE, AudioChannel>(new E_AUDIO_CHANNEL_TYPEComparer());

    internal void RemoveListener()
    {
        if(this.GetComponent<AudioListener>() != null)
        {
            Destroy(this.GetComponent<AudioListener>());
        }
    }

    private Dictionary<E_AUDIO_GROUP_TYPE, AudioGroup> m_AudioGroupList = new Dictionary<E_AUDIO_GROUP_TYPE, AudioGroup>(new E_AUDIO_GROUP_TYPEComparer());


    static private double globalTimeAtLanch = globalTime;
    static public double globalTime
    {
        get
        {
            const double ticks2seconds = 1 / (double)TimeSpan.TicksPerSecond;
            long ticks = DateTime.Now.Ticks;
            double seconds = ((double)ticks) * ticks2seconds;
            return seconds;
        }
    }
    static public double globalTimeSinceLaunch { get { return globalTime - globalTimeAtLanch; } }

    static private double systemDeltaTime;
    static public double SystemDeltaTime { get { return systemDeltaTime; } }

    static private double lastSystemTime = -1;
    static public double systemTime;

    private List<AudioObject> audioObjectList = new List<AudioObject>();
    
    public AudioListener audioListener;

    public Transform m_trGroup;
    public Transform m_trChannel;

    public AudioChannel AudioChannelTemplate;
    /*
    private AudioChannel AudioChannelTemplate
    {
        get
        {
            if (_AudioChannelTemplate == null)
            {
                _AudioChannelTemplate = BundlePatchManager.Instance.LocalResourcesLoad<AudioChannel>("Prefabs/Common/Sound/AudioChannelPrefab");
                _AudioChannelTemplate.transform.localPosition = Vector3.zero;
            }
            return _AudioChannelTemplate;
        }
    }*/

    public AudioGroup AudioGroupTemplate;
    /*
    private AudioGroup AudioGroupTemplate
    {
        get
        {
            if (_AudioGroupTemplate == null)
            {
                _AudioGroupTemplate = BundlePatchManager.Instance.LocalResourcesLoad<AudioGroup>("Prefabs/Common/Sound/AudioGroupPrefab");
                _AudioGroupTemplate.transform.localPosition = Vector3.zero;
            }
            return _AudioGroupTemplate;
        }
    }
    */

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(transform.root.gameObject);
        //AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        m_Loader.Init(m_Storage);
        InitBaseAudio();
    }
    
    public void InitAudioChannel()
    {
        ///동시채널의 경우 넉넉하게 설정해도 하드웨어 출력 우선순위 부여에 따라 출력이 결정됩니다.
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map, 2, 5, 0.0f); ///3개 이상의 채널 지원안함...
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL, 3, 10, 0.0f); ///빠른 전환 시 부드럽게 쓰고싶다면 3개까지
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.SE, 16, 206, 1.0f);
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE, 8, 256, 0.0f);
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_3D, 12, 156, 1.0f);
        AddAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_2D, 4, 106, 0.0f);

        // 여기에서 환경설정 볼륨을 가져오십시오.
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).m_fVolume = 1.0f;// (float)(UserGameSettings.BgmVolume / 100.0f);
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL).m_fVolume = 1.0f;//(float)(UserGameSettings.BgmVolume / 100.0f);
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.SE).m_fVolume = 1.0f;//(float)(UserGameSettings.SfxVolume / 100.0f);
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE).m_fVolume = 1.0f;//(float)(UserGameSettings.SfxVolume / 100.0f);
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_2D).m_fVolume = 1.0f;//(float)(UserGameSettings.Cv2DVolume / 100.0f);
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_3D).m_fVolume = 1.0f;//(float)(UserGameSettings.Cv3DVolume / 100.0f);

        //여기에서 볼륨에 대한 보정값을 적용하십시오.
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).m_fStandardVolume = 0.9f;
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL).m_fStandardVolume = 0.9f;
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.SE).m_fStandardVolume = 0.95f;
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE).m_fStandardVolume = 0.9f;
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_3D).m_fStandardVolume = 1.0f;
        GetAudioChannel(E_AUDIO_CHANNEL_TYPE.CV_2D).m_fStandardVolume = 1.0f;

    }

    public void AddAudioChannel(E_AUDIO_CHANNEL_TYPE eChannelType, int nMaxAudio, int nPriority, float fSpatialBlend = 1.0f)
    {
        if (!m_AudioChannelList.ContainsKey(eChannelType))
        {
            AudioChannel NewChannel = Instantiate<AudioChannel>(AudioChannelTemplate);
            NewChannel.gameObject.name = string.Concat(eChannelType.ToString(), " Auidio Channel");
            NewChannel.gameObject.transform.SetParent(m_trChannel);
            NewChannel.Init(eChannelType, nMaxAudio, NewChannel.transform, fSpatialBlend);
            NewChannel.SetPriority(nPriority);
            m_AudioChannelList.Add(eChannelType, NewChannel);
        }
    }

    public void InitAudioGroup()
    {
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame_BGM);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Monster);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Pet);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Player);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.InGame_PlayerMulti);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.UI);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.UI_Base);
        AddAudioGroup(E_AUDIO_GROUP_TYPE.UI_CV);

        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_BGM).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame));
        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Monster).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame));
        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Pet).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame));
        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_Player).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame));
        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_PlayerMulti).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame));

        GetAudioGroup(E_AUDIO_GROUP_TYPE.UI_Base).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.UI));
        GetAudioGroup(E_AUDIO_GROUP_TYPE.UI_CV).SetParentGroup(GetAudioGroup(E_AUDIO_GROUP_TYPE.UI));

        ///각 그룹별 볼륨을 여기에서 조정하십시오.
        GetAudioGroup(E_AUDIO_GROUP_TYPE.InGame_PlayerMulti).m_fVolume = 0.7f;
    }

    public void AddAudioGroup(E_AUDIO_GROUP_TYPE eGroupType)
    {
        if (!m_AudioGroupList.ContainsKey(eGroupType))
        {
            AudioGroup NewGroup = Instantiate<AudioGroup>(AudioGroupTemplate);
            NewGroup.gameObject.name = string.Concat(eGroupType.ToString(), " Auidio Group");
            NewGroup.gameObject.transform.SetParent(m_trGroup);
            NewGroup.Init(eGroupType);
            m_AudioGroupList.Add(eGroupType, NewGroup);
        }
    }

    public AudioGroup GetAudioGroup(E_AUDIO_GROUP_TYPE eGroupType)
    {
        if (!m_AudioGroupList.ContainsKey(eGroupType))
            return null;

        return m_AudioGroupList[eGroupType];
    }

    public AudioChannel GetAudioChannel(E_AUDIO_CHANNEL_TYPE eChannelType)
    {
        if (!m_AudioChannelList.ContainsKey(eChannelType))
            return null;

        return m_AudioChannelList[eChannelType];
    }

    public bool AvaliableAudioChannel(E_AUDIO_CHANNEL_TYPE eChannelType)
    {
        if (!m_AudioChannelList.ContainsKey(eChannelType))
            return false;

        if (!m_AudioChannelList[eChannelType].m_bActive)
            return false;

        return true;
    }
    public Transform GetAudioChannelRoot(E_AUDIO_CHANNEL_TYPE eChannelType)
    {
        if (!m_AudioChannelList.ContainsKey(eChannelType))
        {
            Debugs.LogError("[사운드] 대상 오디오 채널", eChannelType.ToString(), "이 없습니다!");
            return null;
        }

        return m_AudioChannelList[eChannelType].m_trTransform;
    }

    /// <summary>
    /// 오디오 설정 변경 발생시
    /// 이어폰이나 HDMI 등을 연결시 발생.
    /// </summary>
    /// <param name="deviceWasChanged"></param>
    void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        int bufferLength, numBuffers;
        AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
        AudioConfiguration config = AudioSettings.GetConfiguration();
        string m_audio_info = string.Format("Audio : {0 : #, #} Hz {1} {2} samples {3} buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers);
        Debugs.Log("[AudioInfo]", m_audio_info);
    }

    void Update()
    {
        updateSystemTime();
    }

    void LateUpdate()
    {
        m_Loader.LateUpdate();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    void onReset()
    {
        m_Loader.ClearSoundLoaders();
        AudioClipGroup.Global.ClearAudioClip();
        AudioClipGroup.Map.ClearAudioClip();
        AudioClipGroup.Bgm_Map.ClearAudioClip();
        AudioClipGroup.Bgm_Special.ClearAudioClip();
        AudioClipGroup.UI.ClearAudioClip();
        m_Storage.ClearAudioClip();
        AudioClipGroup.ClaerPlayingClip();
    }
    


    #region 기본 오디오 설정.

    public void AddAudioListener()
    {
        if (audioListener == null)
        {
            audioListener = this.gameObject.GetComponent<AudioListener>();
            if (audioListener == null)
                audioListener = this.gameObject.AddComponent<AudioListener>();
        }
    }
    private void InitBaseAudio()
    {
        AddAudioListener();
        audioListener.enabled = true;

        //music1Audio = CreateAudioObject(eAudioOptionType.BGM, eAudioGroupType.InGame_BGM);
        //music1Audio.Priority = 1;
        //music2Audio = CreateAudioObject(eAudioOptionType.BGM, eAudioGroupType.InGame_BGM);
        //music2Audio.Priority = 1;
        //curMusicAudio = music1Audio;


        //publicSFXAudio = CreateAudioObject(eAudioOptionType.SE, eAudioGroupType.UI);

        InitAudioChannel();
        InitAudioGroup();

    }

    #endregion 기본 오디오 설정.

    protected void OnDestroy()
    {
        AudioChannelTemplate = null;
        onReset();
        //base.OnDestroy();
    }

    static private void updateSystemTime()
    {
        double newSystemTime = globalTimeSinceLaunch;
        if (lastSystemTime >= 0)
        {
            systemDeltaTime = newSystemTime - lastSystemTime;
            if (systemDeltaTime <= Time.maximumDeltaTime + 0.01f)
            {
                systemTime += systemDeltaTime;
            }
            else
            {
                systemDeltaTime = 0;
            }
        }
        else
        {
            systemDeltaTime = 0f;
            systemTime = 0f;
        }
        lastSystemTime = newSystemTime;
    }


    #region Async Loader

    static private string m_strLocalLoad = "Sound/";
    

    #endregion Async Loader
    ///// <summary>
    ///// 오디오 그룹에 따라 볼륨을 조절한다.(부모 포함)
    ///// </summary>
    ///// <param name="group"></param>
    //static public void ApplyVolumeAudioObject(AudioGroup group)
    //{
    //    List<AudioObject> aoList = Instance.audioObjectList.FindAll(v => AudioGroup.HasSelfParent(v.AudioGroup, group));
    //    for (int i = 0; i < aoList.Count; ++i)
    //    {
    //        aoList[i].ApplyVolume();
    //    }
    //}
    ///// <summary>
    ///// 오디오 그룹에 따라 Mute를 설정한다.(부모 포함)
    ///// </summary>
    ///// <param name="group"></param>
    //static public void MuteAudioObject(AudioGroup group)
    //{
    //    List<AudioObject> aoList = Instance.audioObjectList.FindAll(v => AudioGroup.HasSelfParent(v.AudioGroup, group));
    //    for (int i = 0; i < aoList.Count; ++i)
    //    {
    //        aoList[i].ApplyMute();
    //    }
    //}
    /// <summary>
    /// 오디오를 리스트에서 추가한다.
    /// </summary>
    /// <param name="ao"></param>
    static public void AddAudioObject(AudioObject ao)
    {
        Instance.audioObjectList.Add(ao);

        Debugs.Log("[AudioObject.Add] Count:", Instance.audioObjectList.Count);
    }
    /// <summary>
    /// 오디오를 리스트에서 삭제한다.
    /// </summary>
    /// <param name="ao"></param>
    static public void RemoveAudioObject(AudioObject ao)
    {
        if (ao == null || Instance == null || Instance.audioObjectList == null)
            return;
        Instance.audioObjectList.Remove(ao);

        Debugs.Log("[AudioObject.Remove] Count:", Instance.audioObjectList.Count);
    }

    /// <summary>
    /// 해당 클립을 플레이(보유) 하는 오디오를 찾는다.
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public static AudioObject FindAudioObject(string clipName)
    {
        return Instance.audioObjectList.Find(v => v.curAudioClip && v.curAudioClip.name == clipName);
    }

    /// <summary>
    /// 오디오 오브젝트를 생성한다.
    /// </summary>
    /// <param name="groupType"></param>
    /// <param name="optionType"></param>
    /// <returns></returns>
    //public static AudioObject CreateAudioObject(eAudioOptionType optionType = eAudioOptionType.SE, eAudioGroupType groupType = eAudioGroupType.UI)
    //{
    //    return CreateAudioObject(Instance.gameObject.transform, optionType, groupType);
    //}

    //public static AudioObject CreateAudioObject(Transform parent, eAudioOptionType optionType = eAudioOptionType.SE, eAudioGroupType groupType = eAudioGroupType.UI)
    //{
    //    AudioObject NewAudioObject = Instantiate(Instance.AudioObjectPrefab);
    //    NewAudioObject.SetParent(parent);
    //    NewAudioObject.transform.localPosition = Vector3.zero;
    //    NewAudioObject.SetAudioChannel(optionType);
    //    NewAudioObject.SetAudioGroup(groupType);
    //    return NewAudioObject;
    //}


    #region Play (SoundPlayData모델)

    /// <summary>
    /// 최종 사운드 재생
    /// </summary>
    /// <param name="Data"></param>
    public void Play(SoundPlayData Data)
    {
        if (Data == null)
        {
            Debugs.LogError("[사운드-재생] 이 사운드 재생 데이터는 NULL입니다!");
            return;
        }

        if (!AvaliableAudioChannel(Data.m_eAudioChannel))
        {
            Debugs.LogError("[사운드-재생] 재생할 채널이 없거나 비활성화 중입니다!");
            return;
        }
        
        m_AudioChannelList[Data.m_eAudioChannel].Play(Data);
    }

    /// <summary>
    /// 해당 객체의 해당 채널의 해당 오디오를 중지합니다.
    /// </summary>
    /// <param name="trPlace"></param>
    /// <param name="eAudioChannel"></param>
    /// <param name="strKey"></param>
    public void Stop(Transform trPlace, E_AUDIO_CHANNEL_TYPE eAudioChannel, string strKey)
    {
        if (!AvaliableAudioChannel(eAudioChannel))
        {
            if (g_bPrintLog)
                Debugs.LogError("[사운드-중지] 중지할 채널이 없거나 비활성화 중입니다!");
            return;
        }
        m_AudioChannelList[eAudioChannel].Stop(string.Concat(trPlace.GetInstanceID(), strKey));
    }
    /// <summary>
    /// 해당 채널의 해당 오디오를 중지합니다. (비용이 거의 없음)
    /// </summary>
    /// <param name="eAudioChannel"></param>
    /// <param name="strKey"></param>
    public void Stop(E_AUDIO_CHANNEL_TYPE eAudioChannel, string strKey)
    {
        if (!AvaliableAudioChannel(eAudioChannel))
        {
            if (g_bPrintLog)
                Debugs.LogError("[사운드-중지] 중지할 채널이 없거나 비활성화 중입니다!");
            return;
        }

        m_AudioChannelList[eAudioChannel].Stop(string.Concat(m_AudioChannelList[eAudioChannel].m_trTransform.GetInstanceID(), strKey));
    }

    /// <summary>
    /// 해당 채널의 모든 오디오를 중지합니다.
    /// </summary>
    /// <param name="eAudioChannel"></param>
    public void StopAll(E_AUDIO_CHANNEL_TYPE eAudioChannel)
    {
        if (!AvaliableAudioChannel(eAudioChannel))
        {
            if (g_bPrintLog)
                Debugs.LogError("[사운드-중지] 중지할 채널이 없거나 비활성화 중입니다!");
            return;
        }
        m_AudioChannelList[eAudioChannel].StopAll();
    }

    /// <summary>
    /// 해당 채널의 해당 객체의 모든 오디오를 중지합니다.
    /// </summary>
    /// <param name="trPlace"></param>
    /// <param name="eAudioChannel"></param>

    public void StopAll(Transform trPlace, E_AUDIO_CHANNEL_TYPE eAudioChannel)
    {
        if (!AvaliableAudioChannel(eAudioChannel))
        {
            if (g_bPrintLog)
                Debugs.LogError("[사운드-중지] 중지할 채널이 없거나 비활성화 중입니다!");
            return;
        }
        m_AudioChannelList[eAudioChannel].StopAll(trPlace);
    }

    /// <summary>
    /// 해당 객체의 모든 채널의 모든 오디오를 중지합니다. (객체별로 기억하지는 않기때문에 비용이 아주 조금 있다.(활성화된 오브젝트중에서 객체 비교하는식 일반적으로 활성화된 오브젝트는 한자리수이내))
    /// </summary>
    /// <param name="trPlace"></param>
    public void StopAll(Transform trPlace)
    {
        for (int i = 0; i < (int)E_AUDIO_CHANNEL_TYPE.BGM_SPECIAL + 1; i++)
        {
            StopAll(trPlace, (E_AUDIO_CHANNEL_TYPE)i);
        }
    }


    //public void Play(int nSoundId, Transform trParent, eAudioGroupType eAudioGroup, bool bLocal = false, int nIndex = 0)
    //{
    //    SoundData Data = SoundDataManager.Instance.GetSoundDataByID(nSoundId);
    //    if (Data == null) return;

    //    Play(Data.name, trParent, eAudioGroup, Data.m_eChannelType, bLocal, nIndex);
    //}

    //public void Play(string strSoundName, Transform trParent, eAudioGroupType eAudioGroup, E_AUDIO_CHANNEL_TYPE eChannelType, bool bLocal = false, int nIndex = 0)
    //{
    //    if (m_AudioChannelList.ContainsKey(eChannelType))
    //    {
    //        AudioObject NewAudioObject = m_AudioChannelList[eChannelType].GetAudioObject();
    //        NewAudioObject.SetParent(trParent);
    //        NewAudioObject.transform.localPosition = Vector3.zero;
    //        NewAudioObject.SetAudioOptionType(eAudioOptionType.BGM);
    //        NewAudioObject.SetAudioGroup(eAudioGroup);

    //        LoadAsyncAddCachClipData(NewAudioObject.name, (clip, loadDelay) =>
    //        {
    //            publicSFXAudio.Play(clip);
    //            publicSFXAudio.OnStop = () => {
    //                ClipCachData data = GetCachClipData(clip.name);
    //                if (data == null)
    //                    return;
    //                RemoveClipCachData(data);
    //            };
    //        }, E_AUDIO_CLIP_GROUP.Stop, bLocal);

    //    }
    //}

    public void Pause(Transform trPlace, E_AUDIO_CHANNEL_TYPE eAudioChannel, string strKey)
    {
        if (!AvaliableAudioChannel(eAudioChannel))
        {
            if (g_bPrintLog)
                Debugs.LogError("[사운드-일시정지] 일시정지 할 채널이 없거나 비활성화 중입니다!");
            return;
        }
        m_AudioChannelList[eAudioChannel].Pause(string.Concat(trPlace.GetInstanceID(), strKey));
    }
    public void Pause(E_AUDIO_CHANNEL_TYPE eAudioChannel, string strKey)
    {
        this.Pause(m_AudioChannelList[eAudioChannel].m_trTransform, eAudioChannel, strKey);
    }

    public int m_nCurrMapBGM = 0;
    private AudioObject m_CurrBGM
    {
        get
        {
            return GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).GetRecentPlayingAudio();
        }
    }

    private void RecordMapBGM(int nId)
    {
        m_nCurrMapBGM = nId;
        Debugs.Log("[사운드-BGM] 새로운 BGM(", nId.ToString(), ") 감지");
    }

    public void PauseMapBGM()
    {
        if (m_CurrBGM != null)
            m_CurrBGM.FadeOutAndPause();
    }

    public void RePlayMapBGM()
    {
        if (m_CurrBGM != null)
        {
            m_CurrBGM.FadeInAndResume();
        }
    }

    #endregion
}