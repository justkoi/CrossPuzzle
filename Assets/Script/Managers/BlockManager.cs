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

    private int[,] StartMap = new int[Constants.BlockMapWidth, Constants.BlockMapHeight];
    private bool[,] BlockPopMap = new bool[Constants.BlockMapWidth, Constants.BlockMapHeight];
    private bool[,] BlockVisitMap = new bool[Constants.BlockMapWidth, Constants.BlockMapHeight];
    public bool canFall = false;
    private float fDelayedFallTimer;
    private List<Vector2> DotList = new List<Vector2>();
    private int needBlockForPop = 4;
    public int GetNeedBlockForPpo()
    {
        if (gameScene.AP % 3 == 0)
            return needBlockForPop - 1;
        else
            return needBlockForPop;
    }
    public GameScene gameScene;

    public List<Vector2Int> StackedLightning = new List<Vector2Int>();
    void Start()
    {
        ClearMap();
    }


    public void ClearAll()
    {
        ClearMap();
        ClearPopMap();
        ClearVisitMap();
        DotList.Clear();
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
        if (fDelayedFallTimer > 0)
        {
            fDelayedFallTimer -= Time.deltaTime;
            if (fDelayedFallTimer <= float.Epsilon)
                canFall = true;
        }
    }

    public bool isCheckedBlock(int x, int y)
    {
        if (x < 0 || x >= Constants.BlockMapWidth)
            return true;
        if (y < 0 || y >= Constants.BlockMapHeight_InRange)
            return true;
        return BlockPopMap[x, y] == true;
    }

    public void Pop(Block block, bool itemPop = false)
    {
        if (block == null)
            return;
        if (!itemPop && block.eItemType == E_ITEM_TYPE.Lighting)
            StackedLightning.Add(new Vector2Int(block.x, block.y));
        EffectManager.Instance.PlayEffect((E_EFFECT_TYPE)block.eBlockType, block.transform.localPosition.x, block.transform.localPosition.y);
        EffectManager.Instance.PlayEffect(E_EFFECT_TYPE.Score, block.transform.localPosition.x, block.transform.localPosition.y + Constants.BlockHeight * 0.3f).SetText(Constants.BlockPopScore.ToString());
        ScoreManager.Instance.currScore += Constants.BlockPopScore;
        BlockList.Remove(block);
        BlockMap[block.x, block.y] = null;
        Destroy(block.gameObject);
    }

    public void ItemPop()
    {
        for(int i=0; i<StackedLightning.Count; i++)
        {
            EffectData e = EffectManager.Instance.PlayEffect(E_EFFECT_TYPE.Lightning, Constants.BlockWidth * (StackedLightning[i].x - 3), 0);
            e.transform.parent = trHolder;
            e.transform.SetLocalPositionY(580);
            SoundManager.Instance.Play(new SoundPlayData("Lightning", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
            for (int y=0; y<Constants.BlockMapHeight_InRange; y++)
            {
                Pop(GetBlock(StackedLightning[i].x, y), true);
            }
        }
        StackedLightning.Clear();
    }


    public void Delete(Block block)
    {
        BlockList.Remove(block);
        BlockMap[block.x, block.y] = null;
        block.DelayedDestroy();
    }


    public void CheckBlockPopMap(int x, int y)
    {
        BlockPopMap[x, y] = true;
    }
    private void CheckVisit(Block block)
    {
        BlockVisitMap[block.x, block.y] = true;
    }

    public bool BlockPop()
    {
        List<BlockGroup> blockGroupList = new List<BlockGroup>();
        ClearVisitMap();
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                if (isVIsited(x,y))
                    continue;
                if (GetBlock(x, y) == null)
                    continue;
                ClearPopMap();
                BlockGroup newGroup = new BlockGroup();
                newGroup.Init(x, y);
                if(newGroup.BlockList.Count >= GetNeedBlockForPpo())
                {
                    for (int i = 0; i < newGroup.BlockList.Count; i++)
                    {
                        CheckVisit(newGroup.BlockList[i]);
                    }
                    blockGroupList.Add(newGroup);
                }
            }
        }

        if (blockGroupList.Count > 0)
            SoundManager.Instance.Play(new SoundPlayData("BlockPop_1", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));

        DotList.Clear();
        for (int i=0; i<blockGroupList.Count; i++)
        {
            Vector2 Dot = blockGroupList[i].CreateCenterPoint();
            DotList.Add(Dot);
            blockGroupList[i].PopAll(); //And Create Dot
        }
        return blockGroupList.Count > 0;
    }

    public void CreateStartLine(int blockMapHeight_InRange, int y)
    {
        for(int x=0; x<Constants.BlockMapWidth; x++)
        {
            CreateBlock((E_BLOCK_TYPE)StartMap[x, y], x, blockMapHeight_InRange);
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

    List<Vector3> lastLine1 = new List<Vector3>();
    List<Vector3> lastLine2 = new List<Vector3>();

    List<Vector3> lastBlock1 = new List<Vector3>();
    List<Vector3> lastBlock2 = new List<Vector3>();
    List<Block> delayedPopList = new List<Block>();

    private void LinePop(Vector2 p1, Vector2 p2)
    {
        lastLine1.Add(new Vector3(p1.x, p1.y - 671.13f, - 400));
        lastLine2.Add(new Vector3(p2.x, p2.y - 671.13f, -400));
        for (int i=0; i< BlockList.Count; i++)
        {
            var block = BlockList[i];

            Vector2 TopLeft = new Vector2(block.transform.localPosition.x - (Constants.BlockWidth / 2), block.transform.localPosition.y + (Constants.BlockHeight / 2));
            Vector2 TopRight = new Vector2(block.transform.localPosition.x + (Constants.BlockWidth / 2), block.transform.localPosition.y + (Constants.BlockHeight / 2));
            Vector2 BottomLeft = new Vector2(block.transform.localPosition.x - (Constants.BlockWidth / 2), block.transform.localPosition.y - (Constants.BlockHeight / 2));
            Vector2 BottomRIght = new Vector2(block.transform.localPosition.x + (Constants.BlockWidth / 2), block.transform.localPosition.y - (Constants.BlockHeight / 2));

            lastBlock1.Add(new Vector3(TopLeft.x, TopLeft.y - 671.13f));
            lastBlock2.Add(new Vector3(TopRight.x, TopRight.y - 671.13f));

            lastBlock1.Add(new Vector3(TopRight.x, TopRight.y - 671.13f));
            lastBlock2.Add(new Vector3(BottomRIght.x, BottomRIght.y - 671.13f));

            lastBlock1.Add(new Vector3(BottomRIght.x, BottomRIght.y - 671.13f));
            lastBlock2.Add(new Vector3(BottomLeft.x, BottomLeft.y - 671.13f));

            lastBlock1.Add(new Vector3(BottomLeft.x, BottomLeft.y - 671.13f));
            lastBlock2.Add(new Vector3(TopLeft.x, TopLeft.y - 671.13f));

            //Camera.main.ScreenToWorldPoint()
            if (HitTest_LintToLine(p1, p2, TopLeft, TopRight) ||
                HitTest_LintToLine(p1, p2, TopRight, BottomRIght) ||
                HitTest_LintToLine(p1, p2, BottomRIght, BottomLeft) ||
                HitTest_LintToLine(p1, p2, BottomLeft, TopLeft) )
                delayedPopList.Add(block);
        }
    }
    public bool HasWaitedLinePop()
    {
        return delayedPopList.Count > 0;
    }
    public void WaitedLinePop()
    {
        //바로지우면 blockList 인덱스 꼬임
        for (int i = 0; i < delayedPopList.Count; i++)
        {
            BlockManager.Instance.Pop(delayedPopList[i]);
        }
        delayedPopList.Clear();
        SoundManager.Instance.Play(new SoundPlayData("BlockPop_1", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < lastLine1.Count; i++)
            Gizmos.DrawLine(lastLine1[i], lastLine2[i]);
        Gizmos.color = Color.green;
        for (int i=0; i< lastBlock1.Count; i++)
            Gizmos.DrawLine(lastBlock1[i], lastBlock2[i]);
    }

    bool HitTest_LintToLine(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        double d = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
        if (d <= Double.Epsilon) return false;
        double pre = (p1.x * p2.y - p1.y * p2.x), post = (p3.x * p4.y - p3.y * p4.x);

        double x = (pre * (p3.x - p4.x) - (p1.x - p2.x) * post) / d;
        double y = (pre * (p3.y - p4.y) - (p1.y - p2.y) * post) / d;

        if (x < Math.Min(p1.x, p2.x) || x > Math.Max(p1.x, p2.x) || x < Math.Min(p3.x, p4.x) || x > Math.Max(p3.x, p4.x))
            return false;
        if (y < Math.Min(p1.y, p2.y) || y > Math.Max(p1.y, p2.y) || y < Math.Min(p3.y, p4.y) || y > Math.Max(p3.y, p4.y))
            return false;

        return true;
    }

    bool HitTest_LineToSquare()
    {
        return true;
    }

    bool HitTest_LineToBlock()
    {
        return true;
    }

    private void CreateLine()
    {
        delayedPopList.Clear();
        lastLine1.Clear();
        lastLine2.Clear();
        lastBlock1.Clear();
        lastBlock2.Clear();
        Vector2 CenterPoint = Vector2.zero;
        E_EFFECT_TYPE eEffectType = E_EFFECT_TYPE.Dot;
        for(int i=0; i<DotList.Count-1; i++)
        {
           EffectManager.Instance.PlayLineEffect(DotList[i], DotList[i+1]);
            LinePop(DotList[i], DotList[i + 1]);
            //LineHitTest();
            //DotList[i],DotList[i + 1];
        }

        if (DotList.Count >= 3)
        {
            EffectManager.Instance.PlayLineEffect(DotList[DotList.Count - 1], DotList[0]);
            LinePop(DotList[DotList.Count - 1], DotList[0]);
        }

        for (int i = 0; i < DotList.Count; i++)
        {
            CenterPoint.x += DotList[i].x;
            CenterPoint.y += DotList[i].y;
        }

        CenterPoint.x /= DotList.Count;
        CenterPoint.y /= DotList.Count;
        switch (DotList.Count)
        {
            case 1:
                eEffectType = E_EFFECT_TYPE.Msg_Dot;
                break;
            case 2:
                eEffectType = E_EFFECT_TYPE.Msg_Line;
                SoundManager.Instance.Play(new SoundPlayData("Line", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
                break;
            case 3:
                eEffectType = E_EFFECT_TYPE.Msg_Triangle;
                SoundManager.Instance.Play(new SoundPlayData("Triangle", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
                break;
            case 4:
                eEffectType = E_EFFECT_TYPE.Msg_Square;
                SoundManager.Instance.Play(new SoundPlayData("Square", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
                break;
            default:
                eEffectType = E_EFFECT_TYPE.Msg_Pentagon;
                SoundManager.Instance.Play(new SoundPlayData("Pentagon", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
                break;
        }

        if(DotList.Count == 1)
            EffectManager.Instance.PlayEffect(eEffectType, CenterPoint.x, CenterPoint.y + Constants.BlockHeight * 0.7f);
        else if (DotList.Count > 1)
            EffectManager.Instance.PlayEffect(eEffectType, CenterPoint.x, CenterPoint.y);
    }

    private void LineHitTest(Vector2 p1, Vector2 p2)
    {

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
        newBlock.Init(gameScene, eBlockType, x, y);
        BlockMap[x, y] = newBlock;
        BlockList.Add(newBlock);
    }

    public void CreateRandomBlock(int x, int y, bool noAdjacent = false)
    {

        List<int> RandomBlockList = new List<int>();
        for(int i=0; i< (int)E_BLOCK_TYPE.MAX; i++)
        {
            RandomBlockList.Add(i);
        }
        if(noAdjacent)
        {
            Block right = GetBlock(x + 1, y);
            Block left = GetBlock(x - 1, y);
            Block up = GetBlock(x, y + 1);
            Block down = GetBlock(x, y - 1);
            if (right != null)
                RandomBlockList.RemoveAll((e) => e == (int)right.eBlockType);
            if (left != null)
                RandomBlockList.RemoveAll((e) => e == (int)left.eBlockType);
            if (up != null)
                RandomBlockList.RemoveAll((e) => e == (int)up.eBlockType);
            if (down != null)
                RandomBlockList.RemoveAll((e) => e == (int)down.eBlockType);
        }
        int random = UnityEngine.Random.Range(0, RandomBlockList.Count);
        CreateBlock((E_BLOCK_TYPE)random, x, y);
    }

    public void BakeStartMap()
    {
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                StartMap[x, y] = -1;
            }
        }

        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                SetStartMap(x, y);
            }
        }
    }

    public int GetStartMap(int x, int y)
    {
        if (x < 0 || x >= Constants.BlockMapWidth)
            return -1;
        if (y < 0 || y >= Constants.BlockMapHeight_InRange)
            return -1;
        return StartMap[x, y];
    }
    public void SetStartMap(int x, int y)
    {
        List<int> RandomBlockList = new List<int>();
        for (int i = 0; i < (int)E_BLOCK_TYPE.MAX; i++)
        {
            RandomBlockList.Add(i);
        }
        int right = GetStartMap(x + 1, y);
        int left = GetStartMap(x - 1, y);
        int up = GetStartMap(x, y + 1);
        int down = GetStartMap(x, y - 1);
        if (right != -1)
            RandomBlockList.RemoveAll((e) => e == (int)right);
        if (left != -1)
            RandomBlockList.RemoveAll((e) => e == (int)left);
        if (up != -1)
            RandomBlockList.RemoveAll((e) => e == (int)up);
        if (down != -1)
            RandomBlockList.RemoveAll((e) => e == (int)down);
        int random = UnityEngine.Random.Range(0, RandomBlockList.Count);
        StartMap[x, y] = RandomBlockList[random] ;
    }

    public bool isAllBlockReady()
    {
        for (int y = 0; y < Constants.BlockMapHeight_InRange; y++)
        {
            for (int x = 0; x < Constants.BlockMapWidth; x++)
            {
                if(GetBlock(x,y) == null)
                    return false;
            }
        }
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
        for(int y=Constants.BlockMapHeight_InRange+1; y<Constants.BlockMapHeight; y++)
        {
            if(CanCreate(x,y))
            {
                return y;
            }
        }
        return Constants.BlockMapHeight - 1;
    }

    public void CreateLine(int y, bool noAdjacent = false)
    {
        for(int x=0; x<Constants.BlockMapWidth; x++)
        {
            CreateRandomBlock(x, y, noAdjacent);
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
        BlockList.Clear();
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
