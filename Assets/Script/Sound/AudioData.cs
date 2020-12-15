using System;
using UnityEngine;
using System.Collections.Generic;
using Global.Constants;

namespace Audio.Sound
{
    /// <summary>
    /// 오디오 그룹 (특정부분)
    /// 그룹의 그룹을 묶을 수 있음.
    /// </summary>
    //public class AudioGroup : EnumBaseType<AudioGroup, byte, E_AUDIO_GROUP_TYPE>
    //{
    //    public static AudioGroup InGame_BGM = new AudioGroup(21, E_AUDIO_GROUP_TYPE.InGame_BGM, InGame);
    //    public static AudioGroup InGame_MyPlayer = new AudioGroup(22, E_AUDIO_GROUP_TYPE.InGame_Player, InGame);
    //    public static AudioGroup InGame_MultiPlayer = new AudioGroup(23, E_AUDIO_GROUP_TYPE.InGame_PlayerMulti, InGame);
    //    public static AudioGroup InGame_Monster = new AudioGroup(24, E_AUDIO_GROUP_TYPE.InGame_Monster, InGame);
    //    public static AudioGroup InGame_Pet = new AudioGroup(25, E_AUDIO_GROUP_TYPE.InGame_Pet, InGame);

    //    public static AudioGroup InGame = new AudioGroup(20, E_AUDIO_GROUP_TYPE.InGame);


    //    public static AudioGroup UI_Base = new AudioGroup(11, E_AUDIO_GROUP_TYPE.UI_Base, UI);

    //    public static AudioGroup UI = new AudioGroup(10, E_AUDIO_GROUP_TYPE.UI);

    //    private AudioGroupData groupData = new AudioGroupData();
    //    private AudioGroup parentAudioGroup;

    //    public E_AUDIO_GROUP_TYPE audioGroupType { get { return Value; } }
    //    /// <summary>
    //    /// 볼륨 설정(그룹에 포함된 오디오 전부 적용)
    //    /// </summary>
    //    public float Volume
    //    {
    //        get { return parentAudioGroup == null ? groupData.Volume : groupData.Volume * parentAudioGroup.Volume; }
    //        set
    //        {
    //            groupData.Volume = value;
    //            SoundManager.ApplyVolumeAudioObject(this);
    //        }
    //    }

    //    /// <summary>
    //    /// 뮤트 설정(그룹에 포함된 오디오 전부 적용)
    //    /// </summary>
    //    public bool Mute
    //    {
    //        get { return parentAudioGroup == null ? groupData.Mute : groupData.Mute || parentAudioGroup.Mute; }
    //        set
    //        {
    //            groupData.Mute = value;
    //            SoundManager.MuteAudioObject(this);
    //        }
    //    }

    //    public void SetParent(AudioGroup audioGroup)
    //    {
    //        this.parentAudioGroup = audioGroup;

    //        SoundManager.ApplyVolumeAudioObject(this);
    //        SoundManager.MuteAudioObject(this);
    //    }

    //    public AudioGroup(byte key, E_AUDIO_GROUP_TYPE groupType, AudioGroup parent = null, float volume = 1.0f, bool mute = false) : base(key, groupType)
    //    {
    //        groupData = new AudioGroupData()
    //        {
    //            Volume = volume,
    //            Mute = mute
    //        };
    //        SetParent(parent);
    //    }

    //    public void SetData(AudioGroupData groupData)
    //    {
    //        this.groupData = groupData;

    //        SoundManager.ApplyVolumeAudioObject(this);
    //        SoundManager.MuteAudioObject(this);
    //    }

    //    /// <summary>
    //    /// 자기자신과 부모의 오디오그룹을 가지고 있는 확인한다.
    //    /// </summary>
    //    /// <param name="thisAudioGroup"></param>
    //    /// <param name="findAudioGroup"></param>
    //    /// <returns></returns>
    //    static public bool HasSelfParent(AudioGroup thisAudioGroup, AudioGroup findAudioGroup)
    //    {
    //        if (thisAudioGroup == null)
    //            return false;
    //        if (thisAudioGroup == findAudioGroup)
    //            return true;

