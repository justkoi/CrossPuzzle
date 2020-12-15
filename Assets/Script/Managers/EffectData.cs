using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData : MonoBehaviour
{
    public float fTime;
    private float fTimer;

    public tk2dTextMesh textName;
    public Vector2 power;
    
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
        this.transform.SetLocalPositionXY(this.transform.localPosition.x + (power.x * Time.deltaTime), this.transform.localPosition.y + (power.y * Time.deltaTime));
    }

    public void SetText(string text)
    {
        textName.text = text;
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
