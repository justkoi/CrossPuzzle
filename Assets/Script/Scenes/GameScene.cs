using System;
using System.Collections;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public GameObject[] objBackGrounds;
    public GameSceneView view;
    public Selector selector;

    private int _AP;
    public int AP
    {
        get
        {
            return _AP;
        }
        set
        {
            if (_AP != value)
            {
                OnChangeAP.Execute(value);
            }
            _AP = value;
        }
    }


    public Action<int> OnChangeAP;

    public E_GAME_STEP eGameStep = E_GAME_STEP.Starting;

    void Start()
    {
        ScoreManager.Instance.OnScoreChange += HandleOnScoreChange;
        view.textGoalScore.text = ScoreManager.Instance.GetGoalScore().ToString();
        OnChangeAP += HandleOnChangeAP;
        AP = UserDataManager.Instance.currStageInfos.nActPoint;
        view.textStage.text = (UserDataManager.Instance.currStage+1).ToString();

        BlockManager.Instance.gameScene = this;
        BlockManager.Instance.trHolder = view.trBlockHolder;
        EffectManager.Instance.trHolder = view.trBlockHolder;
        //BlockManager.Instance.CreateLine(13);
        //BlockManager.Instance.CreateBlock(E_BLOCK_TYPE.Blue, 3, 13);
        SetScore();
        eGameStep = E_GAME_STEP.Starting;
        BlockManager.Instance.canFall = true;
        SoundManager.Instance.Play(new SoundPlayData("Game_BGM_1", E_AUDIO_GROUP_TYPE.InGame_BGM, E_AUDIO_CHANNEL_TYPE.BGM_Map, E_AUDIO_CLIP_GROUP.Bgm_Map, null, true, E_LOOP_TYPE.Loop, "Default", 3.0f, 4.0f, 0, 1, 0.0f));
        SetupView();
        selector.Init(this);
        BlockManager.Instance.BakeStartMap();
        CoroutineManager.Instance.AddCoroutine(this, CreateStartBlock());

    }

    public void SetupView()
    {
        view.viewVictory.btnMenu.Init(OnClickMenu);
        view.viewVictory.btnNext.Init(OnClickNext);
        view.viewOver.btnMenu.Init(OnClickMenu);
        view.viewOver.btnRetry.Init(OnClickRetry);
        for (int i = 0; i < objBackGrounds.Length; i++)
        {
            objBackGrounds[i].SetActive(i == (int)UserDataManager.Instance.currDifficulty);
        }
    }

    private void SetScore()
    {
        ScoreManager.Instance.currScore = 0;
    }

    public void OnClickMenu()
    {
        FadeInManager.Instance.FadingTranslation("StageScene");
    }

    public void OnClickRetry()
    {
        FadeInManager.Instance.FadingTranslation("GameScene");
    }

    public void OnClickNext()
    {
        UserDataManager.Instance.currStage += 1;
        FadeInManager.Instance.FadingTranslation("GameScene");
    }

    public bool CanVictory()
    {
        return ScoreManager.Instance.currScore >= ScoreManager.Instance.GetGoalScore();
    }

    public void Victory()
    {
        view.viewVictory.trMain.SetActive(true);
        SoundManager.Instance.Play(new SoundPlayData("Clear", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
    }

    public bool CanDefeat()
    {
        return AP <= 0;
    }

    public void Defeat()
    {
        view.viewOver.trMain.SetActive(true);
        SoundManager.Instance.Play(new SoundPlayData("Over", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
    }

    IEnumerator CreateStartBlock()
    {
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            BlockManager.Instance.CreateStartLine(Constants.BlockMapHeight_InRange, y);
            yield return CoroutineManager.Wait(0.4f);
        }
        eGameStep = E_GAME_STEP.Ready;
    }

    public void FillingAllBlock()
    {
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                BlockManager.Instance.FillBlock(x, y);
            }
        }
    }


    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChange -= HandleOnScoreChange;
        CoroutineManager.Instance.RemoveAllCoroutines(this);
        BlockManager.Instance.ClearAll();
    }

    private void HandleOnScoreChange(int score)
    {
        view.textCurrScore.text = score.ToString();
    }

    private void HandleOnChangeAP(int AP)
    {
        view.textAP.text = AP.ToString();
    }

    private bool isReadyToStart()
    {
        return BlockManager.Instance.isAllBlockReady();
    }

    private float fWaitTimer = 999f;
    private float fWaitTimeForLinePop = 0.3f;
    private float fWaitTimeForFill = 0.6f;
    private float fWaitTimeForFill2 = 0.6f;
    void Update()
    {
        if((AP-1)%3 == 0)
        {
            view.spriteBG.SetSprite("Game_BackGround_4");
        }
        else
        {
            view.spriteBG.SetSprite("Game_BackGround_3");
        }
        if (eGameStep == E_GAME_STEP.Ready)
        {
            if (isReadyToStart())
            {
                eGameStep = E_GAME_STEP.Pop;
            }
        }
        else if (eGameStep == E_GAME_STEP.Pop)
        {
            BlockManager.Instance.canFall = false;
            if (BlockManager.Instance.BlockPop())
            {
                fWaitTimer = 0;
                eGameStep = E_GAME_STEP.Line;
            }
            else
            {
                BlockManager.Instance.canFall = true;
                eGameStep = E_GAME_STEP.WaitSelect;
            }
        }
        else if (eGameStep == E_GAME_STEP.Line)
        {
            BlockManager.Instance.BlockLinePop();
            if (BlockManager.Instance.HasWaitedLinePop())
            {
                fWaitTimer = 0;
                eGameStep = E_GAME_STEP.WaitLinePop;
            }
            else
            {
                fWaitTimer = 0;
                eGameStep = E_GAME_STEP.Item;
            }
        }
        else if(eGameStep == E_GAME_STEP.Item)
        {
            BlockManager.Instance.ItemPop();
            eGameStep = E_GAME_STEP.WaitFill;
        }
        else if (eGameStep == E_GAME_STEP.WaitLinePop)
        {
            fWaitTimer += Time.deltaTime;
            if (fWaitTimer >= fWaitTimeForLinePop)
            {
                BlockManager.Instance.WaitedLinePop();
                fWaitTimer = 0;
                eGameStep = E_GAME_STEP.Item;
            }
        }
        else if (eGameStep == E_GAME_STEP.WaitFill)
        {
            fWaitTimer += Time.deltaTime;
            if (fWaitTimer >= fWaitTimeForFill)
            {
                eGameStep = E_GAME_STEP.Fill;
            }
        }
        else if (eGameStep == E_GAME_STEP.Fill)
        {
            BlockManager.Instance.canFall = true;
            FillingAllBlock();
            eGameStep = E_GAME_STEP.Ready;
        }
        else if (eGameStep == E_GAME_STEP.WaitSelect)
        {
            if (isReadyToStart())
            {
                BlockManager.Instance.canFall = false;
                eGameStep = E_GAME_STEP.Select;
                fWaitTimer = 0.0f;
            }
        }
        else if (eGameStep == E_GAME_STEP.Select)
        {
            if (CanVictory())
            {
                Victory();
                eGameStep = E_GAME_STEP.End;
            }
            else if (CanDefeat())
            {
                Defeat();
                eGameStep = E_GAME_STEP.End;
            }

            //모든 블록 클릭가능
            //클릭시 셀렉터.다음스텝
            //if(셀렉터.스텝완료)
            //셀렉터.Select()
            //eGameStep == E_GAME_STEP.Ready
        }
        else if (eGameStep == E_GAME_STEP.SelectPop)
        {
            fWaitTimer += Time.deltaTime;
            if (fWaitTimer >= fWaitTimeForFill2)
            {
                BlockManager.Instance.canFall = true;
                eGameStep = E_GAME_STEP.Fill;
            }
        }
    }
}

[Serializable]
public class GameSceneView
{
    public tk2dTextMesh textCurrScore;
    public tk2dTextMesh textGoalScore;
    public tk2dTextMesh textAP;
    public tk2dTextMesh textStage;
    public Transform trBlockHolder;
    public VictoryUIView viewVictory;
    public OverUIView viewOver;
    public tk2dSprite spriteBG;
}

[Serializable]
public class VictoryUIView
{
    public GameObject trMain;
    public CommonButton btnMenu;
    public CommonButton btnNext;
}


[Serializable]
public class OverUIView
{
    public GameObject trMain;
    public CommonButton btnMenu;
    public CommonButton btnRetry;
}

public enum E_GAME_STEP
{
    Starting,
    Ready,
    Pop,
    WaitLine,
    WaitLinePop,
    WaitFill,
    WaitSelect,
    Line,
    Item,
    Fill,
    Select,
    SelectPop,
    End,
}