    //        return HasSelfParent(thisAudioGroup.parentAudioGroup, findAudioGroup);
    //    }

    //    static public AudioGroup GetAudioGroup(E_AUDIO_GROUP_TYPE audioGroupType)
    //    {
    //        foreach(var audioGroup in GetBaseValues())
    //        {
    //            if (audioGroup.audioGroupType == audioGroupType)
    //                return audioGroup;
    //        }
    //        return null;
    //    }
    //}
    /// <summary>
    /// 오디오의 초기데이터
    /// </summary>
    [Serializable]
    public class AudioData
    {
        public E_AUDIO_CHANNEL_TYPE OptionType = E_AUDIO_CHANNEL_TYPE.SE;
        public E_AUDIO_GROUP_TYPE GroupType = E_AUDIO_GROUP_TYPE.UI;
        public float Volume = 1.0f;
        public float MinDistance = 1.5f;
        public float MaxDistance = 10.5f;
        /// <summary>
        /// Sets the AudioSource spatialBlend value (0=2D 1=3D)
        /// </summary>
        public float spatialBlend = 0f;
        public int Priority = 128;
    }
    /// <summary>
    /// 오디오 그룹 데이터
    /// </summary>
    [Serializable]
    public class AudioGroupData
    {
        public float Volume = 1.0f;
        public bool Mute = false;
        public int Priority = 128;
    }

    /// <summary>
    /// 오디오클립 데이터(애니메이션(어떤 이름)에 따른)
    /// [] 오디오클립이 여러개 처리할 경우.
    /// </summary>
    [Serializable]
    public class AudioClipAnimationConfig
    {
        public string animationName = "";
        public AudioClipConfig[] audioClipDatas;
    }
    [Serializable]
    public class AudioClipConfig
    {
        public int SoundID = -1;
        public string SoundName = "";
        public float Delay = 0f;
        public float Volume = 1.0f;
    }
    
    /// <summary>
    /// 오디오클립 로더 클래스.
    /// </summary>
    public class SoundLoader
    {
        public List<SoundPlayData> m_PlayDataList = new List<SoundPlayData>();
        public AsyncOperation m_Request;
        public float m_StartTime;
        public string m_strName;

        public SoundLoader(SoundPlayData m_PlayData, AsyncOperation request)
        {
            AddPlayData(m_PlayData);
            this.m_Request = request;
            this.m_StartTime = Time.time;
            m_strName = m_PlayData.m_strSoundFileName;
        }

        public void AddPlayData(SoundPlayData m_PlayData)
        {
            m_PlayDataList.Add(m_PlayData);
        }

        public bool ProcessCheckLoad()
        {
            bool Done = IsDone();
            if (Done)
            {
                ProcessPlay();
            }
            return Done;
        }

        public bool IsDone()
        {
            return m_Request.isDone;
        }

        public void ProcessPlay()
        {
            UnityEngine.Object Asset = null;
            if (m_Request is AssetBundleRequest)
            {
                AssetBundleRequest LoaderRequest = this.m_Request as AssetBundleRequest;
                Asset = LoaderRequest.asset;

            }
            else if (this.m_Request is ResourceRequest)
            {
                ResourceRequest LoaderRequest = this.m_Request as ResourceRequest;
                Asset = LoaderRequest.asset;
            }

            if (Asset)
            {
                AudioClip TargetClip = Asset as AudioClip;
                float fDelay = Time.time - this.m_StartTime;
                for (int i = 0; i < m_PlayDataList.Count; i++)
                {
                    if (i == 0)
                        SoundManager.Instance.m_Storage.AddAudioClip(TargetClip);

                    m_PlayDataList[i].m_AudioClipGroup.PushAudioClip(TargetClip);
                    m_PlayDataList[i].LoadComplete(TargetClip, fDelay);
                    m_PlayDataList[i].TryPlay();

#if UNITY_EDITOR
                    if (i == 0)
                        Debugs.Log(string.Concat("[사운드로드] 완료! ", m_PlayDataList[i].m_LoadedAudioClip.name, " 소요 시간 [", fDelay, "]"));
#endif
                }
            }
        }
    }
}