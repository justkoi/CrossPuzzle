using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CoroutineManager.Instance.AddCoroutine(this, DelayedTranslation());
    }

    IEnumerator DelayedTranslation()
    {
        yield return CoroutineManager.Wait(3.3f);
        FadeInManager.Instance.FadingTranslation("TitleScene");

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
