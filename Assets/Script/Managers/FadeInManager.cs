using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInManager : Singleton<FadeInManager>
{
    public tk2dBaseSprite spriteBlind;
    public string _strNextScene;
    public eFaidState currState = eFaidState.E_FAID_NONE;
    IEnumerator coFadeIn;
    IEnumerator coFadeOut;
    public void FadingTranslation(string strNextScene, float fFadeInTime=1.0f, float fFadeOutTime=1.0f)
    {
        _strNextScene = strNextScene;
        spriteBlind.gameObject.SetActive(true);
        spriteBlind.SetAlpha(0.0f);
        coFadeIn = FadeIn(fFadeInTime);
        coFadeOut = FadeOut(fFadeOutTime);
        CoroutineManager.Instance.AddCoroutine(this, coFadeIn);
    }

    IEnumerator FadeIn(float fFadeInTime)
    {
        var nextRoutine = CoroutineManager.Instance.AddFadeCoroutine(this, spriteBlind, CoroutineManager.Instance.CreateFadeCoroutine(spriteBlind, 0, 1.0f, fFadeInTime));
        CoroutineManager.Instance.AddNextCoroutine(this, nextRoutine, coFadeOut);
        CoroutineManager.Instance.AddNextCoroutine(this, nextRoutine, GoToNextScene(_strNextScene));
        yield return CoroutineManager.End();
    }

    IEnumerator FadeOut(float fFadeOutTime)
    {
        var nextRoutine =  CoroutineManager.Instance.AddFadeCoroutine(this, spriteBlind, CoroutineManager.Instance.CreateFadeCoroutine(spriteBlind, 1, 0, fFadeOutTime));
        yield return CoroutineManager.End();
    }


    IEnumerator GoToNextScene(string strNextScene)
    {
        yield return CoroutineManager.Pass();
        SceneManager.LoadScene(strNextScene);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum eFaidState
{
    E_FAID_NONE,
    E_FAID_IN,
    E_FAID_OUT,
}