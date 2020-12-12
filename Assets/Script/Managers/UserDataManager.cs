using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : Singleton<UserDataManager>
{
    public Difficulty currDifficulty;
    public int currStage;

    public StageInfo currStageInfos
    {
        get
        {
            return StageInfos[(int)UserDataManager.Instance.currDifficulty, UserDataManager.Instance.currStage];
        }
    }

    public StageInfo[,] StageInfos = new StageInfo[,]
    {
    {
        new StageInfo( 5 , 400 ), //!< 1Stage
		new StageInfo( 6 , 550 ), //!< 2Stage
		new StageInfo( 6 , 750 ), //!< 3Stage
		new StageInfo( 7 , 1150 ), //!< 4Stage
		new StageInfo( 7 , 1250 ), //!< 5Stage
		new StageInfo( 8 , 1350 ), //!< 6Stage
		new StageInfo( 8 , 1650 ), //!< 7Stage
		new StageInfo( 8 , 1850 ), //!< 8Stage
		new StageInfo( 8 , 2150 ), //!< 9Stage
		new StageInfo( 8 , 2550 ), //!< 10Stage
		new StageInfo( 9 , 2950 ), //!< 11Stage
		new StageInfo( 9 , 3350 ), //!< 12Stage
		new StageInfo( 9 , 3650 ), //!< 13Stage
		new StageInfo( 10 , 4050 ), //!< 14Stage
		new StageInfo( 10 , 4650 ), //!< 15Stage
		new StageInfo( 11 , 5150 ), //!< 16Stage
		new StageInfo( 12 , 5650 ), //!< 17Stage
		new StageInfo( 13 , 6100 ), //!< 18Stage
		new StageInfo( 14, 6550 ), //!< 19Stage
		new StageInfo( 15 , 6850 ), //!< 20Stage
		new StageInfo( 16 , 7050 ), //!< 21Stage
		new StageInfo( 16 , 7050 ), //!< 22Stage
		new StageInfo( 16 , 7050 ), //!< 23Stage
		new StageInfo( 16 , 7050 ), //!< 24Stage
		new StageInfo( 16 , 7050 ), //!< 25Stage
		new StageInfo( 16 , 7050 ), //!< 26Stage
		new StageInfo( 16 , 7050 ), //!< 27Stage
		new StageInfo( 16 , 7050 ), //!< 28Stage
		new StageInfo( 16 , 7050 ), //!< 29Stage
		new StageInfo( 16 , 7050 ), //!< 30Stage
		new StageInfo( 16 , 7050 ), //!< 31Stage
	},
    {
        new StageInfo( 5 , 600 ), //!< 1Stage
		new StageInfo( 6 , 750 ), //!< 2Stage
		new StageInfo( 6 , 950 ), //!< 3Stage
		new StageInfo( 7 , 1450 ), //!< 4Stage
		new StageInfo( 7 , 1650 ), //!< 5Stage
		new StageInfo( 8 , 1950 ), //!< 6Stage
		new StageInfo( 8 , 2150 ), //!< 7Stage
		new StageInfo( 8 , 2350 ), //!< 8Stage
		new StageInfo( 8 , 2650 ), //!< 9Stage
		new StageInfo( 8 , 3250 ), //!< 10Stage
		new StageInfo( 9 , 3550 ), //!< 11Stage
		new StageInfo( 9 , 3850 ), //!< 12Stage
		new StageInfo( 9 , 4250 ), //!< 13Stage
		new StageInfo( 9 , 4550 ), //!< 14Stage
		new StageInfo( 9 , 5850 ), //!< 15Stage
		new StageInfo( 10 , 6550 ), //!< 16Stage
		new StageInfo( 11 , 7050 ), //!< 17Stage
		new StageInfo( 12 , 7400 ), //!< 18Stage
		new StageInfo( 13 , 7850 ), //!< 19Stage
		new StageInfo( 14 , 8350 ), //!< 20Stage
		new StageInfo( 15 , 9050 ), //!< 21Stage
		new StageInfo( 15 , 9050 ), //!< 22Stage
		new StageInfo( 15 , 9050 ), //!< 23Stage
		new StageInfo( 15 , 9050 ), //!< 24Stage
		new StageInfo( 15 , 9050 ), //!< 25Stage
		new StageInfo( 15 , 9050 ), //!< 26Stage
		new StageInfo( 15 , 9050 ), //!< 27Stage
		new StageInfo( 15 , 9050 ), //!< 28Stage
		new StageInfo( 15 , 9050 ), //!< 29Stage
		new StageInfo( 15 , 9050 ), //!< 30Stage
		new StageInfo( 15 , 9050 ), //!< 31Stage
	},
    {
        new StageInfo( 5 , 1000 ), //!< 1Stage
		new StageInfo( 5 , 1550 ), //!< 2Stage
		new StageInfo( 6 , 1750 ), //!< 3Stage
		new StageInfo( 7 , 2150 ), //!< 4Stage
		new StageInfo( 8 , 2650 ), //!< 5Stage
		new StageInfo( 9 , 3150 ), //!< 6Stage
		new StageInfo( 9 , 3450 ), //!< 7Stage
		new StageInfo( 9 , 3850 ), //!< 8Stage
		new StageInfo( 9 , 4250 ), //!< 9Stage
		new StageInfo( 9 , 4650 ), //!< 10Stage
		new StageInfo( 10 , 5050 ), //!< 11Stage
		new StageInfo( 10, 5450 ), //!< 12Stage
		new StageInfo( 10 , 5850 ), //!< 13Stage
		new StageInfo( 10 , 6250 ), //!< 14Stage
		new StageInfo( 10 , 6650 ), //!< 15Stage
		new StageInfo( 12 , 7450 ), //!< 16Stage
		new StageInfo( 13 , 8050 ), //!< 17Stage
		new StageInfo( 14 , 8600 ), //!< 18Stage
		new StageInfo( 15 , 9250 ), //!< 19Stage
		new StageInfo( 16 , 9950 ), //!< 20Stage
		new StageInfo( 17 , 10450 ), //!< 21Stage
		new StageInfo( 17 , 10450 ), //!< 22Stage
		new StageInfo( 17 , 10450 ), //!< 23Stage
		new StageInfo( 17 , 10450 ), //!< 24Stage
		new StageInfo( 17 , 10450 ), //!< 25Stage
		new StageInfo( 17 , 10450 ), //!< 26Stage
		new StageInfo( 17 , 10450 ), //!< 27Stage
		new StageInfo( 17 , 10450 ), //!< 28Stage
		new StageInfo( 17 , 10450 ), //!< 29Stage
		new StageInfo( 17 , 10450 ), //!< 30Stage
		new StageInfo( 17 , 10450 ), //!< 31Stage
	}
};

    void Start()
    {
        
    }
}


public struct StageInfo
{

    public int nActPoint;  //!< 행동력
    public int nTargetScore; //!< 목표점수

    public StageInfo (int nActPoint, int nTargetScore)
    {
        this.nActPoint = nActPoint;
        this.nTargetScore = nTargetScore;
    }
};

