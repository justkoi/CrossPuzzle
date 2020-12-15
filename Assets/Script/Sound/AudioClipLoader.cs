using UnityEngine;
using Global.Constants;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 오디오 클립 로드 요청을 받으면 로드 후 보관처에 할당 후 플레이까지 이어주는 클래스입니다.
/// </summary>
public class AudioClipLoader
{
    private AudioClipStorage m_Storage;
    //public BetterList<SoundLoader> Loaders = new BetterList<SoundLoader>();

    public void Init(AudioClipStorage Storage)
    {
        m_Storage = Storage;
    }

    public void LoadAsyncAudioClip(SoundPlayData Data, AudioPlayType PlayType)
    {
        ///해당 그룹에 클립이 있는가? => 해당 그룹에서 클립을 꺼내옴
        ///클립이 없는가? => 저장소에 클립이 있는가? (2)로 없는가? (1)로
        ///(1)로드후 저장소에 추가 => (2)저장소에서 해당 그룹에 클립을 할당
        ///
        ///위의 로직이 완료되면 Play 시작
        ///

        Data.m_PlayType = PlayType;

        AudioClip TargetClip = null;
        if (Data.m_AudioClipGroup.HasAudioClip(Data.m_strSoundFileName, out TargetClip))
        {
            Data.LoadComplete(TargetClip, 0.0f);
            Data.TryPlay();
        }
        else
        {
            ProcessLoad(Data);
        }
    }

    static private string m_strLocalLoad = "Sound/";

    public void ProcessLoad(SoundPlayData Data)
    {
        /// 이미 로드 중인 데이터 클립 제거 추가
        /// 
        /// 이미 로드중인 클립이 있는가? => 플레이 리스트에 추가
        ///
        /*
        for (int i = 0; i < Loaders.size; i++)
        {
            if (Loaders[i].m_strName == Data.m_strSoundFileName)
            {
               Loaders[i].AddPlayData(Data);
                return;
            }
        }
        */
        AudioClip TargetClip = Resources.Load<AudioClip>(string.Concat(m_strLocalLoad, Data.m_strSoundFileName));
        if (TargetClip == null)
        {
            Debugs.LogError("[사운드로드] ProcessLoad 실패! 대상 클립(", Data.m_strSoundFileName, ")을 찾을 수 없습니다!");
        }
        else
        {
            m_Storage.AddAudioClip(TargetClip);
            Data.m_AudioClipGroup.PushAudioClip(TargetClip);
            Data.LoadComplete(TargetClip, 0.0f);
            Debugs.Log(string.Concat("[사운드로드] 완료! ", Data.m_LoadedAudioClip.name, " 소요 시간 [", 0.0f, "]"));
            Data.TryPlay();
        }
        return;
    }

    public void LateUpdate()
    {
        /*
        for (int i = 0; i < Loaders.size; i++)
        {
            SoundLoader Loader = Loaders[i];
            if (Loader.ProcessCheckLoad())
            {
                Loaders.RemoveAt(i--);
            }
        }
        */
    }

    public void ClearSoundLoaders()
    {
        //Loaders.Release();
    }
    
}
