using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 이미 로드된 오디오 클립을 1차적으로 실제 보관하는 장소입니다. (최종적으로 모든 보관 및 제거는 이곳을 통합니다.)
/// </summary>
public class AudioClipStorage
{
    public List<AudioClip> m_ClipList = new List<AudioClip>();
    public Dictionary<string, int> m_GroupRequestCount = new Dictionary<string, int>();

    public void AddAudioClip(AudioClip Clip)
    {
        if (m_ClipList.Find(x=>x.name == Clip.name) == null)
        {
            m_ClipList.Add(Clip);
        }
    }
    
    public void ClearAudioClip()
    {
        m_ClipList.Clear();
        m_GroupRequestCount.Clear();
    }

    public AudioClip RequestAudioClip(string strClipName)
    {
        AudioClip SearchClip = m_ClipList.Find(x => x.name == strClipName);
        return SearchClip;
    }

    public void GiveupAudioClip(AudioClip TargetClip)
    {
        DecreaseGroupRequestCount(TargetClip.name);
    }

    public void IncreaseGroupRequestCount(string strClipName)
    {
        if (m_GroupRequestCount.ContainsKey(strClipName))
        {
            m_GroupRequestCount[strClipName]++;
        }
        else
        {
            m_GroupRequestCount.Add(strClipName, 1);
        }
    }

    private void DecreaseGroupRequestCount(string strClipName)
    {
        if (m_GroupRequestCount.ContainsKey(strClipName))
        {
            m_GroupRequestCount[strClipName]--;
            if (m_GroupRequestCount[strClipName] <= 0)
            {
                AudioClip TargetClip = m_ClipList.Find(x => x.name == strClipName);
                m_ClipList.Remove(TargetClip);
                TargetClip = null;
            }
        }
    }
}
