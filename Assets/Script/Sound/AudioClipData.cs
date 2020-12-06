using UnityEngine;
using System.Collections;

/// <summary>
/// 오디오 클립 입니다.
/// </summary>
public class AudioClipData {
    public AudioClip m_ClipData;
    public int m_nPeekCount;

    public AudioClipData(AudioClip Data)
    {
        m_ClipData = Data;
        m_nPeekCount = 0;
    }
}
