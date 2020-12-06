using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScene : MonoBehaviour
{
    public GameObject[] objBackGrounds;
    public StageSlot slotTemplate;
    public Transform trHolder;
    // Start is called before the first frame update
    void Start()
    {
        SetBackground();
    }

    public void SetBackground()
    {
        for(int i=0; i< objBackGrounds.Length; i++)
        {
            objBackGrounds[i].SetActive(i == (int)UserDataManager.Instance.currDifficulty);
        }

        for(int i=1; i<= UserDataManager.Instance.maxStage; i++)
        {
            StageSlot newSlot = Instantiate(slotTemplate);
            newSlot.transform.parent = trHolder;
            newSlot.Init(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
