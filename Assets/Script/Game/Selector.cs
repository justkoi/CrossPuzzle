using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public SelectorView view;
    public int nStep = 0;
    public List<Block> selectList = new List<Block>();
    public Block currBlock;
    public float fRotationTimer;
    public static float fRotationTime = 1.0f;
    public GameScene gameManager;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Init(GameScene gameManager)
    {
        this.gameManager = gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        view.textStep.text = nStep.ToString();
        if(fRotationTimer > float.Epsilon)
        {
            fRotationTimer -= Time.deltaTime;
            if (fRotationTimer <= 0)
            {
                fRotationTimer = 0;
                DeSelect();

                for (int i = 0; i < selectList.Count; i++)
                {
                    BlockManager.Instance.Delete(selectList[i]);
                    gameManager.eGameStep = E_GAME_STEP.SelectPop;
                }
                gameManager.AP -= 1;
                this.gameObject.SetActive(nStep > 0);
            }
        }
        float z = ((fRotationTime-fRotationTimer)/ fRotationTime) * -90.0f;
        view.spriteSelector.transform.localEulerAngles = new Vector3(view.spriteSelector.transform.localEulerAngles.x, view.spriteSelector.transform.localEulerAngles.y, z);//
    }

    public void Select(Block block)
    {
        SoundManager.Instance.Play(new SoundPlayData("BlockSelect", E_AUDIO_GROUP_TYPE.UI, E_AUDIO_CHANNEL_TYPE.UISE, E_AUDIO_CLIP_GROUP.UI, null, false));
        this.transform.SetWorldPositionXY(block.transform.position.x, block.transform.position.y);
        selectList.Clear();
        if (currBlock != null && currBlock != block)
        {
            DeSelect();
        }

        currBlock = block;
        nStep++;
        int x = block.x;
        int y = block.y;
        if(nStep == 1)
        {
            TrySelect(x,y);

            TrySelect(x+1, y);
            TrySelect(x-1, y);
            TrySelect(x, y+1);
            TrySelect(x, y-1);

            TrySelect(x + 2, y);
            TrySelect(x - 2, y);
            TrySelect(x, y + 2);
            TrySelect(x, y - 2);
            fRotationTimer = 1.0f;
            view.spriteSelector.transform.SetLocalPositionXY(-15, 0);
            view.spriteSelector.transform.localScale = new Vector3(1.0f, 0.85f, view.spriteSelector.transform.localScale.z);
        }
        if(nStep == 2)
        {
            TrySelect(x, y);

            TrySelect(x + 1, y);
            TrySelect(x - 1, y);
            TrySelect(x, y + 1);
            TrySelect(x, y - 1);
            fRotationTimer = 1.0f;
            view.spriteSelector.transform.SetLocalPositionXY(-10, 0);
            view.spriteSelector.transform.localScale = new Vector3(0.6f,0.5f, view.spriteSelector.transform.localScale.z);
        }
        else if(nStep == 3)
        {
            TrySelect(x, y);
            fRotationTimer = 1.0f;
            view.spriteSelector.transform.SetLocalPositionXY(-5, 0);
            view.spriteSelector.transform.localScale = new Vector3(0.2f, 0.2f, view.spriteSelector.transform.localScale.z);
        }
        else if(nStep == 4)
        {
            DeSelect();
        }

        for(int i=0; i<selectList.Count; i++)
        {
            selectList[i].Select();
        }

        this.gameObject.SetActive(nStep > 0);
    }

    private void TrySelect(int x, int y)
    {
        Block target = BlockManager.Instance.GetBlock(x, y);
        if (target == null)
            return;
        selectList.Add(target);
    }

    private void DeSelect()
    {
        currBlock = null;
        nStep = 0;
    }
}

[System.Serializable]
public class SelectorView
{
    public tk2dSprite spriteSelector;
    public tk2dTextMesh textStep;
}