using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroup
{
    public List<Block> BlockList = new List<Block>();
    int tail = 0;
    public void Init(int x, int y)
    {
        if (BlockManager.Instance.isCheckedBlock(x, y))
            return;
        BlockManager.Instance.CheckBlockPopMap(x, y);

        BlockList.Add(BlockManager.Instance.GetBlock(x, y));

        while (true)
        {
            CheckTarget(BlockList[tail]);
            tail++;
            if (tail >= BlockList.Count)
                break;
        }
    }

    public void CheckTarget(Block target)
    {
        int x = target.x;
        int y = target.y;
        E_BLOCK_TYPE eType = target.eBlockType;
        Inlist(eType, x + 1, y);
        Inlist(eType, x - 1, y);
        Inlist(eType, x, y + 1);
        Inlist(eType, x, y - 1);
    }

    private void Inlist(E_BLOCK_TYPE eType, int x, int y)
    {
        if (BlockManager.Instance.isCheckedBlock(x, y))
            return;
        BlockManager.Instance.CheckBlockPopMap(x, y);
        var block = BlockManager.Instance.GetBlock(x, y);
        if (block != null && eType == block.eBlockType)
            BlockList.Add(block);
    }

    public bool isEmptyGroup()
    {
        return BlockList.Count <= 0;
    }

    public Vector2 CreateCenterPoint()
    {
        float fX = 0;
        float fY = 0;
        for (int i = 0; i < BlockList.Count; i++)
        {
            fX += BlockList[i].transform.localPosition.x;
            fY += BlockList[i].transform.localPosition.y;
        }

        fX /= BlockList.Count;
        fY /= BlockList.Count;

        return new Vector2(fX,fY);
    }

    public void PopAll()
    {
        for (int i = 0; i < BlockList.Count; i++)
        {
            BlockManager.Instance.Pop(BlockList[i]);
        }
    }
}
