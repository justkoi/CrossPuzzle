using UnityEngine;
using System.Collections;

public class SoundSpeaker : MonoBehaviour {

    public SoundPlayData Data;
	// Use this for initialization
	void Start () {
        CoroutineManager.Instance.AddCoroutine(this, PlaySound());
	}

    public IEnumerator PlaySound()
    {
        yield return CoroutineManager.Wait(2.0f);

        SoundManager.Instance.Play(Data);
    }

    public void OnDestroy()
    {

    }
    
}
