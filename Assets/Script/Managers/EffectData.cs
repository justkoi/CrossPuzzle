using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData : MonoBehaviour
{
    public float fTime;
    private float fTimer;

    
    void Start()
    {
        fTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Playing();
        if(isExpired())
        {
            Destroy(this.gameObject);
        }
    }

    private void Playing()
    {
        fTimer += Time.deltaTime;

    }
    private bool isExpired()
    {
        return fTimer >= fTime;
    }
}
