using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyScene : MonoBehaviour
{
    public float fFadeInTime = 3.0f;
    public float fStayTime = 1.0f;

    public tk2dBaseSprite spriteEasy;
    public tk2dBaseSprite spriteNormal;
    public tk2dBaseSprite spriteHard;

    public CommonButton btnEasy;
    public CommonButton btnNormal;
    public CommonButton btnHard;

    public CommonButton btnBack;

    IEnumerator coDynamicColorize;
    void Start()
    {
        btnEasy.Init(OnClickEasy);
        btnNormal.Init(OnClickNormal);
        btnHard.Init(OnClickHard);

        btnBack.Init(OnClickBack);

        spriteEasy.SetAlpha(1.0f);
        spriteNormal.SetAlpha(0.0f);
        spriteHard.SetAlpha(0.0f);
        CoroutineManager.Instance.AddCoroutine(this, DynamicColorize());
    }

    public void OnClickBack()
    {
        FadeInManager.Instance.FadingTranslation("TitleScene");
    }


    public void GoToNextScene()
    {
        FadeInManager.Instance.FadingTranslation("StageScene");
    }

    public void OnClickEasy()
    {
        UserDataManager.Instance.currDifficulty = Difficulty.Easy;
        GoToNextScene();
    }

    public void OnClickNormal()
    {
        UserDataManager.Instance.currDifficulty = Difficulty.Normal;
        GoToNextScene();
    }

    public void OnClickHard()
    {
        UserDataManager.Instance.currDifficulty = Difficulty.Hard;
        GoToNextScene();
    }

    IEnumerator DynamicColorize()
    {
        while (true)
        {
            CoroutineManager.Instance.AddFadeCoroutine(this, spriteEasy, CoroutineManager.Instance.CreateFadeCoroutine(spriteEasy, 1, 0, fFadeInTime));
            CoroutineManager.Instance.AddFadeCoroutine(this, spriteNormal, CoroutineManager.Instance.CreateFadeCoroutine(spriteNormal, 0, 1, fFadeInTime));

            yield return CoroutineManager.Wait(fFadeInTime+ fStayTime);

            CoroutineManager.Instance.AddFadeCoroutine(this, spriteNormal, CoroutineManager.Instance.CreateFadeCoroutine(spriteNormal, 1, 0, fFadeInTime));
            CoroutineManager.Instance.AddFadeCoroutine(this, spriteHard, CoroutineManager.Instance.CreateFadeCoroutine(spriteHard, 0, 1, fFadeInTime));

            yield return CoroutineManager.Wait(fFadeInTime + fStayTime);

            CoroutineManager.Instance.AddFadeCoroutine(this, spriteHard, CoroutineManager.Instance.CreateFadeCoroutine(spriteHard, 1, 0, fFadeInTime));
            CoroutineManager.Instance.AddFadeCoroutine(this, spriteEasy, CoroutineManager.Instance.CreateFadeCoroutine(spriteEasy, 0, 1, fFadeInTime));
            yield return CoroutineManager.Wait(fFadeInTime + fStayTime);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        CoroutineManager.Instance.RemoveAllCoroutines(this);
    }
}

