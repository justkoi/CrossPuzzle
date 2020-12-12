using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockView view;
    public E_BLOCK_TYPE eBlockType;
    public int x;
    public int y;
    public int nextY;
    public float nextPosY;
    private float jumpPower;
    private static float baseJumpPower = Constants.BlockHeight * 1.7f;
    private E_BLOCK_STATE eBlockState;
    private static float baseGravityPower = Constants.BlockHeight * 6.0f;
    private float gravityPower = baseGravityPower;

    public void Init(E_BLOCK_TYPE eBlockType, int x, int y)
    {
        this.eBlockType = eBlockType;
        this.x = x;
        this.y = y;


        InitBlockType();
        InitPosition();
    }

    private void InitBlockType()
    {
        switch(eBlockType)
        {
            case E_BLOCK_TYPE.Blue:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_BLUE);
                break;
            case E_BLOCK_TYPE.Green:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_GREEN);
                break;
            case E_BLOCK_TYPE.Purple:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_PURPLE);
                break;
            case E_BLOCK_TYPE.Red:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_RED);
                break;
            case E_BLOCK_TYPE.White:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_WHITE);
                break;
            case E_BLOCK_TYPE.Yellow:
                view.btnBlock.SetStyle(E_COMMON_BUTTON_STYLE.COMMON_BLOCK_YELLOW);
                break;
        }
    }
    private void InitPosition()
    {
        this.transform.SetLocalPosition(Constants.BlockWidth * (x - 3), Constants.BlockHeight * (y),0);

    }

    private bool CanFall()
    {
        return BlockManager.Instance.CanFall(this);
    }

    private void Fall(bool continuous)
    {
        BlockManager.Instance.Fall(this);
        eBlockState = E_BLOCK_STATE.Falling;
        nextY = y - 1;
        nextPosY = Constants.BlockHeight * (nextY);
        if(!continuous)
            gravityPower = baseGravityPower;
    }

    private void Falling()
    {
        this.transform.SetLocalPositionY(this.transform.localPosition.y - (gravityPower*Time.deltaTime));
        gravityPower += baseGravityPower * Time.deltaTime * 1.0f;
    }

    private void Bouncing()
    {
        jumpPower -= baseGravityPower * Time.deltaTime;
        this.transform.SetLocalPositionY(this.transform.localPosition.y + (jumpPower * Time.deltaTime));
    }

    public  bool isReady()
    {
        return eBlockState == E_BLOCK_STATE.None;
    }

    private bool isGround()
    {
        return this.transform.localPosition.y <= nextPosY;
    }

    private void SetGround()
    {
        this.transform.SetLocalPositionY(nextPosY);
    }

    private void StopFalling()
    {
        eBlockState = E_BLOCK_STATE.None;
    }

    private void StartBounce()
    {
        eBlockState = E_BLOCK_STATE.Bounced;
        jumpPower = baseJumpPower;
    }

    void Update()
    {
        if(eBlockState == E_BLOCK_STATE.None)
        {
            if(CanFall())
            {
                Fall(false);
            }
        }
        else if(eBlockState == E_BLOCK_STATE.Falling)
        {
            Falling();
            if(isGround())
            {
                SetGround();
                if (CanFall())
                {
                    Fall(true);
                }
                else
                {
                    StartBounce();
                }
            }
        }
        else if(eBlockState == E_BLOCK_STATE.Bounced)
        {
            Bouncing();
            if (isGround())
            {
                SetGround();
                StopFalling();
            }
        }
    }
}
[System.Serializable]
public class BlockView
{
    public CommonButton btnBlock;
}
public enum E_BLOCK_TYPE
{
    Red,
    Yellow,
    Blue,
    Green,
    Purple,
    White,
    MAX
}

public enum E_BLOCK_STATE
{
    None,
    Falling,
    Bounced,
}