using System;
using System.Collections;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public GameSceneView view;


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
        view.textStage.text = UserDataManager.Instance.currStage.ToString();

        BlockManager.Instance.trHolder = view.trBlockHolder;
        EffectManager.Instance.trHolder = view.trBlockHolder;
        //BlockManager.Instance.CreateLine(13);
        CoroutineManager.Instance.AddCoroutine(this, CreateStartBlock());
        //BlockManager.Instance.CreateBlock(E_BLOCK_TYPE.Blue, 3, 13);
        SetScore();
        eGameStep = E_GAME_STEP.Starting;
        BlockManager.Instance.canFall = true;
    }

    private void SetScore()
    {
        ScoreManager.Instance.currScore = 0;
    }


    IEnumerator CreateStartBlock()
    {
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            BlockManager.Instance.CreateLine(Constants.BlockMapHeight_InRange);
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

    void Update()
    {
        if(eGameStep == E_GAME_STEP.Ready)
        {
            if(isReadyToStart())
            {
                eGameStep = E_GAME_STEP.Pop;
            }
        }
        else if(eGameStep == E_GAME_STEP.Pop)
        {
            BlockManager.Instance.BlockPop();
            eGameStep = E_GAME_STEP.Line;
        }
        else if(eGameStep == E_GAME_STEP.Line)
        {
            BlockManager.Instance.BlockLinePop();
            eGameStep = E_GAME_STEP.Fill;
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
}

public enum E_GAME_STEP
{
    Starting,
    Ready,
    Pop,
    Line,
    Fill,
    Select,
    SelectPop,
}