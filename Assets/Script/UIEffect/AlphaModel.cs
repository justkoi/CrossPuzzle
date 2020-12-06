using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlphaModel : MonoBehaviour
{
    public float fTime = 1.0f;
    public float fSlowly = 0.5f;
    private float fTimer = 0.0f;
    public AnimationCurve alphaCurve;
    public tk2dBaseSprite spriteTarget;

    public tk2dBaseSprite fParent;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime * fSlowly;
        if (fTimer >= fTime)
            fTimer -= fTime;

        var value = alphaCurve.Evaluate(fTimer);
        if(fParent != null)
            spriteTarget.SetAlpha(value * fParent.color.a);
        else
            spriteTarget.SetAlpha(value);
    }
}
