using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlot : MonoBehaviour
{
    public CommonButton btnStage;
    public tk2dUILayout layout;
    public int nStage;

    public void Init(int nStage)
    {
        this.nStage = nStage;
        btnStage.Init(StartGame);
        btnStage.SetText(nStage.ToString());
        SetStartPosition();
    }

    public int[] uiPositionX = { -300, 0, +300 };
    public int uiPositionY = 300;

    public void SetStartPosition()
    {
        int x = ((nStage-1) % 3) + 1;
        int y = ((nStage-1) / 3);

        this.transform.SetLocalPositionXY(uiPositionX[x-1],y* uiPositionY) ;
        
    }

    public void StartGame()
    {

    }
}
