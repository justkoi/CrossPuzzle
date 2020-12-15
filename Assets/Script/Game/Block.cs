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

    public float fSelectTimer = 0;
    private static float fSelectTime = 1.0f;

    public GameScene gameScene;
    public E_ITEM_TYPE eItemType = E_ITEM_TYPE.None;

    public void Init(GameScene gameScene, E_BLOCK_TYPE eBlockType, int x, int y)
    {
        this.gameScene = gameScene;
        this.eBlockType = eBlockType;
        this.x = x;
        this.y = y;

        InitBlockType();
        InitPosition();
        view.btnBlock.Init(OnClickBlock);
        SetupItem();
    }

    public void SetupItem()
    {
        if(UserDataManager.Instance.currStage >= 6)
        {
            int random = UnityEngine.Random.Range(0, 100);
            if (random <= 1)
            {
                eItemType = E_ITEM_TYPE.Lighting;
            }
        }
        if(eItemType != E_ITEM_TYPE.None)
        view.items[((int)eItemType)].SetActive(true);
    }

    public void OnClickBlock()
    {
        if(gameScene.eGameStep == E_GAME_STEP.Select)
        {
            gameScene.selector.Select(this);
        }
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

    public void Select()
    {
        fSelectTimer = 1.0f;
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
    bool destroing = false;
    public void DelayedDestroy()
    {
        destroing = true;
        var nextRoutine = CoroutineManager.Instance.AddFadeCoroutine(this, view.btnBlock.m_spriteButtonUp, CoroutineManager.Instance.CreateFadeCoroutine(view.btnBlock.m_spriteButtonUp, 1, 0, 0.6f));
        CoroutineManager.Instance.AddNextCoroutine(this, nextRoutine, DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        Destroy(this.gameObject);
        yield return CoroutineManager.End();
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
        
        if (fSelectTime > float.Epsilon)
        {
            fSelectTimer -= Time.deltaTime;
            if (fSelectTimer <= 0)
                fSelectTimer = 0;
        }
        view.spriteAura.SetAlpha(fSelectTimer);

        if (destroing)
            return;

        if (eBlockState == E_BLOCK_STATE.None)
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

    private void OnDestroy()
    {
        CoroutineManager.Instance.RemoveAllCoroutines(this);
    }
}
[System.Serializable]
public class BlockView
{
    public GameObject[] items;
    public CommonButton btnBlock;
    public tk2dSprite spriteAura;
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

public enum E_ITEM_TYPE
{
    None,
    Lighting,
}