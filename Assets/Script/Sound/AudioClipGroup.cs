using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

/// <summary>
/// 이미 로드된 오디오 클립을 분류하여 저장소에서 꺼내와 보관하는 장소입니다.
/// </summary>
public class AudioClipGroup {

    public static AudioClipGroup Global = new AudioClipGroup("Global", 9999);
    public static AudioClipGroup Map = new AudioClipGroup("Map", 9999);
    public static AudioClipGroup Bgm_Map = new AudioClipGroup("Bgm_Map", 2);
    public static AudioClipGroup Bgm_Special = new AudioClipGroup("Bgm_Special", 2);
    public static AudioClipGroup UI = new AudioClipGroup("UI", 12);
    public static AudioClipGroup Stop = new AudioClipGroup("UI", 0);

    private static List<string> m_PlayingClipList = new List<string>();

    public static void RecordPlayingClip(AudioClip Clip)
    {
        if (!m_PlayingClipList.Contains(Clip.name))
            m_PlayingClipList.Add(Clip.name);
    }

    public static void ErasePlayingClip(AudioClip Clip)
    {
        m_PlayingClipList.Remove(Clip.name);
    }

    public static bool IsPlayingClip(AudioClip Clip)
    {
        return m_PlayingClipList.Contains(Clip.name);
    }

    public static void ClaerPlayingClip()
    {
        m_PlayingClipList.Clear();
    }

    public List<AudioClip> m_ClipList = new List<AudioClip>();
    public int m_nCacheCount;
    public string m_strGroupName;


    public AudioClipGroup(string strGroupName, int nCacheCount)
    {
        m_strGroupName = strGroupName;
        m_nCacheCount = nCacheCount;
    }

    public void PushAudioClip(AudioClip Clip)
    {
        if (m_ClipList.Find(x => x.name == Clip.name) == null)
        {
            m_ClipList.Insert(0, Clip);
            SoundManager.Instance.m_Storage.IncreaseGroupRequestCount(Clip.name);
        }

        Collect();

        PrintLog();
    }

    public void Collect()
    {
        if (m_ClipList.Count > m_nCacheCount)
        {
            if (m_ClipList.Count > 0)
            {
                for (int i = m_ClipList.Count - 1; i > 0; i--)
                {
                    AudioClip TargetClip = m_ClipList[i];
                    if (!AudioClipGroup.IsPlayingClip(TargetClip))
                    {
                        SoundManager.Instance.m_Storage.GiveupAudioClip(TargetClip);
                        m_ClipList.Remove(TargetClip);
                        TargetClip = null;
                    }

                    if (m_ClipList.Count <= m_nCacheCount)
                        break;
                }
            }
        }
    }

    public void OnStop(AudioClip TargetClip)
    {
        AudioClip SearchClip = m_ClipList.Find(x => x.name == TargetClip.name);
        if (SearchClip != null)
        {
            if(SoundManager.Instance != null)
                SoundManager.Instance.m_Storage.GiveupAudioClip(SearchClip);
            m_ClipList.Remove(SearchClip);
        }
    }

    public bool PopAudioClip()
    {
        AudioClip TargetClip = m_ClipList[m_ClipList.Count - 1];
        if(AudioClipGroup.IsPlayingClip(TargetClip))
        {
            return false;
        }

        SoundManager.Instance.m_Storage.GiveupAudioClip(TargetClip);
        m_ClipList.Remove(TargetClip);

        PrintLog();
        return true;
    }

    public void ClearAudioClip()
    {
        var soundManager = SoundManager.Instance;
        if (soundManager != null)
        {
            for (int i = 0; i < m_ClipList.Count; i++)
            {
                AudioClip TargetClip = m_ClipList[i];
                soundManager.m_Storage.GiveupAudioClip(TargetClip);
            }
        }
        m_ClipList.Clear();
    }


    /// <summary>
    /// 클립이 있는지 질의합니다. (1) 클립 그룹에 있으면 꺼내옵니다. (2) 저장소에 있다면 꺼내옵니다. (3) 없다면 false (로드시작)
    /// </summary>
    /// <param name="strClipName"></param>
    /// <param name="TargetClip"></param>
    /// <returns></returns>
    public bool HasAudioClip(string strClipName, out AudioClip TargetClip)
    {
        bool HasClip = false;
        AudioClip SearchClip = m_ClipList.Find(x => x.name == strClipName);
        HasClip = (SearchClip != null);
        if (!HasClip)
        {
            SearchClip = SoundManager.Instance.m_Storage.RequestAudioClip(strClipName);
            HasClip = (SearchClip != null);
            if (HasClip)
            {
                PushAudioClip(SearchClip);
            }
        }
        TargetClip = SearchClip;

        if (HasClip)
        {
            ///사용한 클립을 맨 위로 올립니다.
            m_ClipList.Remove(SearchClip);
            m_ClipList.Insert(0, SearchClip);
            PrintLog();
        }
        return HasClip;
    }

    public void PrintLog()
    {
        //StringBuilder strbLog = new StringBuilder();
        //strbLog.AppendLine("[오디오 그룹 : " + m_strGroupName + "]("+ m_ClipList.Count + ")");

        //for (int i = 0; i < m_ClipList.Count; i++)
        //{
        //    strbLog.AppendLine(m_ClipList[i].name);
        //}
        //Debug.Log(strbLog.ToString());
    }
}

public enum E_AUDIO_CLIP_GROUP
{
    Global,
    Map,
    Stop,
    Bgm_Map,
    Bgm_Special,
    UI,
}