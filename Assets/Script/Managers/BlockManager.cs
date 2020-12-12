using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
    public Transform trHolder;
    private Block[,] BlockMap = new Block[Constants.BlockMapWidth,Constants.BlockMapHeight];
    private List<Block> BlockList = new List<Block>();
    public Block blockTemplate;

    private bool[,] BlockPopMap = new bool[Constants.BlockMapWidth, Constants.BlockMapHeight];
    private bool[,] BlockVisitMap = new bool[Constants.BlockMapWidth, Constants.BlockMapHeight];
    public bool canFall = false;

    private List<Vector2> DotList = new List<Vector2>();
    void Start()
    {
        ClearMap();
    }

    public Block GetBlock(int x, int y)
    {
        if (x < 0 || x >= Constants.BlockMapWidth)
            return null;
        if (y < 0 || y >= Constants.BlockMapHeight_InRange)
            return null;
        return BlockMap[x, y];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isCheckedBlock(int x, int y)
    {
        if (x < 0 || x >= Constants.BlockMapWidth)
            return true;
        if (y < 0 || y >= Constants.BlockMapHeight_InRange)
            return true;
        return BlockPopMap[x, y] == true;
    }

    public void Pop(Block block)
    {
        EffectManager.Instance.PlayEffect((E_EFFECT_TYPE)block.eBlockType, block.transform.localPosition.x, block.transform.localPosition.y);
        BlockList.Remove(block);
        BlockMap[block.x, block.y] = null;
        Destroy(block.gameObject);
    }

    public void CheckBlockPopMap(int x, int y)
    {
        BlockPopMap[x, y] = true;
    }
    private void CheckVisit(Block block)
    {
        BlockVisitMap[block.x, block.y] = true;
    }

    public void BlockPop()
    {
        BlockManager.Instance.canFall = false;
        List<BlockGroup> blockGroupList = new List<BlockGroup>();
        ClearVisitMap();
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                if (isVIsited(x,y))
                    continue;
                ClearPopMap();
                BlockGroup newGroup = new BlockGroup();
                newGroup.Init(x, y);
                if(newGroup.BlockList.Count >= 4)
                {
                    for (int i = 0; i < newGroup.BlockList.Count; i++)
                    {
                        CheckVisit(newGroup.BlockList[i]);
                    }
                    blockGroupList.Add(newGroup);
                }
            }
        }
        DotList.Clear();
        for (int i=0; i<blockGroupList.Count; i++)
        {
            Vector2 Dot = blockGroupList[i].CreateCenterPoint();
            DotList.Add(Dot);
            blockGroupList[i].PopAll(); //And Create Dot
        }
    }

    private bool isVIsited(int x, int y)
    {
        return BlockVisitMap[x, y] == true;
    }

    private void CreateDot()
    {
        for (int i = 0; i < DotList.Count; i++)
            EffectManager.Instance.PlayEffect(E_EFFECT_TYPE.Dot, DotList[i].x, DotList[i].y);
    }

    private void CreateLine()
    {
        for(int i=0; i<DotList.Count-1; i++)
        {
           EffectManager.Instance.PlayLineEffect(DotList[i], DotList[i+1]);
            //DotList[i],DotList[i + 1];
        }
        if(DotList.Count >= 3)
            EffectManager.Instance.PlayLineEffect(DotList[DotList.Count - 1], DotList[0]);
    }

    public void BlockLinePop()
    {
        //Dot0 -> Dot1
        //...Dot1 -> Dot2
        ///...Dot2 -> Dot0
        CreateDot();
        CreateLine();
        /// CreateLine();
        /// LinePop();
    }

    public bool CanFall(Block target)
    {
        return (target.y-1) >= 0 && BlockMap[target.x, target.y - 1] == null && canFall;
    }

    public void Fall(Block target)
    {
        BlockMap[target.x, target.y] = null;
        target.y -= 1;
        BlockMap[target.x, target.y] = target;
    }

    private bool CanCreate(int x, int y)
    {
        return BlockMap[x, y] == null;
    }

    public void CreateBlock(E_BLOCK_TYPE eBlockType, int x, int y)
    {
        if (!CanCreate(x,y))
            return;

        Block newBlock = Instantiate<Block>(blockTemplate);
        newBlock.transform.parent = trHolder;
        newBlock.Init(eBlockType, x, y);
        BlockMap[x, y] = newBlock;
        BlockList.Add(newBlock);
    }

    public void CreateRandomBlock(int x, int y)
    {
        int random = UnityEngine.Random.Range(0, ((int)E_BLOCK_TYPE.MAX));
        CreateBlock((E_BLOCK_TYPE)random, x, y);
    }

    public bool isAllBlockReady()
    {
        for(int i=0; i< BlockList.Count; i++)
        {
            if(!BlockList[i].isReady())
            {
                return false;
            }
        }
        return true;
    }

    public void FillBlock(int x, int y)
    {
        if (!CanCreate(x, y))
            return;
        CreateRandomBlock(x, GetEmptyNearestY(x));
    }
    /*
    public void FillSingleLine()
    {

    }
    */
    private int GetEmptyNearestY(int x)
    {
        for(int y=Constants.BlockMapHeight_InRange; y<Constants.BlockMapHeight; y++)
        {
            if(CanCreate(x,y))
            {
                return y;
            }
        }
        return Constants.BlockMapHeight - 1;
    }

    public void CreateLine(int y)
    {
        for(int x=0; x<Constants.BlockMapWidth; x++)
        {
            CreateRandomBlock(x, y);
        }
    }

    //CreateBlock type, x,y
    //CreateLine type , y
    //CreateTopLine type ,y

    //Fall
    private void ClearMap()
    {
        for (int y = 0; y < Constants.BlockMapHeight; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                BlockMap[x, y] = null;
            }
        }
    }
    private void ClearPopMap()
    {
        for (int y = 0; y < Constants.BlockMapHeight; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                BlockPopMap[x, y] = false;
            }
        }
    }
    private void ClearVisitMap()
    {
        for (int y = 0; y < Constants.BlockMapHeight; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                BlockVisitMap[x, y] = false;
            }
        }
    }
}
