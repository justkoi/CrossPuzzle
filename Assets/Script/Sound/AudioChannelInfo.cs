#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class SimpleAudioChannelInfo
{
    public string m_strKey;
    public Transform m_trObject;
    public AudioObject m_AudioObject;
    public SimpleAudioChannelInfo(string strKey, Transform trObject, AudioObject AudioObject)
    {
        m_strKey = strKey;
        m_trObject = trObject;
        m_AudioObject = AudioObject;
    }
}
/// <summary>
/// 에디터의 인스펙터창에서 확인할 수 있도록 추가
/// </summary>
public class AudioChannelInfo : MonoBehaviour {
    
    public List<SimpleAudioChannelInfo> m_AudioObjectList = new List<SimpleAudioChannelInfo>();
}
#endif