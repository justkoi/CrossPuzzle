using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    
    public CommonButton btnGameStart;
    public CommonButton btnOption;
    public CommonButton btnCredit;
    public CommonButton btnExit;

    // Start is called before the first frame update
    void Start()
    {
        btnGameStart.Init(OnClickStart);
        btnExit.Init(OnClickExit);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
