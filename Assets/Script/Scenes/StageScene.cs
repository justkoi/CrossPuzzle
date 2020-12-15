using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScene : MonoBehaviour
{
    public GameObject[] objBackGrounds;
    public StageSlot slotTemplate;
    public Transform trHolder;

    public CommonButton btnBack;
    // Start is called before the first frame update
    void Start()
    {
        btnBack.Init(OnClickBack);
        SetBackground();
    }

    public void OnClickBack()
    {
        FadeInManager.Instance.FadingTranslation("DifficultyScene");
    }

    public void SetBackground()
    {
        for(int i=0; i< objBackGrounds.Length; i++)
        {
            objBackGrounds[i].SetActive(i == (int)UserDataManager.Instance.currDifficulty);
        }

        for(int i=1; i<= Constants.MaxStage; i++)
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
