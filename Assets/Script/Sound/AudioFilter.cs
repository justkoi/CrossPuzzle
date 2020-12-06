using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Global.Constants;

/// <summary>
/// 중복 재생되는 같은 오디오 클립의 출력을 기록합니다.
/// 필터에 존재한다면 중복 재생이 불가합니다.
/// 버림과 동시에 현재 존재하는 카운트만큼 볼륨을 키울지 말지 생각해보자...
/// </summary>
public class AudioFilter {

    private CustomDictionary<string, AudioFilterInfo> m_FilterInfoList = new CustomDictionary<string, AudioFilterInfo>();
    /// <summary>
    /// 필터 시간 (예 0.1초면 0.1초안에 들어온 같은 오디오 클립의 재생을 버림)
    /// </summary>
    private float m_fTime;

    public AudioFilter(float fTime)
    {
        m_fTime = fTime;
    }

    /// <summary>
    /// 해당 오디오 클립기록이 있는지 검사하여 버릴지 말지 결정 (true면 버림, false면 재생)
    /// </summary>
    public bool CheckFiltering(string strKey)
    {
        return m_FilterInfoList.ContainsKey(strKey);
    }

    /// <summary>
    /// 새로운 오디오 클립의 필터 정보를 기록합니다.
    /// 이미 기록되어있는경우 필터의 카운트를 증가시킵니다.
    /// </summary>
    /// <param name="FilterInfo"></param>
    public void RecordFilterInfo(AudioFilterInfo FilterInfo)
    {
        string strKey = FilterInfo.m_strFilterKey;
        if (m_FilterInfoList.ContainsKey(strKey))
        {
            m_FilterInfoList[strKey].m_nCount++;
        }
        else
        {
            m_FilterInfoList.Add(strKey, FilterInfo);
        }
    }

    /// <summary>
    /// 오디오 클립의 필터 정보를 제거합니다.
    /// </summary>
    /// <param name="FilterInfo"></param>
    public void RemoveFilterInfo(AudioFilterInfo FilterInfo)
    {
        if (m_FilterInfoList.ContainsKey(FilterInfo.m_strFilterKey))
        {
            m_FilterInfoList.Remove(FilterInfo.m_strFilterKey);
        }
    }

    /// <summary>
    /// 시간 누적 및 기록 제거
    /// </summary>
    public void LateUpdate()
    {
        for (int i = 0; i < m_FilterInfoList.Count; i++)
        {
            m_FilterInfoList.ElementAt(i).m_fTimer += Time.deltaTime;
            if (m_FilterInfoList.ElementAt(i).m_fTimer >= m_fTime)
            {
                RemoveFilterInfo(m_FilterInfoList.ElementAt(i));
            }
        }
    }
}

public class AudioFilterInfo
{
    public string m_strFilterKey;
    public float m_fTimer;
    public int m_nCount;

    public AudioFilterInfo(string strFilterKey)
    {
        m_strFilterKey = strFilterKey;
        m_fTimer = 0.0f;
        m_nCount = 1;
    }
}
