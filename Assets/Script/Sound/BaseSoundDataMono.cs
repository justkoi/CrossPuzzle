using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 기존 클래스....( 삭제할 예정.......)
// ClipObject로 대체...


public class BaseSoundDataMono : MonoBehaviour
{
	public List<BaseSoundGroupData> audioGroupDataList = new List<BaseSoundGroupData>();

	[Serializable]
	public class BaseSoundGroupData
	{
		public string soundName;
		public List<BaseSoundData> audioDataList;
	}
	
	[Serializable]
	public class BaseSoundData
	{
		public float DelayTime = 0f;
		public bool Loop = false;
		public int SoundID = 0;
		public AudioClip audioClip;
	}

    //public SoundManager.eSoundType SoundType;

	//public SoundManager.eSubSoundType SubSoundType;
    /// <summary>
    /// 0~1
    /// </summary>
    public float MaxVolume = 1.0f;
}
