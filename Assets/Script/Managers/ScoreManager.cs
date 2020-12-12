using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    static private int[] baseScoreValue = {50, 70, 100};
    private int _currScore;
    public int currScore
    {
        get
        {
            return _currScore;
        }

        set
        {
            _currScore = value;
            OnScoreChange.Execute(value);
        }
    }

    public Action<int> OnScoreChange;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int GetGoalScore()
    {
        return UserDataManager.Instance.currStageInfos.nTargetScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
