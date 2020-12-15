using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public TitleSceneView view;

    // Start is called before the first frame update
    void Start()
    {
        view.OptionUI.SetActive(false);
        view.btnGameStart.Init(OnClickStart);
        view.btnExit.Init(OnClickExit);

        view.btnOption.Init(OnClickOption);
        view.btnClose.Init(OnClickClose);
        view.btnBGM.Init(OnClickBGM);
        view.btnSFX.Init(OnClickSFX);
        SoundManager.Instance.Play(new SoundPlayData("Menu_BGM", E_AUDIO_GROUP_TYPE.InGame_BGM, E_AUDIO_CHANNEL_TYPE.BGM_Map, E_AUDIO_CLIP_GROUP.Bgm_Map, null, true, E_LOOP_TYPE.Loop, "Default", 3.0f, 4.0f, 0, 1, 0.0f));

    }

    public void OnClickStart()
    {
        FadeInManager.Instance.FadingTranslation("DifficultyScene");
    }


    public void OnClickExit()
    {
        FadeInManager.Instance.FadingTranslation("LogoScene");
    }

    public void OnClickOption()
    {
        view.OptionUI.SetActive(true);
    }

    public void OnClickClose()
    {
        view.OptionUI.SetActive(false);
    }

    public void OnClickBGM()
    {
        if(SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).m_fVolume >= 0.5f)
        {
            SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).m_fVolume = 0;
        }
        else
        {
            SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.BGM_Map).m_fVolume = 1.0f;
        }
    }

    public void OnClickSFX()
    {
        if (SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE).m_fVolume >= 0.5f)
        {
            SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE).m_fVolume = 0;
        }
        else
        {
            SoundManager.Instance.GetAudioChannel(E_AUDIO_CHANNEL_TYPE.UISE).m_fVolume = 1.0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class TitleSceneView
{
    public GameObject OptionUI;
    public CommonButton btnClose;
    public CommonButton btnBGM;
    public CommonButton btnSFX;

    public CommonButton btnGameStart;
    public CommonButton btnOption;
    public CommonButton btnCredit;
    public CommonButton btnExit;
}